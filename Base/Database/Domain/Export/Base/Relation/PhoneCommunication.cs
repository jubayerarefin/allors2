// <copyright file="PhoneCommunication.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Allors.Meta;
using Resources;

namespace Allors.Domain
{
    public partial class PhoneCommunication
    {
        public static readonly TransitionalConfiguration[] StaticTransitionalConfigurations =
            {
                new TransitionalConfiguration(M.PhoneCommunication, M.PhoneCommunication.CommunicationEventState),
            };

        public TransitionalConfiguration[] TransitionalConfigurations => StaticTransitionalConfigurations;
    }
}
