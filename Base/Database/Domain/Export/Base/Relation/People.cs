// <copyright file="People.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using Meta;

    public partial class People
    {
        protected override void BaseSetup(Setup setup)
        {
            var internalOrganisations = new Organisations(this.Session).InternalOrganisations();

            var users = new Users(this.Session).Extent();

            foreach (Person person in users)
            {
                foreach (var internalOrganisation in internalOrganisations)
                {
                    new EmploymentBuilder(this.Session).WithEmployer(internalOrganisation).WithEmployee(person).Build();
                }
            }
        }

        protected override void BasePrepare(Setup setup)
        {
            setup.AddDependency(this.Meta.ObjectType, M.Role);
            setup.AddDependency(this.Meta.ObjectType, M.PersonRole);
            setup.AddDependency(this.Meta.ObjectType, M.InternalOrganisation);
            setup.AddDependency(this.ObjectType, M.Locale.ObjectType);
            setup.AddDependency(this.ObjectType, M.ContactMechanismPurpose.ObjectType);
            setup.AddDependency(this.ObjectType, M.InternalOrganisation.ObjectType);
        }

        protected override void BaseSecure(Security config)
        {
            var full = new[] { Operations.Read, Operations.Write, Operations.Execute };

            config.GrantOwner(this.ObjectType, full);
        }
    }
}
