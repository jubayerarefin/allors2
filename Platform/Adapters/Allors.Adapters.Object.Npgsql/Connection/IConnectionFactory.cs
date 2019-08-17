// <copyright file="IConnectionFactory.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Adapters.Object.Npgsql
{
    public interface IConnectionFactory
    {
        Connection Create(Database database);
    }
}
