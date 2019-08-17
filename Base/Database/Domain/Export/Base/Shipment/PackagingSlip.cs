// <copyright file="PackagingSlip.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace Allors.Domain
{
    public partial class PackagingSlip
    {
        public Shipment GetShipment => this.ShipmentPackageWhereDocument.ShipmentWhereShipmentPackage;
    }
}
