// <copyright file="ProductQuote.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Allors.Meta;
    using Allors.Services;

    using Microsoft.Extensions.DependencyInjection;

    public partial class ProductQuote
    {
        public static readonly TransitionalConfiguration[] StaticTransitionalConfigurations =
            {
                new TransitionalConfiguration(M.ProductQuote, M.ProductQuote.QuoteState),
            };

        public TransitionalConfiguration[] TransitionalConfigurations => StaticTransitionalConfigurations;

        public void BaseApprove(ProductQuoteApprove method) => this.QuoteState = new QuoteStates(this.strategy.Session).Approved;

        public void BaseReject(ProductQuoteReject method) => this.QuoteState = new QuoteStates(this.strategy.Session).Rejected;

        public void BaseOrder(ProductQuoteOrder method)
        {
            this.QuoteState = new QuoteStates(this.Strategy.Session).Ordered;
            this.OrderThis();
        }

        public void BasePrint(PrintablePrint method)
        {
            if (!method.IsPrinted)
            {
                var singleton = this.Strategy.Session.GetSingleton();
                var logo = this.Issuer?.ExistLogoImage == true ?
                               this.Issuer.LogoImage.MediaContent.Data :
                               singleton.LogoImage.MediaContent.Data;

                var images = new Dictionary<string, byte[]>
                                 {
                                     { "Logo", logo },
                                 };

                if (this.ExistQuoteNumber)
                {
                    var session = this.Strategy.Session;
                    var barcodeService = session.ServiceProvider.GetRequiredService<IBarcodeService>();
                    var barcode = barcodeService.Generate(this.QuoteNumber, BarcodeType.CODE_128, 320, 80);
                    images.Add("Barcode", barcode);
                }

                var printModel = new Print.ProductQuoteModel.Model(this, images);
                this.RenderPrintDocument(this.Issuer?.ProductQuoteTemplate, printModel, images);

                this.PrintDocument.Media.FileName = $"{this.QuoteNumber}.odt";
            }
        }

        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;
            var session = this.strategy.Session;

            // SalesOrderItem Derivations and Validations
            foreach (QuoteItem quoteItem in this.QuoteItems)
            {
                var isSubTotalItem = quoteItem.ExistInvoiceItemType && (quoteItem.InvoiceItemType.ProductItem || quoteItem.InvoiceItemType.PartItem);
                if (isSubTotalItem)
                {
                    if (quoteItem.Quantity == 0)
                    {
                        derivation.Validation.AddError(quoteItem, M.QuoteItem.Quantity, "Quantity is Required");
                    }
                }
                else
                {
                    if (quoteItem.UnitPrice == 0)
                    {
                        derivation.Validation.AddError(quoteItem, M.QuoteItem.UnitPrice, "Price is Required");
                    }
                }
            }

            var currentPriceComponents = new PriceComponents(session).CurrentPriceComponents(this.IssueDate);

            var quantityOrderedByProduct = this.QuoteItems
                .Where(v => v.ExistProduct)
                .GroupBy(v => v.Product)
                .ToDictionary(v => v.Key, v => v.Sum(w => w.Quantity));

            // First run to calculate price
            foreach (QuoteItem quoteItem in this.QuoteItems)
            {
                decimal quantityOrdered = 0;

                if (quoteItem.ExistProduct)
                {
                    quantityOrderedByProduct.TryGetValue(quoteItem.Product, out quantityOrdered);
                }

                foreach (QuoteItem featureItem in quoteItem.QuotedWithFeatures)
                {
                    featureItem.Quantity = quoteItem.Quantity;
                    this.CalculatePrices(derivation, featureItem, currentPriceComponents, quantityOrdered, 0);
                }

                this.CalculatePrices(derivation, quoteItem, currentPriceComponents, quantityOrdered, 0);
            }

            var totalBasePriceByProduct = this.QuoteItems
                .Where(v => v.ExistProduct)
                .GroupBy(v => v.Product)
                .ToDictionary(v => v.Key, v => v.Sum(w => w.TotalBasePrice));

            // Second run to calculate price (because of order value break)
            foreach (QuoteItem quoteItem in this.QuoteItems)
            {
                decimal quantityOrdered = 0;
                decimal totalBasePrice = 0;

                if (quoteItem.ExistProduct)
                {
                    quantityOrderedByProduct.TryGetValue(quoteItem.Product, out quantityOrdered);
                    totalBasePriceByProduct.TryGetValue(quoteItem.Product, out totalBasePrice);
                }

                foreach (QuoteItem featureItem in quoteItem.QuotedWithFeatures)
                {
                    this.CalculatePrices(derivation, featureItem, currentPriceComponents, quantityOrdered, totalBasePrice);
                }

                this.CalculatePrices(derivation, quoteItem, currentPriceComponents, quantityOrdered, totalBasePrice);
            }

            // Calculate Totals
            if (this.ExistQuoteItems)
            {
                this.TotalBasePrice = 0;
                this.TotalDiscount = 0;
                this.TotalSurcharge = 0;
                this.TotalExVat = 0;
                this.TotalFee = 0;
                this.TotalShippingAndHandling = 0;
                this.TotalVat = 0;
                this.TotalIncVat = 0;
                this.TotalListPrice = 0;

                foreach (QuoteItem quoteItem in this.QuoteItems)
                {
                    if (!quoteItem.ExistQuoteItemWhereQuotedWithFeature)
                    {
                        this.TotalBasePrice += quoteItem.TotalBasePrice;
                        this.TotalDiscount += quoteItem.TotalDiscount;
                        this.TotalSurcharge += quoteItem.TotalSurcharge;
                        this.TotalExVat += quoteItem.TotalExVat;
                        this.TotalVat += quoteItem.TotalVat;
                        this.TotalIncVat += quoteItem.TotalIncVat;
                        this.TotalListPrice += quoteItem.UnitPrice;
                    }
                }

                if (this.ExistDiscountAdjustment)
                {
                    var discount = this.DiscountAdjustment.Percentage.HasValue ?
                                           Math.Round(this.TotalExVat * this.DiscountAdjustment.Percentage.Value / 100, 2) :
                                           this.DiscountAdjustment.Amount ?? 0;

                    this.TotalDiscount += discount;
                    this.TotalExVat -= discount;

                    if (this.ExistVatRegime)
                    {
                        var vat = Math.Round(discount * this.VatRegime.VatRate.Rate / 100, 2);

                        this.TotalVat -= vat;
                        this.TotalIncVat -= discount + vat;
                    }
                }

                if (this.ExistSurchargeAdjustment)
                {
                    var surcharge = this.SurchargeAdjustment.Percentage.HasValue ?
                                            Math.Round(this.TotalExVat * this.SurchargeAdjustment.Percentage.Value / 100, 2) :
                                            this.SurchargeAdjustment.Amount ?? 0;

                    this.TotalSurcharge += surcharge;
                    this.TotalExVat += surcharge;

                    if (this.ExistVatRegime)
                    {
                        var vat = Math.Round(surcharge * this.VatRegime.VatRate.Rate / 100, 2);
                        this.TotalVat += vat;
                        this.TotalIncVat += surcharge + vat;
                    }
                }

                if (this.ExistFee)
                {
                    var fee = this.Fee.Percentage.HasValue ?
                                      Math.Round(this.TotalExVat * this.Fee.Percentage.Value / 100, 2) :
                                      this.Fee.Amount ?? 0;

                    this.TotalFee += fee;
                    this.TotalExVat += fee;

                    if (this.Fee.ExistVatRate)
                    {
                        var vat1 = Math.Round(fee * this.Fee.VatRate.Rate / 100, 2);
                        this.TotalVat += vat1;
                        this.TotalIncVat += fee + vat1;
                    }
                }

                if (this.ExistShippingAndHandlingCharge)
                {
                    var shipping = this.ShippingAndHandlingCharge.Percentage.HasValue ?
                                           Math.Round(this.TotalExVat * this.ShippingAndHandlingCharge.Percentage.Value / 100, 2) :
                                           this.ShippingAndHandlingCharge.Amount ?? 0;

                    this.TotalShippingAndHandling += shipping;
                    this.TotalExVat += shipping;

                    if (this.ShippingAndHandlingCharge.ExistVatRate)
                    {
                        var vat2 = Math.Round(shipping * this.ShippingAndHandlingCharge.VatRate.Rate / 100, 2);
                        this.TotalVat += vat2;
                        this.TotalIncVat += shipping + vat2;
                    }
                }

                //// Only take into account items for which there is data at the item level.
                //// Skip negative sales.
                decimal totalUnitBasePrice = 0;
                decimal totalListPrice = 0;

                foreach (QuoteItem item1 in this.QuoteItems)
                {
                    if (item1.TotalExVat > 0)
                    {
                        totalUnitBasePrice += item1.UnitBasePrice;
                        totalListPrice += item1.UnitPrice;
                    }
                }
            }

            this.DeriveWorkflow();

            this.Sync(this.Strategy.Session);

            this.ResetPrintDocument();
        }

        public void CalculatePrices(
            IDerivation derivation,
            QuoteItem quoteItem,
            PriceComponent[] currentPriceComponents,
            decimal quantityOrdered,
            decimal totalBasePrice)
        {
            var currentGenericOrProductOrFeaturePriceComponents = Array.Empty<PriceComponent>();
            if (quoteItem.ExistProduct)
            {
                currentGenericOrProductOrFeaturePriceComponents = quoteItem.Product.GetPriceComponents(currentPriceComponents);
            }
            else if (quoteItem.ExistProductFeature)
            {
                currentGenericOrProductOrFeaturePriceComponents = quoteItem.ProductFeature.GetPriceComponents(quoteItem.QuoteItemWhereQuotedWithFeature.Product, currentPriceComponents);
            }

            var priceComponents = currentGenericOrProductOrFeaturePriceComponents.Where(
                v => PriceComponents.BaseIsApplicable(
                    new PriceComponents.IsApplicable
                    {
                        PriceComponent = v,
                        Customer = this.Receiver,
                        Product = quoteItem.Product,
                        QuantityOrdered = quantityOrdered,
                        ValueOrdered = totalBasePrice,
                    })).ToArray();

            var unitBasePrice = priceComponents.OfType<BasePrice>().Min(v => v.Price);

            // Calculate Unit Price (with Discounts and Surcharges)
            if (quoteItem.AssignedUnitPrice.HasValue)
            {
                quoteItem.UnitBasePrice = unitBasePrice ?? 0M;
                quoteItem.UnitDiscount = 0;
                quoteItem.UnitSurcharge = 0;
                quoteItem.UnitPrice = quoteItem.AssignedUnitPrice.Value;
            }
            else
            {
                if (!unitBasePrice.HasValue)
                {
                    derivation.Validation.AddError(quoteItem, M.SalesOrderItem.UnitBasePrice, "No BasePrice with a Price");
                    return;
                }

                quoteItem.UnitBasePrice = unitBasePrice.Value;

                quoteItem.UnitDiscount = priceComponents.OfType<DiscountComponent>().Sum(
                    v => v.Percentage.HasValue
                             ? Math.Round(quoteItem.UnitBasePrice * v.Percentage.Value / 100, 2)
                             : v.Price ?? 0);

                quoteItem.UnitSurcharge = priceComponents.OfType<SurchargeComponent>().Sum(
                    v => v.Percentage.HasValue
                             ? Math.Round(quoteItem.UnitBasePrice * v.Percentage.Value / 100, 2)
                             : v.Price ?? 0);

                quoteItem.UnitPrice = quoteItem.UnitBasePrice - quoteItem.UnitDiscount + quoteItem.UnitSurcharge;

                if (quoteItem.ExistDiscountAdjustment)
                {
                    quoteItem.UnitDiscount += quoteItem.DiscountAdjustment.Percentage.HasValue ?
                        Math.Round(quoteItem.UnitPrice * quoteItem.DiscountAdjustment.Percentage.Value / 100, 2) :
                        quoteItem.DiscountAdjustment.Amount ?? 0;
                }

                if (quoteItem.ExistSurchargeAdjustment)
                {
                    quoteItem.UnitSurcharge += quoteItem.SurchargeAdjustment.Percentage.HasValue ?
                        Math.Round(quoteItem.UnitPrice * quoteItem.SurchargeAdjustment.Percentage.Value / 100, 2) :
                        quoteItem.SurchargeAdjustment.Amount ?? 0;
                }

                quoteItem.UnitPrice = quoteItem.UnitBasePrice - quoteItem.UnitDiscount + quoteItem.UnitSurcharge;
            }

            foreach (QuoteItem featureItem in quoteItem.QuotedWithFeatures)
            {
                quoteItem.UnitBasePrice += featureItem.UnitBasePrice;
                quoteItem.UnitPrice += featureItem.UnitPrice;
                quoteItem.UnitDiscount += featureItem.UnitDiscount;
                quoteItem.UnitSurcharge += featureItem.UnitSurcharge;
            }

            quoteItem.UnitVat = quoteItem.ExistVatRate ? Math.Round(quoteItem.UnitPrice * quoteItem.VatRate.Rate / 100, 2) : 0;

            // Calculate Totals
            quoteItem.TotalBasePrice = quoteItem.UnitBasePrice * quoteItem.Quantity;
            quoteItem.TotalDiscount = quoteItem.UnitDiscount * quoteItem.Quantity;
            quoteItem.TotalSurcharge = quoteItem.UnitSurcharge * quoteItem.Quantity;
            quoteItem.TotalPriceAdjustment = quoteItem.TotalSurcharge - quoteItem.TotalDiscount;

            if (quoteItem.TotalBasePrice > 0)
            {
                quoteItem.TotalDiscountAsPercentage = Math.Round(quoteItem.TotalDiscount / quoteItem.TotalBasePrice * 100, 2);
                quoteItem.TotalSurchargeAsPercentage = Math.Round(quoteItem.TotalSurcharge / quoteItem.TotalBasePrice * 100, 2);
            }
            else
            {
                quoteItem.TotalDiscountAsPercentage = 0;
                quoteItem.TotalSurchargeAsPercentage = 0;
            }

            quoteItem.TotalExVat = quoteItem.UnitPrice * quoteItem.Quantity;
            quoteItem.TotalVat = quoteItem.UnitVat * quoteItem.Quantity;
            quoteItem.TotalIncVat = quoteItem.TotalExVat + quoteItem.TotalVat;
        }

        private void Sync(ISession session)
        {
            // session.Prefetch(this.SyncPrefetch, this);
            foreach (QuoteItem quoteItem in this.QuoteItems)
            {
                quoteItem.Sync(this);
            }
        }

        private void DeriveWorkflow()
        {
            this.WorkItemDescription = $"ProductQuote: {this.QuoteNumber} [{this.Issuer?.PartyName}]";

            var openTasks = this.TasksWhereWorkItem.Where(v => !v.ExistDateClosed).ToArray();

            if (this.QuoteState.IsCreated)
            {
                if (!openTasks.OfType<ProductQuoteApproval>().Any())
                {
                    new ProductQuoteApprovalBuilder(this.strategy.Session).WithProductQuote(this).Build();
                }
            }
        }

        private SalesOrder OrderThis()
        {
            var salesOrder = new SalesOrderBuilder(this.Strategy.Session)
                .WithTakenBy(this.Issuer)
                .WithBillToCustomer(this.Receiver)
                .WithDescription(this.Description)
                .WithShipToContactPerson(this.ContactPerson)
                .WithBillToContactPerson(this.ContactPerson)
                .WithQuote(this)
                .Build();

            var quoteItems = this.QuoteItems.Where(i => i.QuoteItemState.Equals(new QuoteItemStates(this.Strategy.Session).Submitted)).ToArray();

            foreach (var quoteItem in quoteItems)
            {
                quoteItem.QuoteItemState = new QuoteItemStates(this.Strategy.Session).Ordered;

                salesOrder.AddSalesOrderItem(
                    new SalesOrderItemBuilder(this.Strategy.Session)
                        .WithInvoiceItemType(quoteItem.InvoiceItemType)
                        .WithInternalComment(quoteItem.InternalComment)
                        .WithAssignedDeliveryDate(quoteItem.EstimatedDeliveryDate)
                        .WithAssignedUnitPrice(quoteItem.UnitPrice)
                        .WithProduct(quoteItem.Product)
                        .WithSerialisedItem(quoteItem.SerialisedItem)
                        .WithProductFeature(quoteItem.ProductFeature)
                        .WithQuantityOrdered(quoteItem.Quantity).Build());
            }

            return salesOrder;
        }
    }
}