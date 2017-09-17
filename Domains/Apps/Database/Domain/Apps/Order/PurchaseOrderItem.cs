// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurchaseOrderItem.cs" company="Allors bvba">
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
namespace Allors.Domain
{
    using Meta;

    public partial class PurchaseOrderItem
    {
        ObjectState Transitional.CurrentObjectState => this.CurrentObjectState;

        public string SupplierReference
        {
            get
            {
                Extent<SupplierOffering> offerings = null;

                if (this.ExistProduct)
                {
                    offerings = this.Product.SupplierOfferingsWhereProduct;
                }

                if (this.ExistPart)
                {
                    offerings = this.Part.SupplierOfferingsWherePart;
                }

                if (offerings != null)
                {
                    offerings.Filter.AddEquals(M.SupplierOffering.Supplier, this.IPurchaseOrderWherePurchaseOrderItem.TakenViaSupplier);
                    foreach (SupplierOffering offering in offerings)
                    {
                        if (offering.FromDate <= this.IPurchaseOrderWherePurchaseOrderItem.OrderDate &&
                            (!offering.ExistThroughDate || offering.ThroughDate >= this.IPurchaseOrderWherePurchaseOrderItem.OrderDate))
                        {
                            return offering.ReferenceNumber;
                        }
                    }
                }

                return string.Empty;
            }
        }

        public void AppsConfirm(OrderItemConfirm method)
        {
            this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).InProcess;
        }

