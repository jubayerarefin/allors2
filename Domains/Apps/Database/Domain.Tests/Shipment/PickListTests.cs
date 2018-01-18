//------------------------------------------------------------------------------------------------- 
// <copyright file="PickListTests.cs" company="Allors bvba">
// Copyright 2002-2009 Allors bvba.
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
// <summary>Defines the MediaTests type.</summary>
//-------------------------------------------------------------------------------------------------

using System.Security.Principal;
using System.Threading;

namespace Allors.Domain
{
    using System;
    using Meta;
    using Xunit;

    
    public class PickListTests : DomainTest
    {
        [Fact]
        public void GivenPickListBuilder_WhenBuild_ThenPostBuildRelationsMustExist()
        {
            var pickList = new PickListBuilder(this.Session).Build();

            this.Session.Derive();

            Assert.Equal(new PickListStates(this.Session).Created, pickList.PickListState);
        }

        [Fact]
        public void GivenPickList_WhenObjectStateIsCreated_ThenCheckTransitions()
        {
            this.SetIdentity("orderProcessor");

            var pickList = new PickListBuilder(this.Session).Build();

            this.Session.Derive();

            var acl = new AccessControlList(pickList, this.Session.GetUser());
            Assert.True(acl.CanExecute(M.PickList.Cancel));
        }

        [Fact]
        public void GivenPickList_WhenObjectStateIsCancelled_ThenCheckTransitions()
        {
            this.SetIdentity("orderProcessor");

            var pickList = new PickListBuilder(this.Session).Build();

            this.Session.Derive();

            pickList.Cancel();

            this.Session.Derive();

            var acl = new AccessControlList(pickList, this.Session.GetUser());
            Assert.False(acl.CanExecute(M.PickList.Cancel));
            Assert.False(acl.CanExecute(M.PickList.SetPicked));
        }

        [Fact]
        public void GivenPickList_WhenObjectStateIsPicked_ThenCheckTransitions()
        {
            this.SetIdentity("orderProcessor");

            var pickList = new PickListBuilder(this.Session).Build();

            this.Session.Derive();

            pickList.SetPicked();

            this.Session.Derive();

            var acl = new AccessControlList(pickList, this.Session.GetUser());
            Assert.False(acl.CanExecute(M.PickList.Cancel));
            Assert.False(acl.CanExecute(M.PickList.SetPicked));
        }

        [Fact]
        public void GivenPickList_WhenPicked_ThenInventoryIsAdjustedAndOrderItemsQuantityPickedIsSet()
        {
            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            var mechelenAddress = new PostalAddressBuilder(this.Session).WithGeographicBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var shipToMechelen = new PartyContactMechanismBuilder(this.Session)
                .WithContactMechanism(mechelenAddress)
                .WithContactPurpose(new ContactMechanismPurposes(this.Session).ShippingAddress)
                .WithUseAsDefault(true)
                .Build();

            var supplier = new OrganisationBuilder(this.Session).WithName("supplier").WithOrganisationRole(new OrganisationRoles(this.Session).Supplier).Build();
            var customer = new PersonBuilder(this.Session).WithLastName("person1").WithPartyContactMechanism(shipToMechelen).WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var internalOrganisation = this.Session.GetSingleton().InternalOrganisation;

            new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer).Build();

            new SupplierRelationshipBuilder(this.Session)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            var vatRate21 = new VatRateBuilder(this.Session).WithRate(21).Build();

