// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurchaseOrderItemStates.cs" company="Allors bvba">
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

    public partial class PurchaseOrderItemStates
    {
        public static readonly Guid CreatedId = new Guid("57273ADE-A813-40ba-B319-EF8D62AC92B6");
        public static readonly Guid AwaitingApprovalId = new Guid("BB3F365A-BC0D-44ff-9682-0D9FF910C637");
        public static readonly Guid CancelledId = new Guid("7342A3E6-69E4-49a7-9C2E-93574BF14072");
        public static readonly Guid CancelledByOrderId = new Guid("23F051B3-1A9A-4B0A-B6A5-24CB8EBE6248");
        public static readonly Guid CompletedId = new Guid("9B338149-43EA-4091-BBD8-C3485337FBC5");
        public static readonly Guid RejectedId = new Guid("0CD96679-4699-42de-9AB6-C4DA197F907D");
        public static readonly Guid OnHoldId = new Guid("BEB5870C-0542-42fa-B2FC-5D2BD21673B7");
        public static readonly Guid InProcessId = new Guid("9CD110AE-7787-469f-9A3E-F0000E35E588");
        public static readonly Guid FinishedId = new Guid("4166228F-0ECC-444b-A45E-43794184DBB9");

        private UniquelyIdentifiableSticky<PurchaseOrderItemState> stateCache;

        public PurchaseOrderItemState Created => this.StateCache[CreatedId];

        public PurchaseOrderItemState AwaitingApproval => this.StateCache[AwaitingApprovalId];

        public PurchaseOrderItemState Cancelled => this.StateCache[CancelledId];

        public PurchaseOrderItemState CancelledByOrder => this.StateCache[CancelledByOrderId];

        public PurchaseOrderItemState Completed => this.StateCache[CompletedId];

        public PurchaseOrderItemState Rejected => this.StateCache[RejectedId];

        public PurchaseOrderItemState Finished => this.StateCache[FinishedId];

        public PurchaseOrderItemState OnHold => this.StateCache[OnHoldId];

        public PurchaseOrderItemState InProcess => this.StateCache[InProcessId];

        private UniquelyIdentifiableSticky<PurchaseOrderItemState> StateCache => this.stateCache ?? (this.stateCache = new UniquelyIdentifiableSticky<PurchaseOrderItemState>(this.Session));

        protected override void AppsSetup(Setup setup)
        {
            base.AppsSetup(setup);

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(CreatedId)
                .WithName("Created")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(AwaitingApprovalId)
                .WithName("Awaiting Approval")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(CancelledId)
                .WithName("Cancelled")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(CancelledByOrderId)
                .WithName("Cancelled")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(CompletedId)
                .WithName("Completed")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(RejectedId)
                .WithName("Rejected")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(OnHoldId)
                .WithName("On Hold")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(InProcessId)
                .WithName("In Process")
                .Build();

            new PurchaseOrderItemStateBuilder(this.Session)
                .WithUniqueId(FinishedId)
                .WithName("Finished")
                .Build();
        }
    }
}