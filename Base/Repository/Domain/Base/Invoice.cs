// <copyright file="Invoice.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using System;

    using Allors.Repository.Attributes;

    #region Allors
    [Id("a6f4eedb-b0b5-491d-bcc0-09d2bc109e86")]
    #endregion
    public partial interface Invoice : Commentable, Printable, Auditable, Transitional, Deletable
    {
        #region Allors
        [Id("8EBB1372-CA22-4639-85FC-D1C14AB0F500")]
        [AssociationId("D594FF30-C48F-4E93-9158-EF5906251CD3")]
        [RoleId("C67F31B3-A9D2-44EA-8795-7A76D5DC7F30")]
        #endregion
        [Workspace]
        [Size(-1)]
        string InternalComment { get; set; }

        #region Allors
        [Id("1c535b3f-bb97-43a8-bd29-29c4dc267814")]
        [AssociationId("d3155310-1267-4780-b69d-4dd47ef15e73")]
        [RoleId("2603b50f-78b9-429c-be30-38949bdec59a")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        [Indexed]
        Currency Currency { get; set; }

        #region Allors
        [Id("2d82521d-30bd-4185-84c7-4dfe08b5ddef")]
        [AssociationId("aa6230a9-7a9e-4d42-a14a-49b1c3b382ab")]
        [RoleId("5c1fbd73-39e2-4a4a-b58b-2e6c7a110755")]
        #endregion
        [Size(-1)]
        [Workspace]
        string Description { get; set; }

        #region Allors
        [Id("4b2eedbb-ec59-4e18-949f-f467e41f6401")]
        [AssociationId("b41474a8-482f-458f-b70d-b11e97129ea0")]
        [RoleId("5bab4dea-3566-4421-96c5-27b774b6542a")]
        #endregion
        [Size(256)]
        [Workspace]
        string CustomerReference { get; set; }

        #region Allors
        [Id("4d3f69a0-6e9d-4ba3-acd8-e5dab2a7f401")]
        [AssociationId("4ac19707-3c95-4b7d-b281-2f9d86c3eeb9")]
        [RoleId("15779f7b-07ce-4373-a9cd-1ee5690ddbfc")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal AmountPaid { get; set; }

        #region Allors
        [Id("6b474ddd-c2fd-4db1-bf18-44c86a309d53")]
        [AssociationId("01576aed-1f77-47db-bf04-40aa5dcae63a")]
        [RoleId("f0bd433a-f5cc-4a6d-be5f-8f09594aa566")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalDiscount { get; set; }

        #region Allors
        [Id("6ea961d5-89fc-4526-922a-80538ecb5654")]
        [AssociationId("66c5cfdd-6af4-4d75-b826-843be3b01bca")]
        [RoleId("f560bb3d-f855-4ec3-a5e7-4bd6c4da2595")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        BillingAccount BillingAccount { get; set; }

        #region Allors
        [Id("7b6ab1ed-845d-4671-bda2-43ad2327ea53")]
        [AssociationId("d0994e3f-4741-4f9e-9f4f-8923ed3afdf3")]
        [RoleId("4b4902c0-780d-4de2-97ff-54a5f3bdc521")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalIncVat { get; set; }

        #region Allors
        [Id("7e8de8bd-f1c0-4fa5-a629-34d9d5f71b85")]
        [AssociationId("483b0b71-a4a8-4606-a432-d98d8bd262a2")]
        [RoleId("32e8201c-9e71-48e7-ae20-972e14ea4aeb")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalSurcharge { get; set; }

        #region Allors
        [Id("7fda150d-44c8-45a9-8048-dfe38d936c3e")]
        [AssociationId("e2199200-562f-474d-a822-094fba167dc6")]
        [RoleId("09cba9f7-d85f-4c54-a857-c28f22f0eaae")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalBasePrice { get; set; }

        #region Allors
        [Id("7a783e3c-9197-4d1a-8291-f95a3b3a799d")]
        [AssociationId("03f9fd09-c4c9-4d52-b15e-e83ceb490019")]
        [RoleId("d5738ae0-13ed-4466-8fb1-f6ade4fba672")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal GrandTotal { get; set; }

        #region Allors
        [Id("82541f62-bf0e-4e33-9971-15a5a4fa4469")]
        [AssociationId("b3579af4-1c8e-46c5-bc1c-a9d7711a4a48")]
        [RoleId("d54fdbf9-c580-4a49-b058-28aab77d81e0")]
        #endregion
        [Required]
        [Workspace]
        DateTime InvoiceDate { get; set; }

        #region Allors
        [Id("8798a760-de3d-4210-bd22-165582728f36")]
        [AssociationId("d0d6a00a-2d79-4798-b51f-7e6dfb8551d5")]
        [RoleId("c1f88c71-2415-4928-ae3b-16c7f85af30c")]
        #endregion
        [Derived]
        [Required]
        [Workspace]
        DateTime EntryDate { get; set; }

        #region Allors
        [Id("94029787-f838-47bb-9617-807a8514a350")]
        [AssociationId("92badbf6-7d16-46b2-b214-1ea26855970d")]
        [RoleId("2dc528f7-451e-4570-922b-649d9448ed11")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalShippingAndHandling { get; set; }

        #region Allors
        [Id("9eec85a4-e41a-4ca2-82fa-2dc0aa45c9d5")]
        [AssociationId("26c9285b-4c0e-443e-914b-ceb95d37a8fe")]
        [RoleId("4d9bb0e9-23b1-429e-bf61-2fa3b9afb2b8")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalExVat { get; set; }

        #region Allors
        [Id("5e4bc0b7-8d9a-45ea-a5e7-8c608a286fdf")]
        [AssociationId("99c6caaf-3b1e-4735-8695-a5df344f546e")]
        [RoleId("10930c0b-ce15-47d2-bfba-fdff48103275")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Workspace]
        [Indexed]
        OrderAdjustment[] OrderAdjustments { get; set; }

        #region Allors
        [Id("9ff2d65b-0478-41cc-b70b-0df90cdbe190")]
        [AssociationId("38654202-df58-4f2a-9c8d-094fb511a19a")]
        [RoleId("a12bdf85-5c6d-43e4-92b2-8f2fefc03e3e")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Indexed]
        [Workspace]
        SalesTerm[] SalesTerms { get; set; }

        #region Allors
        [Id("ab342937-1e58-4cd7-99b5-c8a5e7afe317")]
        [AssociationId("0cd0981d-d26b-42e4-a50d-9747a1171b12")]
        [RoleId("431bbc5d-4de6-4cee-aa2d-f1f5c6e7e745")]
        #endregion
        [Required]
        [Size(256)]
        [Workspace]
        string InvoiceNumber { get; set; }

        #region Allors
        [Id("b298c12c-620b-4cf2-b47e-df17afc65552")]
        [AssociationId("4eff42a0-dfe5-440c-a2d2-7612ece8ff11")]
        [RoleId("92365fb1-d257-4fbd-81e4-097ef6d2405e")]
        #endregion
        [Size(-1)]
        [Workspace]
        string Message { get; set; }

        #region Allors
        [Id("c2ecfd15-7662-45b4-99bd-9093ca108d23")]
        [AssociationId("32efeb84-a275-4b14-ba1f-aa99ba1bc776")]
        [RoleId("4e4351e1-7174-4337-b448-bd3f79e3aaa4")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        VatRegime VatRegime { get; set; }

        #region Allors
        [Id("9a12fc10-722b-42c4-aa68-cec89aeb5c12")]
        [AssociationId("5fd3a989-2e6a-4d38-8289-dd63f5de1ebc")]
        [RoleId("c77df0d9-7303-4d5d-ae4b-3f096f45a865")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        IrpfRegime IrpfRegime { get; set; }

        #region Allors
        [Id("c7350047-9282-41c8-8d82-4e1f86369e9c")]
        [AssociationId("0468ccd7-0e03-4bff-8812-ee1f979a6a3f")]
        [RoleId("09a4e368-3d7e-4dd5-8708-fa9ff5bddc4b")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalVat { get; set; }

        #region Allors
        [Id("2bba9da0-c6d6-4af8-9f93-3bf8c7a46a98")]
        [AssociationId("88695ee4-c648-46e8-9cf9-0c3b1d0cd803")]
        [RoleId("fbdb6a04-68c2-4a98-8401-8d1bc8007d29")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalIrpf { get; set; }

        #region Allors
        [Id("fa826458-5423-43dd-b02f-fe2673a2d0f3")]
        [AssociationId("ac559656-d5c1-4325-a267-9775136a25af")]
        [RoleId("837d36ee-f23f-45bc-87a9-9760d08f29c4")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalFee { get; set; }

        #region Allors
        [Id("636a3b83-7157-42a4-bc24-db5419ccb3b7")]
        [AssociationId("06ba4fd1-96af-448a-85dc-3392b89755b2")]
        [RoleId("53485360-3319-456b-84a3-9b759b5c2117")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalExtraCharge { get; set; }

        #region Allors
        [Id("BBA2D4EA-D31F-4C68-8935-2AC3CC1A267D")]
        [AssociationId("CE243EC9-8607-4B06-8749-5BC779BD12DF")]
        [RoleId("997B1E33-048F-4358-8495-E495653706F6")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Workspace]
        [Derived]
        [Indexed]
        InvoiceItem[] ValidInvoiceItems { get; set; }

        #region Allors
        [Id("1ef6f9b0-c541-4a8d-9ce2-fb6a330244e9")]
        [AssociationId("194041ad-93b6-4eb2-802d-2e5d111c3177")]
        [RoleId("f34e32a1-606c-4272-bdf8-b0545ba0c34b")]
        #endregion
        [Indexed]
        [Derived]
        [Workspace]
        int SortableInvoiceNumber { get; set; }

        #region Allors
        [Id("eaa66e01-f597-4c71-86e5-d78652fe926b")]
        [AssociationId("43978013-8e98-46e5-941c-ba19ba4fa6a5")]
        [RoleId("fddb51c0-e427-4575-b9d3-24f6fd3f4e06")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Indexed]
        [Workspace]
        public Media[] ElectronicDocuments { get; set; }

        #region Allors

        [Id("B9226E72-AD90-4195-9DC7-64A26D12E6A3")]

        #endregion
        [Workspace]
        void Create();

        #region Allors

        [Id("832244fc-9ac7-4d45-b154-5b49136d97af")]

        #endregion
        [Workspace]
        void Revise();
    }
}
