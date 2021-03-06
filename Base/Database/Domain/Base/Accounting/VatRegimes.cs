// <copyright file="VatRegimes.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using Allors.Meta;

    public partial class VatRegimes
    {
        public static readonly Guid PrivatePersonId = new Guid("001A6A60-CC8A-4e6a-8FC0-BCE9707FA496");
        public static readonly Guid Assessable21Id = new Guid("5973BE64-C785-480f-AF30-74D32C6D6AF9");
        public static readonly Guid Assessable10Id = new Guid("a82edb4e-dc92-4864-96fe-26ec6d1ef914");
        public static readonly Guid Assessable9Id = new Guid("61fe8f79-eed2-4d40-a538-35361c47ad02");
        public static readonly Guid ExportId = new Guid("3268B6E5-995D-4f4b-B94E-AF4BE25F4282");
        public static readonly Guid IntraCommunautairId = new Guid("CFA1860E-DEBA-49a8-9062-E5577CDE0CCC");
        public static readonly Guid ServiceB2BId = new Guid("4D57C8ED-1DF4-4db2-9AAA-4552257DC2BF");
        public static readonly Guid ExemptId = new Guid("82986030-5E18-43c1-8CBE-9832ACD4151D");

        private UniquelyIdentifiableSticky<VatRegime> cache;

        public VatRegime PrivatePerson => this.Cache[PrivatePersonId];

        public VatRegime Assessable21 => this.Cache[Assessable21Id];

        public VatRegime Assessable10 => this.Cache[Assessable10Id];

        public VatRegime Assessable9 => this.Cache[Assessable9Id];

        public VatRegime Export => this.Cache[ExportId];

        public VatRegime IntraCommunautair => this.Cache[IntraCommunautairId];

        public VatRegime ServiceB2B => this.Cache[ServiceB2BId];

        public VatRegime Exempt => this.Cache[ExemptId];

        private UniquelyIdentifiableSticky<VatRegime> Cache => this.cache ??= new UniquelyIdentifiableSticky<VatRegime>(this.Session);

        protected override void BasePrepare(Setup setup)
        {
            setup.AddDependency(this.ObjectType, M.VatRate);
            setup.AddDependency(this.ObjectType, M.VatClause);
        }

        protected override void BaseSetup(Setup setup)
        {
            var vatRate0 = new VatRates(this.Session).Zero;
            var vatRate21 = new VatRates(this.Session).TwentyOne;
            var vatRate10 = new VatRates(this.Session).Ten;
            var vatRate9 = new VatRates(this.Session).Nine;

            var dutchLocale = new Locales(this.Session).DutchNetherlands;

            var merge = this.Cache.Merger().Action();
            var localisedName = new LocalisedTextAccessor(this.Meta.LocalisedNames);

            merge(PrivatePersonId, v =>
            {
                v.Name = "Private Person";
                localisedName.Set(v, dutchLocale, "particulier");
                v.VatRate = vatRate21;
                v.IsActive = true;
            });

            merge(Assessable21Id, v =>
            {
                v.Name = "VAT Assessable 21%";
                localisedName.Set(v, dutchLocale, "BTW-plichtig 21%");
                v.VatRate = vatRate21;
                v.IsActive = true;
            });

            merge(Assessable10Id, v =>
            {
                v.Name = "VAT Assessable 10%";
                localisedName.Set(v, dutchLocale, "BTW-plichtig 10%");
                v.VatRate = vatRate10;
                v.IsActive = true;
            });

            merge(Assessable9Id, v =>
            {
                v.Name = "VAT Assessable 9%";
                localisedName.Set(v, dutchLocale, "BTW-plichtig 9%");
                v.VatRate = vatRate9;
                v.IsActive = true;
            });

            merge(ExportId, v =>
            {
                v.Name = "Export";
                localisedName.Set(v, dutchLocale, "Export");
                v.VatRate = vatRate0;
                v.IsActive = true;
            });

            merge(IntraCommunautairId, v =>
            {
                v.Name = "Intracommunautair";
                localisedName.Set(v, dutchLocale, "Intracommunautair");
                v.VatRate = vatRate0;
                v.VatClause = new VatClauses(this.Session).Intracommunautair;
                v.IsActive = true;
            });

            merge(ServiceB2BId, v =>
            {
                v.Name = "Service B2B: Not VAT assessable";
                localisedName.Set(v, dutchLocale, "Service B2B: Niet BTW-plichtig");
                v.VatRate = vatRate0;
                v.VatClause = new VatClauses(this.Session).ServiceB2B;
                v.IsActive = true;
            });

            merge(ExemptId, v =>
            {
                v.Name = "Exempt";
                localisedName.Set(v, dutchLocale, "Vrijgesteld");
                v.VatRate = vatRate0;
                v.IsActive = true;
            });
        }
    }
}
