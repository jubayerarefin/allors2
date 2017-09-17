// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurchaseInvoice.cs" company="Allors bvba">
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
    using System;
    using Meta;
    using Resources;

    public partial class PurchaseInvoice
    {
        ObjectState Transitional.CurrentObjectState => this.CurrentObjectState;

        public InvoiceItem[] InvoiceItems => this.PurchaseInvoiceItems;

        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistCurrentObjectState)
            {
                this.CurrentObjectState = new PurchaseInvoiceObjectStates(this.Strategy.Session).InProcess;
            }

            if (!this.ExistInvoiceNumber && this.ExistBilledToInternalOrganisation)
            {
                this.InvoiceNumber = this.BilledToInternalOrganisation.DeriveNextPurchaseInvoiceNumber();
            }

            if (!this.ExistInvoiceDate)
            {
                this.InvoiceDate = DateTime.UtcNow;
            }

            if (!this.ExistEntryDate)
            {
                this.EntryDate = DateTime.UtcNow;
            }
        }

        public void AppsOnPreDerive(ObjectOnPreDerive method)
        {
            var derivation = method.Derivation;

            // TODO:
            if (derivation.ChangeSet.Associations.Contains(this.Id))
            {
                if (this.ExistBilledToInternalOrganisation)
                {
                    derivation.AddDependency(this, this.BilledToInternalOrganisation);
                }
            }

            if (this.ExistBilledFromParty)
            {
                var supplier = this.BilledFromParty as Organisation;
                if (supplier != null)
                {
                    var supplierRelationships = supplier.SupplierRelationshipsWhereSupplier;
                    supplierRelationships.Filter.AddEquals(M.SupplierRelationship.InternalOrganisation, this.BilledToInternalOrganisation);

                    foreach (SupplierRelationship supplierRelationship in supplierRelationships)
                    {
                        if (supplierRelationship.FromDate <= DateTime.UtcNow && (!supplierRelationship.ExistThroughDate || supplierRelationship.ThroughDate >= DateTime.UtcNow))
                        {
                            derivation.AddDependency(this, supplierRelationship);
                        }
                    }
                }
            }
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            Organisation supplier = this.BilledFromParty as Organisation;
            if (supplier != null && this.ExistBilledToInternalOrganisation)
            {
                if (!this.BilledToInternalOrganisation.Equals(supplier.InternalOrganisationWhereSupplier))
                {
                    derivation.Validation.AddError(this, this.Meta.BilledFromParty, ErrorMessages.PartyIsNotASupplier);
                }
            }

            this.AppsOnDeriveInvoiceItems(derivation);
            this.AppsOnDeriveInvoiceTotals();
        }

        public void AppsOnPostDerive(ObjectOnPostDerive method)
        {
            var isNewVersion =
                !this.ExistCurrentVersion ||
                !object.Equals(this.InternalComment, this.CurrentVersion.InternalComment) ||
                !object.Equals(this.TotalShippingAndHandlingCustomerCurrency, this.CurrentVersion.TotalShippingAndHandlingCustomerCurrency) ||
                !object.Equals(this.Description, this.CurrentVersion.Description) ||
                !object.Equals(this.ShippingAndHandlingCharge, this.CurrentVersion.ShippingAndHandlingCharge) ||
                !object.Equals(this.Fee, this.CurrentVersion.Fee) ||
                !object.Equals(this.CustomerReference, this.CurrentVersion.CustomerReference) ||
                !object.Equals(this.DiscountAdjustment, this.CurrentVersion.DiscountAdjustment) ||
                !object.Equals(this.BillingAccount, this.CurrentVersion.BillingAccount) ||
                !object.Equals(this.InvoiceDate, this.CurrentVersion.InvoiceDate) ||
                !object.Equals(this.SurchargeAdjustment, this.CurrentVersion.SurchargeAdjustment) ||
                !object.Equals(this.InvoiceTerms, this.CurrentVersion.InvoiceTerms) ||
                !object.Equals(this.InvoiceNumber, this.CurrentVersion.InvoiceNumber) ||
                !object.Equals(this.Message, this.CurrentVersion.Message) ||
                !object.Equals(this.VatRegime, this.CurrentVersion.VatRegime) ||
                !object.Equals(this.PurchaseInvoiceItems, this.CurrentVersion.PurchaseInvoiceItems) ||
                !object.Equals(this.BilledToInternalOrganisation, this.CurrentVersion.BilledToInternalOrganisation) ||
                !object.Equals(this.BilledFromParty, this.CurrentVersion.BilledFromParty) ||
                !object.Equals(this.PurchaseInvoiceType, this.CurrentVersion.PurchaseInvoiceType);

            var isNewStateVersion =
                !this.ExistCurrentVersion ||
                !object.Equals(this.CurrentObjectState, this.CurrentVersion.CurrentObjectState);

            if (isNewVersion)
            {
                this.PreviousVersion = this.CurrentVersion;
                this.CurrentVersion = new PurchaseInvoiceVersionBuilder(this.Strategy.Session).WithPurchaseInvoice(this).Build();
                this.AddAllVersion(this.CurrentVersion);
            }

            if (isNewStateVersion)
            {
                this.CurrentStateVersion = CurrentVersion;
                this.AddAllStateVersion(this.CurrentStateVersion);
            }
        }

        public void AppsOnDeriveInvoiceTotals()
        {
            if (this.ExistPurchaseInvoiceItems)
            {
                this.TotalBasePrice = 0;
                this.TotalDiscount = 0;
                this.TotalSurcharge = 0;
                this.TotalVat = 0;
                this.TotalExVat = 0;
                this.TotalIncVat = 0;

                foreach (PurchaseInvoiceItem item in this.PurchaseInvoiceItems)
                {
                    this.TotalBasePrice += item.TotalBasePrice;
                    this.TotalSurcharge += item.TotalSurcharge;
                    this.TotalSurcharge += item.TotalSurcharge;
                    this.TotalVat += item.TotalVat;
                    this.TotalExVat += item.TotalExVat;
                    this.TotalIncVat += item.TotalIncVat;
                }
            }
        }

        public void AppsSearchDataApprove(IDerivation derivation)
        {
            this.CurrentObjectState = new PurchaseInvoiceObjectStates(this.Strategy.Session).Approved;
        }

        public void AppsReady(IDerivation derivation)
        {
            this.CurrentObjectState = new PurchaseInvoiceObjectStates(this.Strategy.Session).ReadyForPosting;
        }

        public void AppsCancel(IDerivation derivation)
        {
            this.CurrentObjectState = new PurchaseInvoiceObjectStates(this.Strategy.Session).Cancelled;
        }

        public void AppsOnDeriveInvoiceItems(IDerivation derivation)
        {
            foreach (PurchaseInvoiceItem purchaseInvoiceItem in this.PurchaseInvoiceItems)
            {
                purchaseInvoiceItem.AppsOnDerivePrices();
            }
        }
    }
}
