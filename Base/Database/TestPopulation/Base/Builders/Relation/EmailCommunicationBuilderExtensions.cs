// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailCommunicationBuilderExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Allors.Domain.TestPopulation
{
    using System;

    public static partial class EmailCommunicationBuilderExtensions
    {
        public static EmailCommunicationBuilder WithDefaults(this EmailCommunicationBuilder @this, Organisation internalOrganisation)
        {
            var faker = @this.Session.Faker();

            var administrator = (Person)new UserGroups(@this.Session).Administrators.Members.First;

            @this.WithDescription(faker.Lorem.Sentence(20));
            @this.WithSubject(faker.Lorem.Sentence(5));
            @this.WithFromParty(internalOrganisation.ActiveEmployees.First);
            @this.WithToParty(internalOrganisation.ActiveCustomers.First);
            @this.WithFromEmail(internalOrganisation.ActiveEmployees.First.GeneralEmail);
            @this.WithToEmail(internalOrganisation.ActiveCustomers.First.GeneralEmail);
            @this.WithEventPurpose(new CommunicationEventPurposes(@this.Session).Meeting);
            @this.WithOwner(administrator);
            @this.WithActualStart(DateTime.UtcNow);

            return @this;
        }
    }
}
