// <copyright file="OrganisationContactRelationship.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Allors.Meta;

namespace Allors.Domain
{
    using System;

    public partial class OrganisationContactRelationship
    {
        public void BaseOnPreDerive(ObjectOnPreDerive method)
        {
            var derivation = method.Derivation;

            derivation.AddDependency(this.Organisation, this);
            derivation.AddDependency(this.Contact, this);
        }

        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            this.Parties = new Party[] { this.Contact, this.Organisation };
        }
    }
}
