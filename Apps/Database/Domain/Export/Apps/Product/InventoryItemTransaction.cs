// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InventoryItemTransaction.cs" company="Allors bvba">
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
    using System.Linq;

    public partial class InventoryItemTransaction
    {
        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistTransactionDate)
            {
                this.TransactionDate = this.strategy.Session.Now();
            }
        }

        public void AppsOnPreDerive(ObjectOnPreDerive method)
        {
            var derivation = method.Derivation;

            derivation.AddDependency(this.InventoryItem, this);
        }

        public void AppsOnInit(ObjectOnInit method)
        {
            if (!this.ExistPart)
            {
                this.Part = this.SerialisedItem?.PartWhereSerialisedItem;
            }

            if (!this.ExistFacility)
            {
                this.Facility = this.Part?.DefaultFacility;
            }

            if (!this.ExistUnitOfMeasure)
            {
                this.UnitOfMeasure = this.Part?.UnitOfMeasure;
            }

            if (this.ExistPart && this.Part.InventoryItemKind.Serialised)
            {
                if (!this.ExistSerialisedInventoryItemState)
                {
                    this.SerialisedInventoryItemState = this.Reason.DefaultSerialisedInventoryItemState;
                }
            }
            else
            {
                if (!this.ExistNonSerialisedInventoryItemState)
                {
                    this.NonSerialisedInventoryItemState = this.Reason.DefaultNonSerialisedInventoryItemState;
                }
            }

            // Match on required properties
            bool matched = false;
            var matchingItems = this.Part.InventoryItemsWherePart.ToArray();
            bool possibleMatches = matchingItems.Length > 0;

            if (possibleMatches)
            {
                matchingItems = matchingItems.Where(i => i.Facility.Equals(this.Facility)).ToArray();
                possibleMatches = (matchingItems != null) && (matchingItems.Length > 0);
            }

            if (possibleMatches)
            {
                matchingItems = matchingItems.Where(m => m.UnitOfMeasure.Equals(this.UnitOfMeasure)).ToArray();
                possibleMatches = matchingItems.Length > 0;
            }

            // Match on optional properties
            if (possibleMatches && this.ExistLot)
            {
                matchingItems = matchingItems.Where(m => m.Lot.Equals(this.Lot)).ToArray();
                possibleMatches = matchingItems.Length > 0;
            }

            if (possibleMatches && this.ExistSerialisedInventoryItemState)
            {
                if (matchingItems is SerialisedInventoryItem[] matchingSerialisedItems)
                {
                    matchingItems = matchingSerialisedItems.Where(m => m.InventoryItemState.Equals(this.SerialisedInventoryItemState)).ToArray();
                    possibleMatches = matchingItems.Length > 0;
                }
            }

            if (possibleMatches && this.ExistNonSerialisedInventoryItemState)
            {
                if (matchingItems is NonSerialisedInventoryItem[] matchingNonSerialisedItems)
                {
                    matchingItems = matchingNonSerialisedItems.Where(m => m.InventoryItemState.Equals(this.NonSerialisedInventoryItemState)).ToArray();
                    possibleMatches = matchingItems.Length > 0;
                }
            }

            if (possibleMatches)
            {
                // Match on Non/SerialisedInventoryItemState
                foreach (InventoryItem item in matchingItems)
                {
                    if (item is NonSerialisedInventoryItem nonSerialItem)
                    {
                        if (nonSerialItem.NonSerialisedInventoryItemState.Equals(this.NonSerialisedInventoryItemState))
                        {
                            this.InventoryItem = item;
                            matched = true;
                            break;
                        }
                    }
                    else if (item is SerialisedInventoryItem serialItem)
                    {
                        if (serialItem.SerialisedItem.Equals(this.SerialisedItem))
                        {
                            this.InventoryItem = item;
                            matched = true;
                            break;
                        }
                    }
                }
            }

            if (matched)
            {
                if (this.InventoryItem is SerialisedInventoryItem serialItem)
                {
                    serialItem.SerialisedInventoryItemState = this.SerialisedInventoryItemState;
                }

                if (this.InventoryItem is NonSerialisedInventoryItem nonSerialItem)
                {
                    nonSerialItem.NonSerialisedInventoryItemState = this.NonSerialisedInventoryItemState;
                }
            }
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            if (this.Part.InventoryItemKind.Serialised)
            {
                if (this.Quantity != 1 && this.Quantity != -1 && this.Quantity != 0)
                {
                    var message = "Serialised Inventory Items only accept Quantities of -1, 0, and 1.";
                    derivation.Validation.AddError(this, this.Meta.Quantity, message);
                }

                if (!this.ExistSerialisedItem)
                {
                    var message = "The Serial Number is required for Inventory Item Transactions involving Serialised Inventory Items.";
                    derivation.Validation.AddError(this, this.Meta.SerialisedItem, message);
                }

                if (this.Reason.IncreasesQuantityOnHand == true && this.Quantity <= 0)
                {
                    var message = "Invalid transaction";
                    derivation.Validation.AddError(this, this.Meta.Reason, message);
                }

                if (this.Reason.IncreasesQuantityOnHand == false && this.Quantity <= 0)
                {
                    var message = "Invalid transaction";
                    derivation.Validation.AddError(this, this.Meta.Reason, message);
                }

                if (this.Quantity == 1 
                    && this.SerialisedItem.ExistSerialisedInventoryItemsWhereSerialisedItem 
                    && this.SerialisedItem.SerialisedInventoryItemsWhereSerialisedItem.Any(v => v.Quantity == 1)
                    && this.Reason.IncreasesQuantityOnHand == true)
                {
                    var message = "Serialised item already in inventory";
                    derivation.Validation.AddError(this, this.Meta.SerialisedItem, message);
                }
            }

            if (!this.ExistInventoryItem)
            {
                this.SyncInventoryItem(derivation);
            }
        }

        private void SyncInventoryItem(IDerivation derivation)
        {
            var facility = this.Facility ?? this.Part.DefaultFacility;
            var unitOfMeasure = this.UnitOfMeasure ?? this.Part.UnitOfMeasure;

            if (this.Part.InventoryItemKind.Serialised && this.Quantity <= 0)
            {
                var message = "Invalid transaction";
                derivation.Validation.AddError(this, this.Meta.SerialisedItem, message);
            }

            if (this.Part.InventoryItemKind.Serialised && this.Quantity > 0)
            {
                var builder = new SerialisedInventoryItemBuilder(this.Strategy.Session)
                    .WithFacility(facility)
                    .WithUnitOfMeasure(unitOfMeasure)
                    .WithSerialisedItem(this.SerialisedItem)
                    .WithPart(this.Part)
                    .WithSerialisedInventoryItemState(this.SerialisedInventoryItemState);

                if (this.ExistLot)
                {
                    builder.WithLot(this.Lot);
                }

                this.InventoryItem = builder.Build();
            }
            else if (this.Part.InventoryItemKind.NonSerialised)
            {
                var builder = new NonSerialisedInventoryItemBuilder(this.Strategy.Session)
                    .WithFacility(facility)
                    .WithUnitOfMeasure(unitOfMeasure)
                    .WithPart(this.Part)
                    .WithNonSerialisedInventoryItemState(this.NonSerialisedInventoryItemState);

                if (this.ExistLot)
                {
                    builder.WithLot(this.Lot);
                }

                this.InventoryItem = builder.Build();
            }
        }
    }
}