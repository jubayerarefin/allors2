// <copyright file="ProductQuotes.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using Meta;

    public partial class ProductQuotes
    {
        protected override void BasePrepare(Setup setup)
        {
            base.BasePrepare(setup);

            setup.AddDependency(this.ObjectType, M.QuoteState);
        }

        protected override void BaseSecure(Security config)
        {
            var created = new QuoteStates(this.Session).Created;
            var approved = new QuoteStates(this.Session).Approved;
            var ordered = new QuoteStates(this.Session).Ordered;
            var rejected = new QuoteStates(this.Session).Rejected;
            var cancelled = new QuoteStates(this.Session).Cancelled;

            var approve = this.Meta.Approve;
            var reject = this.Meta.Reject;
            var order = this.Meta.Order;
            var cancel = this.Meta.Cancel;

            config.Deny(this.ObjectType, created, order);
            config.Deny(this.ObjectType, ordered, approve, reject, order, cancel);
            config.Deny(this.ObjectType, rejected, approve, reject, order);
            config.Deny(this.ObjectType, cancelled, cancel, reject, order);

            config.Deny(this.ObjectType, rejected, Operations.Write);
            config.Deny(this.ObjectType, ordered, Operations.Write);
            config.Deny(this.ObjectType, cancelled, Operations.Write);
        }
    }
}
