// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Organisations.cs" company="Allors bvba">
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
    using System.IO;

    using Meta;

    public partial class Organisations
    {
        protected override void AppsPrepare(Setup setup)
        {
            setup.AddDependency(this.Meta.ObjectType, M.InventoryStrategy);
            setup.AddDependency(this.Meta.ObjectType, M.Role);
            setup.AddDependency(this.Meta.ObjectType, M.OrganisationRole);
            setup.AddDependency(this.Meta.ObjectType, M.InvoiceSequence);
            setup.AddDependency(this.Meta.ObjectType, M.Singleton);
            setup.AddDependency(this.Meta.ObjectType, M.FacilityType);
        }

        public static Organisation CreateInternalOrganisation(
            ISession session,
            string name,
            string address,
            string postalCode,
            string locality,
            Country country,
            string countryCode,
            string phone,
            string emailAddress,
            string websiteAddress,
            string taxNumber,
            string bankName,
            string facilityName,
            string bic,
            string iban,
            Currency currency,
            string logo,
            string storeName,
            string outgoingShipmentNumberPrefix,
            string salesInvoiceNumberPrefix,
            string salesOrderNumberPrefix,
            string requestNumberPrefix,
            string quoteNumberPrefix,
            string productNumberPrefix,
            int? requestCounterValue,
            int? quoteCounterValue,
            string partNumberPrefix,
            bool useProductNumberCounter,
            bool usePartNumberCounter)
        {
            var postalAddress1 = new PostalAddressBuilder(session)
                    .WithAddress1(address)
                    .WithPostalBoundary(new PostalBoundaryBuilder(session).WithPostalCode(postalCode).WithLocality(locality).WithCountry(country).Build())
                    .Build();

            TelecommunicationsNumber phoneNumber = null;
            if (!string.IsNullOrEmpty(phone))
            {
                phoneNumber = new TelecommunicationsNumberBuilder(session).WithContactNumber(phone).Build();
                if (!string.IsNullOrEmpty(countryCode))
                {
                    phoneNumber.CountryCode = countryCode;
                }
            }

            var email = new EmailAddressBuilder(session)
                .WithElectronicAddressString(emailAddress)
                .Build();

            var webSite = new WebAddressBuilder(session)
                .WithElectronicAddressString(websiteAddress)
                .Build();

            var bank = new BankBuilder(session).WithName(bankName).WithBic(bic).WithCountry(country).Build();
            var bankaccount = new BankAccountBuilder(session)
                .WithBank(bank)
                .WithIban(iban)
                .WithNameOnAccount(name)
                .WithCurrency(currency)
                .Build();

            var organisation = new OrganisationBuilder(session)
                .WithIsInternalOrganisation(true)
                .WithTaxNumber(taxNumber)
                .WithName(name)
                .WithBankAccount(bankaccount)
                .WithDefaultCollectionMethod(new OwnBankAccountBuilder(session).WithBankAccount(bankaccount).WithDescription("Huisbank").Build())
                .WithPreferredCurrency(new Currencies(session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithInvoiceSequence(new InvoiceSequences(session).EnforcedSequence)
                .WithFiscalYearStartMonth(01)
                .WithFiscalYearStartDay(01)
                .WithDoAccounting(false)
                .WithRequestNumberPrefix(requestNumberPrefix)
                .WithQuoteNumberPrefix(quoteNumberPrefix)
                .WithPartNumberPrefix(partNumberPrefix)
                .WithInternetAddress(webSite)
                .WithUseProductNumberCounter(useProductNumberCounter)
                .WithUsePartNumberCounter(usePartNumberCounter)
                .Build();

            if (requestCounterValue != null)
            {
                organisation.RequestCounter = new CounterBuilder(session).WithValue(requestCounterValue).Build();
            }

            if (quoteCounterValue != null)
            {
                organisation.QuoteCounter = new CounterBuilder(session).WithValue(quoteCounterValue).Build();
            }

            organisation.AddPartyContactMechanism(new PartyContactMechanismBuilder(session)
                .WithUseAsDefault(true)
                .WithContactMechanism(email)
                .WithContactPurpose(new ContactMechanismPurposes(session).GeneralEmail)
                .Build());
            organisation.AddPartyContactMechanism(new PartyContactMechanismBuilder(session)
                .WithUseAsDefault(true)
                .WithContactMechanism(postalAddress1)
                .WithContactPurpose(new ContactMechanismPurposes(session).GeneralCorrespondence)
                .Build());

            if (phoneNumber != null)
            {
                organisation.AddPartyContactMechanism(new PartyContactMechanismBuilder(session)
                    .WithUseAsDefault(true)
                    .WithContactMechanism(phoneNumber)
                    .WithContactPurpose(new ContactMechanismPurposes(session).SalesOffice)
                    .Build());
            }

            if (File.Exists(logo))
            {
                var fileInfo = new FileInfo(logo);

                var fileName = System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName).ToLowerInvariant();
                var content = File.ReadAllBytes(fileInfo.FullName);
                var image = new MediaBuilder(session).WithFileName(fileName).WithInData(content).Build();
                organisation.LogoImage = image;
            }

            var magazijn = new FacilityBuilder(session)
                .WithName(facilityName)
                .WithFacilityType(new FacilityTypes(session).Warehouse)
                .WithOwner(organisation)
                .Build();

            organisation.DefaultFacility = magazijn;

            var paymentMethod = new OwnBankAccountBuilder(session).WithBankAccount(bankaccount).WithDescription("Hoofdbank").Build();

            new StoreBuilder(session)
                .WithName(storeName)
                .WithOutgoingShipmentNumberPrefix(outgoingShipmentNumberPrefix)
                .WithSalesInvoiceNumberPrefix(salesInvoiceNumberPrefix)
                .WithSalesOrderNumberPrefix(salesOrderNumberPrefix)
                .WithDefaultCollectionMethod(paymentMethod)
                .WithDefaultShipmentMethod(new ShipmentMethods(session).Ground)
                .WithDefaultCarrier(new Carriers(session).Fedex)
                .WithBillingProcess(new BillingProcesses(session).BillingForShipmentItems)
                .WithSalesInvoiceCounter(new CounterBuilder(session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build())
                .WithIsImmediatelyPicked(true)
                .WithInternalOrganisation(organisation)
                .Build();

            return organisation;
        }
    }
}