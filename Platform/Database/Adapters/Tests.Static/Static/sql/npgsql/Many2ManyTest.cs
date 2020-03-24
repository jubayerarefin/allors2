// <copyright file="Many2ManyTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the Default type.
// </summary>

namespace Allors.Database.Adapters.Npgsql
{
    using Xunit;
    using System;
    using Adapters;

    [Collection(Fixture.Collection)]
    public class Many2ManyTest : Adapters.Many2ManyTest, IDisposable
    {
        private readonly Profile profile;

        public Many2ManyTest(Fixture fixture) => this.profile = new Profile(fixture.PgServer);

        protected override IProfile Profile => this.profile;

        public override void Dispose() => this.profile.Dispose();
    }
}