        public void AppsApprove(OrderItemApprove method)
        {
            this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).InProcess;
        }

        public void AppsCancel(OrderItemCancel method)
        {
            this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).Cancelled;
        }

        public void AppsReject(OrderItemReject method)
        {
            this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).Rejected;
        }

        public void AppsComplete(PurchaseOrderItemComplete method)
        {
            this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).Completed;
        }

        public void AppsFinish(OrderItemFinish method)
        {
            this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).Finished;
        }

        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistCurrentObjectState)
            {
                this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).Created;
            }
        }

        public void AppsOnPreDerive(ObjectOnPreDerive method)
        {
            var derivation = method.Derivation;

            // TODO:
            if (derivation.ChangeSet.Associations.Contains(this.Id))
            {
                if (this.ExistIPurchaseOrderWherePurchaseOrderItem)
                {
                    derivation.AddDependency(this.IPurchaseOrderWherePurchaseOrderItem, this);
                }
            }
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;
            
            derivation.Validation.AssertAtLeastOne(this, M.PurchaseOrderItem.Product, M.PurchaseOrderItem.Part);
            derivation.Validation.AssertExistsAtMostOne(this, M.PurchaseOrderItem.Product, M.PurchaseOrderItem.Part);

            this.AppsDeriveVatRegime(derivation);

            this.AppsOnDeriveIsValidOrderItem(derivation);

            this.AppsOnDeriveCurrentObjectState(derivation);
        }

        public void AppsOnPostDerive(ObjectOnPostDerive method)
        {
            var isNewVersion =
                !this.ExistCurrentVersion ||
                !object.Equals(this.InternalComment, this.CurrentVersion.InternalComment);

            var isNewStateVersion =
                !this.ExistCurrentVersion ||
                !object.Equals(this.DiscountAdjustment, this.CurrentVersion.DiscountAdjustment) ||
                !object.Equals(this.ActualUnitPrice, this.CurrentVersion.ActualUnitPrice) ||
                !object.Equals(this.AssignedVatRegime, this.CurrentVersion.AssignedVatRegime) ||
                !object.Equals(this.CurrentPriceComponents, this.CurrentVersion.CurrentPriceComponents) ||
                !object.Equals(this.SurchargeAdjustment, this.CurrentVersion.SurchargeAdjustment) ||
                !object.Equals(this.InternalComment, this.CurrentVersion.InternalComment) ||
                !object.Equals(this.BudgetItem, this.CurrentVersion.BudgetItem) ||
                !object.Equals(this.QuantityOrdered, this.CurrentVersion.QuantityOrdered) ||
                !object.Equals(this.Description, this.CurrentVersion.Description) ||
                !object.Equals(this.CorrespondingPurchaseOrder, this.CurrentVersion.CorrespondingPurchaseOrder) ||
                !object.Equals(this.QuoteItem, this.CurrentVersion.QuoteItem) ||
                !object.Equals(this.AssignedDeliveryDate, this.CurrentVersion.AssignedDeliveryDate) ||
                !object.Equals(this.OrderTerms, this.CurrentVersion.OrderTerms) ||
                !object.Equals(this.ShippingInstruction, this.CurrentVersion.ShippingInstruction) ||
                !object.Equals(this.Associations, this.CurrentVersion.Associations) ||
                !object.Equals(this.Message, this.CurrentVersion.Message) ||
                !object.Equals(this.QuantityReceived, this.CurrentVersion.QuantityReceived) ||
                !object.Equals(this.Product, this.CurrentVersion.Product) ||
                !object.Equals(this.Part, this.CurrentVersion.Part) ||
                !object.Equals(this.CurrentObjectState, this.CurrentVersion.CurrentObjectState);

            if (isNewVersion)
            {
                this.PreviousVersion = this.CurrentVersion;
                this.CurrentVersion = new PurchaseOrderItemVersionBuilder(this.Strategy.Session).WithPurchaseOrderItem(this).Build();
                this.AddAllVersion(this.CurrentVersion);
            }

            if (isNewStateVersion)
            {
                this.CurrentStateVersion = CurrentVersion;
                this.AddAllStateVersion(this.CurrentStateVersion);
            }
        }

        public void AppsDeriveVatRegime(IDerivation derivation)
        {
            if (this.ExistIPurchaseOrderWherePurchaseOrderItem)
            {
                this.VatRegime = this.ExistAssignedVatRegime ? this.AssignedVatRegime : this.IPurchaseOrderWherePurchaseOrderItem.VatRegime;

                this.AppsDeriveVatRate(derivation);
            }
        }

        public void AppsDeriveVatRate(IDerivation derivation)
        {
            if (!this.ExistDerivedVatRate && this.ExistVatRegime && this.VatRegime.ExistVatRate)
            {
                this.DerivedVatRate = this.VatRegime.VatRate;
            }

            if (!this.ExistDerivedVatRate && this.ExistProduct)
            {
                this.DerivedVatRate = this.Product.VatRate;
            }
        }

        public void AppsOnDeriveIsValidOrderItem(IDerivation derivation)
        {
            if (this.ExistIPurchaseOrderWherePurchaseOrderItem)
            {
                this.IPurchaseOrderWherePurchaseOrderItem.RemoveValidOrderItem(this);

                if (!this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Cancelled)
                    && !this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Rejected))
                {
                    this.IPurchaseOrderWherePurchaseOrderItem.AddValidOrderItem(this);
                }
            }
        }

        public void AppsOnDeriveCurrentObjectState(IDerivation derivation)
        {
            if (this.ExistIOrderWhereValidOrderItem)
            {
                var order = this.IPurchaseOrderWherePurchaseOrderItem;

                if (order.CurrentObjectState.Equals(new PurchaseOrderObjectStates(this.Strategy.Session).Cancelled))
                {
                    this.Cancel();
                }

                if (order.CurrentObjectState.Equals(new PurchaseOrderObjectStates(this.Strategy.Session).InProcess))
                {
                    if (this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Created))
                    {
                        this.Confirm();
                    }
                }

                if (order.CurrentObjectState.Equals(new PurchaseOrderObjectStates(this.Strategy.Session).Completed))
                {
                    this.Complete();
                }

                if (order.CurrentObjectState.Equals(new PurchaseOrderObjectStates(this.Strategy.Session).Finished))
                {
                    this.Finish();
                }
            }

            if (this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).InProcess))
            {
                this.AppsOnDeriveQuantities(derivation);

                this.PreviousQuantity = this.QuantityOrdered;
            }

            if (this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Cancelled) ||
                this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Rejected))
            {
                this.AppsOnDeriveQuantities(derivation);
            }
        }

        public void AppsOnDeriveCurrentOrderStatus(IDerivation derivation)
        {
            if (this.ExistCurrentShipmentStateVersion && this.CurrentShipmentStateVersion.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).PartiallyReceived))
            {
                this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).PartiallyReceived;
                this.AppsOnDeriveCurrentObjectState(derivation);
            }

            if (this.ExistCurrentShipmentStateVersion && this.CurrentShipmentStateVersion.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Received))
            {
                this.CurrentObjectState = new PurchaseOrderItemObjectStates(this.Strategy.Session).Completed;
                this.AppsOnDeriveCurrentObjectState(derivation);
            }
        }

        public void AppsOnDeriveDeliveryDate(IDerivation derivation)
        {
            if (this.AssignedDeliveryDate.HasValue)
            {
                this.DeliveryDate = this.AssignedDeliveryDate.Value;
            }
            else if (this.IPurchaseOrderWherePurchaseOrderItem.DeliveryDate.HasValue)
            {
                this.DeliveryDate = this.IPurchaseOrderWherePurchaseOrderItem.DeliveryDate.Value;
            }            
        }

        public void AppsOnDerivePrices()
        {
            this.UnitBasePrice = 0;
            this.UnitDiscount = 0;
            this.UnitSurcharge = 0;

            if (this.ActualUnitPrice.HasValue)
            {
                this.UnitBasePrice = this.ActualUnitPrice.Value;
                this.CalculatedUnitPrice = this.ActualUnitPrice.Value;
            }
            else
            {
                var order = this.IPurchaseOrderWherePurchaseOrderItem;
                var productPurchasePrice = new SupplierOfferings(this.Strategy.Session).PurchasePrice(order.TakenViaSupplier, order.OrderDate, this.Product, this.Part);
                this.UnitBasePrice = productPurchasePrice != null ? productPurchasePrice.Price : 0M;
            }

            this.UnitVat = 0;
            this.TotalBasePrice = this.UnitBasePrice * this.QuantityOrdered;
            this.TotalDiscount = this.UnitDiscount * this.QuantityOrdered;
            this.TotalSurcharge = this.UnitSurcharge * this.QuantityOrdered;
            this.CalculatedUnitPrice = this.UnitBasePrice - this.UnitDiscount + this.UnitSurcharge;
            this.TotalVat = 0;
            this.TotalExVat = this.CalculatedUnitPrice * this.QuantityOrdered;
            this.TotalIncVat = this.TotalExVat + this.TotalVat;
        }

        public void AppsOnDeriveCurrentShipmentStatus(IDerivation derivation)
        {
            var quantityReceived = 0M;
            foreach (ShipmentReceipt shipmentReceipt in this.ShipmentReceiptsWhereOrderItem)
            {
                quantityReceived += shipmentReceipt.QuantityAccepted;
            }

            this.QuantityReceived = quantityReceived;

            if (quantityReceived > 0)
            {
                if (quantityReceived < this.QuantityOrdered)
                {
                    var newVersion = new PurchaseOrderItemVersionBuilder(this.Strategy.Session)
                        .WithCurrentObjectState(new PurchaseOrderItemObjectStates(this.Strategy.Session).PartiallyReceived)
                        .WithPurchaseOrderItem(this)
                        .Build();
                    this.CurrentShipmentStateVersion = newVersion;
                    this.AddAllShipmentStateVersion(newVersion);
                }
                else
                {
                    var newVersion = new PurchaseOrderItemVersionBuilder(this.Strategy.Session)
                        .WithCurrentObjectState(new PurchaseOrderItemObjectStates(this.Strategy.Session).Received)
                        .WithPurchaseOrderItem(this)
                        .Build();
                    this.CurrentShipmentStateVersion = newVersion;
                    this.AddAllShipmentStateVersion(newVersion);
                }
            }

            this.AppsOnDeriveCurrentOrderStatus(derivation);

            if (this.ExistIPurchaseOrderWherePurchaseOrderItem)
            {
                var purchaseOrder = (PurchaseOrder)this.IPurchaseOrderWherePurchaseOrderItem;
                purchaseOrder.AppsOnDeriveCurrentShipmentStatus(derivation);
            }
        }

        public void AppsOnDeriveQuantities(IDerivation derivation)
        {
            NonSerialisedInventoryItem inventoryItem = null;

            if (this.ExistProduct)
            {
                var good = this.Product as Good;
                if (good != null)
                {
                    var inventoryItems = good.IInventoryItemsWhereGood;
                    inventoryItems.Filter.AddEquals(M.IInventoryItem.Facility, this.IPurchaseOrderWherePurchaseOrderItem.Facility);
                    inventoryItem = inventoryItems.First as NonSerialisedInventoryItem;
                }
            }

            if (this.ExistPart)
            {
                var inventoryItems = this.Part.IInventoryItemsWherePart;
                inventoryItems.Filter.AddEquals(M.IInventoryItem.Facility, this.IPurchaseOrderWherePurchaseOrderItem.Facility);
                inventoryItem = inventoryItems.First as NonSerialisedInventoryItem;
            }

            if (this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).InProcess))
            {
                if (!this.ExistPreviousQuantity || !this.QuantityOrdered.Equals(this.PreviousQuantity))
                {
                    if (inventoryItem != null)
                    {
                        inventoryItem.OnDerive(x => x.WithDerivation(derivation));
                    }
                }
            }

            if (this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Cancelled) ||
                this.CurrentObjectState.Equals(new PurchaseOrderItemObjectStates(this.Strategy.Session).Rejected))
            {
                if (inventoryItem != null)
                {
                    inventoryItem.OnDerive(x => x.WithDerivation(derivation));
                }
            }
        }
    }
}