// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepeatingPurchaseInvoice.cs" company="Allors bvba">
//   Copyright 2002-2012 Allors bvba.
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// Allors Applications is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Allors.Domain
{
    using System;

    using Allors.Meta;

    using Resources;

    public partial class RepeatingPurchaseInvoice
    {
        public void AppsOnInit(ObjectOnInit method)
        {
            var internalOrganisations = new Organisations(this.Strategy.Session).Extent().Where(v => Equals(v.IsInternalOrganisation, true)).ToArray();

            if (!this.ExistInternalOrganisation && internalOrganisations.Count() == 1)
            {
                this.InternalOrganisation = internalOrganisations.First();
            }
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;
            if (!this.Frequency.Equals(new TimeFrequencies(this.Strategy.Session).Month) && !this.Frequency.Equals(new TimeFrequencies(this.Strategy.Session).Week))
            {
                derivation.Validation.AddError(this, M.RepeatingPurchaseInvoice.Frequency, ErrorMessages.FrequencyNotSupported);
            }

            if (this.Frequency.Equals(new TimeFrequencies(this.Strategy.Session).Week) && !this.ExistDayOfWeek)
            {
                derivation.Validation.AssertExists(this, M.RepeatingPurchaseInvoice.DayOfWeek);
            }

            if (this.Frequency.Equals(new TimeFrequencies(this.Strategy.Session).Month) && this.ExistDayOfWeek)
            {
                derivation.Validation.AssertNotExists(this, M.RepeatingPurchaseInvoice.DayOfWeek);
            }

            if (this.Frequency.Equals(new TimeFrequencies(this.Strategy.Session).Week) && this.ExistDayOfWeek && this.ExistNextExecutionDate)
            {
                if (!this.NextExecutionDate.DayOfWeek.ToString().Equals(this.DayOfWeek.Name))
                {
                    derivation.Validation.AddError(this, M.RepeatingPurchaseInvoice.DayOfWeek, ErrorMessages.DateDayOfWeek);
                }
            }
        }

        public void Repeat()
        {
            var now = this.Strategy.Session.Now().Date;
            var monthly = new TimeFrequencies(this.Strategy.Session).Month;
            var weekly = new TimeFrequencies(this.Strategy.Session).Week;

            if (this.Frequency.Equals(monthly))
            {
                var nextDate = now.AddMonths(1).Date;
                this.Repeat(now, nextDate);
            }

            if (this.Frequency.Equals(weekly))
            {
                var nextDate = now.AddDays(7).Date;
                this.Repeat(now, nextDate);
            }
        }

        private void Repeat(DateTime now, DateTime nextDate)
        {
            if (!this.ExistFinalExecutionDate || nextDate <= this.FinalExecutionDate.Value.Date)
            {
                this.NextExecutionDate = nextDate.Date;
            }

            var orderCandidates = this.Supplier.PurchaseOrdersWhereTakenViaSupplier
                .Where(v => v.OrderedBy.Equals(this.InternalOrganisation) &&
                            v.PreviousPurchaseOrderState.IsSent &&
                            (v.PurchaseOrderShipmentState.IsReceived || v.PurchaseOrderShipmentState.IsPartiallyReceived));

            List<PurchaseOrderItem> orderItemsToBill = new List<PurchaseOrderItem>();
            foreach (PurchaseOrder purchaseOrder in orderCandidates)
            {
                foreach (PurchaseOrderItem purchaseOrderItem in purchaseOrder.PurchaseOrderItems)
                {
                    if (!purchaseOrderItem.ExistOrderItemBillingsWhereOrderItem &&
                        purchaseOrderItem.PreviousPurchaseOrderItemShipmentState.IsReceived || purchaseOrderItem.PreviousPurchaseOrderItemShipmentState.IsPartiallyReceived)
                    {
                        orderItemsToBill.Add(purchaseOrderItem);
                    }
                }
            }

            if (orderItemsToBill.Any())
            {
                var purchaseInvoice = new PurchaseInvoiceBuilder(this.Strategy.Session)
                    .WithBilledFrom(this.Supplier)
                    .WithBilledTo(this.InternalOrganisation)
                    .WithInvoiceDate(this.strategy.Session.Now())
                    .WithPurchaseInvoiceType(new PurchaseInvoiceTypes(this.strategy.Session).PurchaseInvoice)
                    .Build();

                foreach (PurchaseOrderItem orderItem in orderItemsToBill)
                {
                    purchaseInvoice.AddPurchaseOrder(orderItem.PurchaseOrderWherePurchaseOrderItem);

                    var invoiceItem = new PurchaseInvoiceItemBuilder(this.Strategy.Session)
                        .WithAssignedUnitPrice(orderItem.UnitPrice)
                        .WithPart(orderItem.Part)
                        .WithQuantity(orderItem.QuantityOrdered)
                        .WithDescription(orderItem.Description)
                        .WithInternalComment(orderItem.InternalComment)
                        .WithMessage(orderItem.Message)
                        .Build();

                    purchaseInvoice.AddPurchaseInvoiceItem(invoiceItem);

                    if (!purchaseInvoice.PurchaseOrders.Contains(orderItem.PurchaseOrderWherePurchaseOrderItem))
                    {
                        purchaseInvoice.AddPurchaseOrder(orderItem.PurchaseOrderWherePurchaseOrderItem);
                    }

                    new OrderItemBillingBuilder(this.Strategy.Session)
                        .WithQuantity(orderItem.QuantityOrdered)
                        .WithAmount(orderItem.TotalBasePrice)
                        .WithOrderItem(orderItem)
                        .WithInvoiceItem(invoiceItem)
                        .Build();
                }
            }

            this.PreviousExecutionDate = now.Date;
        }
    }
}