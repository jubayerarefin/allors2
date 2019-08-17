// <copyright file="LocalTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Local
{
    using System;

    using Allors.Workspace;
    using Allors.Workspace.Domain;
    using Allors.Workspace.Meta;

    public class LocalTest : IDisposable
    {
        public Workspace Workspace { get; set; }

        public LocalTest()
        {
            var objectFactory = new ObjectFactory(MetaPopulation.Instance, typeof(User));
            this.Workspace = new Workspace(objectFactory);
        }

        public void Dispose()
        {
        }
    }
}