            var good1 = new GoodBuilder(this.Session)
                .WithSku("10101")
                .WithVatRate(vatRate21)
                .WithName("good1")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2 = new GoodBuilder(this.Session)
                .WithSku("10102")
                .WithVatRate(vatRate21)
                .WithName("good2")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good1PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good1)
                .WithProductPurchasePrice(good1PurchasePrice)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good2)
                .WithProductPurchasePrice(good2PurchasePrice)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            this.Session.Derive();

            var good1Inventory = (NonSerialisedInventoryItem)good1.InventoryItemsWhereGood[0];
            good1Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Unknown).Build());

            this.Session.Derive();

            var good2Inventory = (NonSerialisedInventoryItem)good2.InventoryItemsWhereGood[0];
            good2Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Unknown).Build());

            this.Session.Derive();

            var colorWhite = new ColourBuilder(this.Session)
                .WithName("white")
                .Build();
            var extraLarge = new SizeBuilder(this.Session)
                .WithName("Extra large")
                .Build();

            var order = new SalesOrderBuilder(this.Session)
                .WithBillToCustomer(customer)
                .WithShipToCustomer(customer)
                .WithVatRegime(new VatRegimes(this.Session).Export)
                .Build();

            var item1 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var item2 = new SalesOrderItemBuilder(this.Session).WithItemType(new SalesInvoiceItemTypes(this.Session).ProductFeatureItem).WithProductFeature(colorWhite).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var item3 = new SalesOrderItemBuilder(this.Session).WithItemType(new SalesInvoiceItemTypes(this.Session).ProductFeatureItem).WithProductFeature(extraLarge).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            item1.AddOrderedWithFeature(item2);
            item1.AddOrderedWithFeature(item3);
            var item4 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(2).WithActualUnitPrice(15).Build();
            var item5 = new SalesOrderItemBuilder(this.Session).WithProduct(good2).WithQuantityOrdered(5).WithActualUnitPrice(15).Build();
            order.AddSalesOrderItem(item1);
            order.AddSalesOrderItem(item2);
            order.AddSalesOrderItem(item3);
            order.AddSalesOrderItem(item4);
            order.AddSalesOrderItem(item5);

            this.Session.Derive();

            order.Confirm();

            this.Session.Derive();

            var shipment = (CustomerShipment)mechelenAddress.ShipmentsWhereShipToAddress[0];

            var pickList = good1.InventoryItemsWhereGood[0].PickListItemsWhereInventoryItem[0].PickListWherePickListItem;
            pickList.Picker = new People(this.Session).FindBy(M.Person.LastName, "orderProcessor");

            //// item5: only 4 out of 5 are actually picked
            foreach (PickListItem pickListItem in pickList.PickListItems)
            {
                if (pickListItem.RequestedQuantity == 5)
                {
                    pickListItem.ActualQuantity = 4;
                }
            }

            pickList.SetPicked();

            this.Session.Derive();

            //// all orderitems have same physical finished good, so there is only one picklist item.
            Assert.Equal(1, item1.QuantityPicked);
            Assert.Equal(0, item1.QuantityReserved);
            Assert.Equal(0, item1.QuantityRequestsShipping);
            Assert.Equal(2, item4.QuantityPicked);
            Assert.Equal(0, item4.QuantityReserved);
            Assert.Equal(0, item4.QuantityRequestsShipping);
            Assert.Equal(4, item5.QuantityPicked);
            Assert.Equal(1, item5.QuantityReserved);
            Assert.Equal(0, item5.QuantityRequestsShipping);
            Assert.Equal(97, good1Inventory.QuantityOnHand);
            Assert.Equal(0, good1Inventory.QuantityCommittedOut);
            Assert.Equal(96, good2Inventory.QuantityOnHand);
            Assert.Equal(1, good2Inventory.QuantityCommittedOut);
        }

        [Fact]
        public void GivenPickList_WhenActualQuantityPickedIsLess_ThenShipmentItemQuantityIsAdjusted()
        {
            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            var mechelenAddress = new PostalAddressBuilder(this.Session).WithGeographicBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var shipToMechelen = new PartyContactMechanismBuilder(this.Session)
                .WithContactMechanism(mechelenAddress)
                .WithContactPurpose(new ContactMechanismPurposes(this.Session).ShippingAddress)
                .WithUseAsDefault(true)
                .Build();

            var supplier = new OrganisationBuilder(this.Session).WithName("supplier").WithOrganisationRole(new OrganisationRoles(this.Session).Supplier).Build();
            var customer = new PersonBuilder(this.Session).WithLastName("person1").WithPartyContactMechanism(shipToMechelen).WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var internalOrganisation = this.Session.GetSingleton().InternalOrganisation;

            new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer).Build();

            new SupplierRelationshipBuilder(this.Session)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            var vatRate21 = new VatRateBuilder(this.Session).WithRate(21).Build();

            var good1 = new GoodBuilder(this.Session)
                .WithSku("10101")
                .WithVatRate(vatRate21)
                .WithName("good1")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2 = new GoodBuilder(this.Session)
                .WithSku("10102")
                .WithVatRate(vatRate21)
                .WithName("good2")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good1PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good1)
                .WithProductPurchasePrice(good1PurchasePrice)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good2)
                .WithProductPurchasePrice(good2PurchasePrice)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            this.Session.Derive();

            var good1Inventory = (NonSerialisedInventoryItem)good1.InventoryItemsWhereGood[0];
            good1Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Unknown).Build());

            this.Session.Derive();

            var good2Inventory = (NonSerialisedInventoryItem)good2.InventoryItemsWhereGood[0];
            good2Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Unknown).Build());

            this.Session.Derive();

            var order = new SalesOrderBuilder(this.Session)
                .WithBillToCustomer(customer)
                .WithShipToCustomer(customer)
                .Build();

            var item1 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var item2 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(2).WithActualUnitPrice(15).Build();
            var item3 = new SalesOrderItemBuilder(this.Session).WithProduct(good2).WithQuantityOrdered(5).WithActualUnitPrice(15).Build();
            order.AddSalesOrderItem(item1);
            order.AddSalesOrderItem(item2);
            order.AddSalesOrderItem(item3);

            this.Session.Derive();

            order.Confirm();

            this.Session.Derive();

            var pickList = good1.InventoryItemsWhereGood[0].PickListItemsWhereInventoryItem[0].PickListWherePickListItem;
            pickList.Picker = new People(this.Session).FindBy(M.Person.LastName, "orderProcessor");

            //// item3: only 4 out of 5 are actually picked
            PickListItem adjustedPicklistItem = null;
            foreach (PickListItem pickListItem in pickList.PickListItems)
            {
                if (pickListItem.RequestedQuantity == 5)
                {
                    adjustedPicklistItem = pickListItem;
                }
            }

            var itemIssuance = adjustedPicklistItem.ItemIssuancesWherePickListItem[0];
            var shipmentItem = adjustedPicklistItem.ItemIssuancesWherePickListItem[0].ShipmentItem;

            Assert.Equal(5, itemIssuance.Quantity);
            Assert.Equal(5, shipmentItem.Quantity);

            adjustedPicklistItem.ActualQuantity = 4;

            pickList.SetPicked();

            this.Session.Derive();

            Assert.Equal(4, itemIssuance.Quantity);
            Assert.Equal(4, shipmentItem.Quantity);
        }

        [Fact]
        public void GivenSalesOrder_WhenShipmentIsCreated_ThenOrdertemsAreAddedToPickList()
        {
            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            var mechelenAddress = new PostalAddressBuilder(this.Session).WithGeographicBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var shipToMechelen = new PartyContactMechanismBuilder(this.Session)
                .WithContactMechanism(mechelenAddress)
                .WithContactPurpose(new ContactMechanismPurposes(this.Session).ShippingAddress)
                .WithUseAsDefault(true)
                .Build();

            var supplier = new OrganisationBuilder(this.Session).WithName("supplier").WithOrganisationRole(new OrganisationRoles(this.Session).Supplier).Build();

            var customer = new PersonBuilder(this.Session).WithLastName("person1").WithPartyContactMechanism(shipToMechelen).WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var internalOrganisation = this.Session.GetSingleton().InternalOrganisation;

            new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer).Build();

            new SupplierRelationshipBuilder(this.Session)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            var vatRate21 = new VatRateBuilder(this.Session).WithRate(21).Build();

            var good1 = new GoodBuilder(this.Session)
                .WithSku("10101")
                .WithVatRate(vatRate21)
                .WithName("good1")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2 = new GoodBuilder(this.Session)
                .WithSku("10102")
                .WithVatRate(vatRate21)
                .WithName("good2")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good1PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good1)
                .WithProductPurchasePrice(good1PurchasePrice)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good2)
                .WithProductPurchasePrice(good2PurchasePrice)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            this.Session.Derive();

            var good1Inventory = (NonSerialisedInventoryItem)good1.InventoryItemsWhereGood[0];
            good1Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Ruined).Build());

            this.Session.Derive();

            var good2Inventory = (NonSerialisedInventoryItem)good2.InventoryItemsWhereGood[0];
            good2Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Ruined).Build());

            this.Session.Derive();

            var order = new SalesOrderBuilder(this.Session)
                .WithBillToCustomer(customer)
                .WithShipToCustomer(customer)
                .Build();

            var item1 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var item2 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(2).WithActualUnitPrice(15).Build();
            var item3 = new SalesOrderItemBuilder(this.Session).WithProduct(good2).WithQuantityOrdered(5).WithActualUnitPrice(15).Build();
            order.AddSalesOrderItem(item1);
            order.AddSalesOrderItem(item2);
            order.AddSalesOrderItem(item3);

            this.Session.Derive();

            order.Confirm();

            this.Session.Derive();

            var pickList = good1.InventoryItemsWhereGood[0].PickListItemsWhereInventoryItem[0].PickListWherePickListItem;

            Assert.Equal(2, pickList.PickListItems.Count);

            var extent1 = pickList.PickListItems;
            extent1.Filter.AddEquals(M.PickListItem.InventoryItem, good1Inventory);
            Assert.Equal(3, extent1.First.RequestedQuantity);

            var extent2 = pickList.PickListItems;
            extent2.Filter.AddEquals(M.PickListItem.InventoryItem, good2Inventory);
            Assert.Equal(5, extent2.First.RequestedQuantity);
        }

        [Fact]
        public void GivenMultipleOrders_WhenCombinedPickListIsPicked_ThenSingleShipmentIsPickedState()
        {
            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            var mechelenAddress = new PostalAddressBuilder(this.Session).WithGeographicBoundary(mechelen).WithAddress1("Haverwerf").Build();
            var shipToMechelen = new PartyContactMechanismBuilder(this.Session)
                .WithContactMechanism(mechelenAddress)
                .WithContactPurpose(new ContactMechanismPurposes(this.Session).ShippingAddress)
                .WithUseAsDefault(true)
                .Build();

            var supplier = new OrganisationBuilder(this.Session).WithName("supplier").WithOrganisationRole(new OrganisationRoles(this.Session).Supplier).Build();
            var customer = new PersonBuilder(this.Session).WithLastName("person1").WithPartyContactMechanism(shipToMechelen).WithPersonRole(new PersonRoles(this.Session).Customer).Build();
            var internalOrganisation = this.Session.GetSingleton().InternalOrganisation;
            new CustomerRelationshipBuilder(this.Session).WithFromDate(DateTime.UtcNow).WithCustomer(customer).Build();

            new SupplierRelationshipBuilder(this.Session)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            var vatRate21 = new VatRateBuilder(this.Session).WithRate(21).Build();

            var good1 = new GoodBuilder(this.Session)
                .WithSku("10101")
                .WithVatRate(vatRate21)
                .WithName("good1")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2 = new GoodBuilder(this.Session)
                .WithSku("10102")
                .WithVatRate(vatRate21)
                .WithName("good2")
                .WithInventoryItemKind(new InventoryItemKinds(this.Session).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good1PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            var good2PurchasePrice = new ProductPurchasePriceBuilder(this.Session)
                .WithCurrency(new Currencies(this.Session).FindBy(M.Currency.IsoCode, "EUR"))
                .WithFromDate(DateTime.UtcNow)
                .WithPrice(7)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.Session).Piece)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good1)
                .WithProductPurchasePrice(good1PurchasePrice)
                .WithFromDate(DateTime.UtcNow)
                .WithSupplier(supplier)
                .Build();

            new SupplierOfferingBuilder(this.Session)
                .WithProduct(good2)
                .WithProductPurchasePrice(good2PurchasePrice)
                .WithSupplier(supplier)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            this.Session.Derive();

            var good1Inventory = (NonSerialisedInventoryItem)good1.InventoryItemsWhereGood[0];
            good1Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Ruined).Build());

            this.Session.Derive();

            var good2Inventory = (NonSerialisedInventoryItem)good2.InventoryItemsWhereGood[0];
            good2Inventory.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.Session).WithQuantity(100).WithReason(new VarianceReasons(this.Session).Ruined).Build());

            this.Session.Derive();

            var order1 = new SalesOrderBuilder(this.Session)
                .WithBillToCustomer(customer)
                .WithShipToCustomer(customer)
                .Build();

            var item1 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var item2 = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(2).WithActualUnitPrice(15).Build();
            var item3 = new SalesOrderItemBuilder(this.Session).WithProduct(good2).WithQuantityOrdered(5).WithActualUnitPrice(15).Build();
            order1.AddSalesOrderItem(item1);
            order1.AddSalesOrderItem(item2);
            order1.AddSalesOrderItem(item3);

            this.Session.Derive();

            order1.Confirm();

            this.Session.Derive();

            var order2 = new SalesOrderBuilder(this.Session)
                .WithBillToCustomer(customer)
                .WithShipToCustomer(customer)
                .Build();

            var itema = new SalesOrderItemBuilder(this.Session).WithProduct(good1).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var itemb = new SalesOrderItemBuilder(this.Session).WithProduct(good2).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            order2.AddSalesOrderItem(itema);
            order2.AddSalesOrderItem(itemb);

            this.Session.Derive();

            order2.Confirm();

            this.Session.Derive();

            var pickList = good1.InventoryItemsWhereGood[0].PickListItemsWhereInventoryItem[0].PickListWherePickListItem;
            pickList.Picker = new People(this.Session).FindBy(M.Person.LastName, "orderProcessor");

            pickList.SetPicked();

            this.Session.Derive();

            Assert.Equal(1, customer.ShipmentsWhereBillToParty.Count);

            var customerShipment = (CustomerShipment)customer.ShipmentsWhereBillToParty.First;
            Assert.Equal(new CustomerShipmentStates(this.Session).Picked, customerShipment.CustomerShipmentState);
        }
    }
}