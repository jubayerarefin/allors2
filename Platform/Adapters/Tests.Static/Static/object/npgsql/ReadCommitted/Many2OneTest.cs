// <copyright file="Many2OneTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Xunit;

namespace Allors.Adapters.Object.Npgsql.ReadCommitted
{
    using System;

    [Collection(Fixture.Collection)]
    public class Many2OneTest : Adapters.Many2OneTest, IDisposable
    {
        private readonly Profile profile;

        public Many2OneTest(Fixture fixture) => this.profile = new Profile(fixture.Server);

        protected override IProfile Profile => this.profile;

        public override void Dispose() => this.profile.Dispose();
    }
}
