// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomerRelationshipTests.cs" company="Allors bvba">
//   Copyright 2002-2009 Allors bvba.
// 
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// 
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
// <summary>
//   Defines the PersonTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Allors.Domain
{
    using System;
    using Xunit;

    public class CustomerRelationshipTests : DomainTest
    {
        [Fact]
        public void GivenCustomerRelationship_WhenDerivingWithout_ThenAmountDueIsZero()
        {
            var customer = new PersonBuilder(this.Session).WithLastName("customer").WithPersonRole(new PersonRoles(this.Session).Customer).Build();


            var customerRelationship = new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer).Build();


            this.Session.Derive();

            Assert.Equal(0, customer.AmountDue);
        }

        [Fact]
        public void GivenCustomerRelationship_WhenDerivingWithout_ThenAmountOverDueIsZero()
        {
            var customer = new PersonBuilder(this.Session).WithLastName("customer").WithPersonRole(new PersonRoles(this.Session).Customer).Build();

            var customerRelationship = new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer).Build();

            this.Session.Derive();

            Assert.Equal(0, customer.AmountOverDue);
        }

        [Fact]
        public void GivenCustomerRelationshipToCome_WhenDeriving_ThenInternalOrganisationCustomersDosNotContainCustomer()
        {
            var customer = new PersonBuilder(this.Session).WithLastName("customer").WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var internalOrganisation = this.Session.GetSingleton().InternalOrganisation;

            new CustomerRelationshipBuilder(this.Session)
                .WithCustomer(customer)
                
                .WithFromDate(DateTime.UtcNow.AddDays(1))
                .Build();

            this.Session.Derive();

            Assert.False(internalOrganisation.CurrentCustomers.Contains(customer));
        }

        [Fact]
        public void GivenCustomerRelationshipThatHasEnded_WhenDeriving_ThenInternalOrganisationCustomersDosNotContainCustomer()
        {
            var customer = new PersonBuilder(this.Session).WithLastName("customer").WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var internalOrganisation = this.Session.GetSingleton().InternalOrganisation;

            new CustomerRelationshipBuilder(this.Session)
                .WithCustomer(customer)
                
                .WithFromDate(DateTime.UtcNow.AddDays(-10))
                .WithThroughDate(DateTime.UtcNow.AddDays(-1))
                .Build();

            this.Session.Derive();

            Assert.False(internalOrganisation.CurrentCustomers.Contains(customer));
        }

        [Fact]
        public void GivenCustomerRelationshipBuilder_WhenBuild_ThenSubAccountNumerIsValidElevenTestNumber()
        {
            var internalOrganisation = this.Session.GetSingleton().InternalOrganisation;
            internalOrganisation.SubAccountCounter.Value = 1000;

            this.Session.Commit();

            var customer1 = new PersonBuilder(this.Session).WithLastName("customer1").WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var customerRelationship1 = new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer1).Build();

            this.Session.Derive();

            Assert.Equal(1007, customer1.SubAccountNumber);

            var customer2 = new PersonBuilder(this.Session).WithLastName("customer2").WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var customerRelationship2 = new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer2).Build();

            this.Session.Derive();

            Assert.Equal(1015, customer2.SubAccountNumber);

            var customer3 = new PersonBuilder(this.Session).WithLastName("customer3").WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var customerRelationship3 = new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer3).Build();

            this.Session.Derive();

            Assert.Equal(1023, customer3.SubAccountNumber);
        }

        [Fact]
        public void GivenCustomerRelationship_WhenDeriving_ThenSubAccountNumberMustBeUniqueWithinSingleton()
        {
            var customer2 = new OrganisationBuilder(this.Session).WithName("customer").WithOrganisationRole(new OrganisationRoles(this.Session).Customer).Build();

            var belgium = new Countries(this.Session).CountryByIsoCode["BE"];
            var euro = belgium.Currency;

            var bank = new BankBuilder(this.Session).WithCountry(belgium).WithName("ING Belgi�").WithBic("BBRUBEBB").Build();

            var ownBankAccount = new OwnBankAccountBuilder(this.Session)
                .WithDescription("BE23 3300 6167 6391")
                .WithBankAccount(new BankAccountBuilder(this.Session).WithBank(bank).WithCurrency(euro).WithIban("BE23 3300 6167 6391").WithNameOnAccount("Koen").Build())
                .Build();

            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            var address1 = new PostalAddressBuilder(this.Session).WithGeographicBoundary(mechelen).WithAddress1("Haverwerf 15").Build();

            var internalOrganisation2 = new InternalOrganisationBuilder(this.Session)
                .WithName("internalOrganisation2")
                .WithBillingAddress(address1)
                .WithDefaultPaymentMethod(ownBankAccount)
                .Build();

            var customerRelationship2 = new CustomerRelationshipBuilder(this.Session)
                .WithCustomer(customer2)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            customer2.SubAccountNumber = 19;

            Assert.False(this.Session.Derive(false).HasErrors);
        }

        [Fact]
        public void GivenCustomerWithUnpaidInvoices_WhenDeriving_ThenAmountDueIsUpdated()
        {
            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            var customer = new PersonBuilder(this.Session).WithLastName("customer").WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var customerRelationship = new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer).Build();

            var billToContactMechanism = new PostalAddressBuilder(this.Session).WithGeographicBoundary(mechelen).WithAddress1("Mechelen").Build();

            var good = new GoodBuilder(this.Session)
                .WithSku("10101")
                .WithVatRate(new VatRateBuilder(this.Session).WithRate(0).Build())
                .WithLocalisedName(new LocalisedTextBuilder(this.Session).WithText("good").WithLocale(this.Session.GetSingleton().DefaultLocale).Build())
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            this.Session.Derive();

            var invoice1 = new SalesInvoiceBuilder(this.Session)
                .WithSalesInvoiceType(new SalesInvoiceTypes(this.Session).SalesInvoice)
                .WithBillToCustomer(customer)
                .WithBillToContactMechanism(billToContactMechanism)
                .WithSalesInvoiceItem(new SalesInvoiceItemBuilder(this.Session).WithProduct(good).WithQuantity(1).WithActualUnitPrice(100M).WithSalesInvoiceItemType(new SalesInvoiceItemTypes(this.Session).ProductItem).Build())
                .Build();

            this.Session.Derive();

            var invoice2 = new SalesInvoiceBuilder(this.Session)
                .WithSalesInvoiceType(new SalesInvoiceTypes(this.Session).SalesInvoice)
                .WithBillToCustomer(customer)
                .WithBillToContactMechanism(billToContactMechanism)
                .WithSalesInvoiceItem(new SalesInvoiceItemBuilder(this.Session).WithProduct(good).WithQuantity(1).WithActualUnitPrice(200M).WithSalesInvoiceItemType(new SalesInvoiceItemTypes(this.Session).ProductItem).Build())
                .Build();

            this.Session.Derive();

            Assert.Equal(300M, customer.AmountDue);

            new ReceiptBuilder(this.Session)
                .WithAmount(50)
                .WithPaymentApplication(new PaymentApplicationBuilder(this.Session).WithInvoiceItem(invoice1.SalesInvoiceItems[0]).WithAmountApplied(50).Build())
                .Build();

            this.Session.Derive();

            Assert.Equal(250, customer.AmountDue);

            new ReceiptBuilder(this.Session)
                .WithAmount(200)
                .WithPaymentApplication(new PaymentApplicationBuilder(this.Session).WithInvoiceItem(invoice2.SalesInvoiceItems[0]).WithAmountApplied(200).Build())
                .Build();

            this.Session.Derive();

            Assert.Equal(50, customer.AmountDue);

            new ReceiptBuilder(this.Session)
                .WithAmount(50)
                .WithPaymentApplication(new PaymentApplicationBuilder(this.Session).WithInvoiceItem(invoice1.SalesInvoiceItems[0]).WithAmountApplied(50).Build())
                .Build();

            this.Session.Derive();

            Assert.Equal(0, customer.AmountDue);
        }

        [Fact]
        public void GivenCustomerWithUnpaidInvoices_WhenDeriving_ThenAmountOverDueIsUpdated()
        {
            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            var customer = new PersonBuilder(this.Session).WithLastName("customer").WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow.AddDays(-31)).WithCustomer(customer).Build();

            var billToContactMechanism = new PostalAddressBuilder(this.Session).WithGeographicBoundary(mechelen).WithAddress1("Mechelen").Build();

            var good = new GoodBuilder(this.Session)
                .WithSku("10101")
                .WithVatRate(new VatRateBuilder(this.Session).WithRate(0).Build())
                .WithLocalisedName(new LocalisedTextBuilder(this.Session).WithText("good").WithLocale(this.Session.GetSingleton().DefaultLocale).Build())
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            this.Session.Derive();

            var invoice1 = new SalesInvoiceBuilder(this.Session)
                .WithSalesInvoiceType(new SalesInvoiceTypes(this.Session).SalesInvoice)
                .WithBillToCustomer(customer)
                .WithBillToContactMechanism(billToContactMechanism)
                .WithInvoiceDate(DateTime.UtcNow.AddDays(-30))
                .WithSalesInvoiceItem(new SalesInvoiceItemBuilder(this.Session).WithProduct(good).WithQuantity(1).WithActualUnitPrice(100M).WithSalesInvoiceItemType(new SalesInvoiceItemTypes(this.Session).ProductItem).Build())
                .Build();

            this.Session.Derive();

            var invoice2 = new SalesInvoiceBuilder(this.Session)
                .WithSalesInvoiceType(new SalesInvoiceTypes(this.Session).SalesInvoice)
                .WithBillToCustomer(customer)
                .WithBillToContactMechanism(billToContactMechanism)
                .WithInvoiceDate(DateTime.UtcNow.AddDays(-5))
                .WithSalesInvoiceItem(new SalesInvoiceItemBuilder(this.Session).WithProduct(good).WithQuantity(1).WithActualUnitPrice(200M).WithSalesInvoiceItemType(new SalesInvoiceItemTypes(this.Session).ProductItem).Build())
                .Build();

            this.Session.Derive();

            Assert.Equal(100M, customer.AmountOverDue);

            new ReceiptBuilder(this.Session)
                .WithAmount(20)
                .WithPaymentApplication(new PaymentApplicationBuilder(this.Session).WithInvoiceItem(invoice1.SalesInvoiceItems[0]).WithAmountApplied(20).Build())
                .Build();

            this.Session.Derive();

            Assert.Equal(80, customer.AmountOverDue);

            invoice2.InvoiceDate = DateTime.UtcNow.AddDays(-10);

            this.Session.Derive();

            Assert.Equal(280, customer.AmountOverDue);
        }
    }
}
