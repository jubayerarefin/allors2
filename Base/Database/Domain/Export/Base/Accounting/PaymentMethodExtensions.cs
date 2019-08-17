﻿// <copyright file="PaymentMethodExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    internal static class PaymentMethodExtensions
    {
        public static void BaseOnPreDerive(this PaymentMethod @this, ObjectOnPreDerive method)
        {
            var derivation = method.Derivation;
            derivation.AddDependency(@this.InternalOrganisationWherePaymentMethod, @this);
        }
    }
}
