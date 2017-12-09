namespace Allors.Repository
{
    using Attributes;

    #region Allors
    [Id("61af6d19-e8e4-4b5b-97e8-3610fbc82605")]
    #endregion
    public partial interface InventoryItem : UniquelyIdentifiable, Transitional, Deletable
    {
        #region Allors
        [Id("91D1A28D-AE04-4445-B4AC-2053559DCFB7")]
        [AssociationId("2FBE6AA9-9E34-4A9A-9972-88E729AAEFBC")]
        [RoleId("6FE84CF4-959C-48AE-9923-C91D77E1C439")]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Indexed]
        [Workspace]
        ProductCharacteristicValue[] ProductCharacteristicValues { get; set; }

        #region Allors
        [Id("9ADBF0A8-5676-430A-8242-97660692A1F6")]
        [AssociationId("F8A66D91-2CED-4252-9B83-55519491BF79")]
        [RoleId("3A57D8C7-7D7E-44D9-9482-12C74393B0DC")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Workspace]
        InventoryItemVariance[] InventoryItemVariances { get; set; }

        #region Allors
        [Id("BCC41DF1-D526-4C78-8F68-B32AB104AD12")]
        [AssociationId("B3E13E3F-3976-4920-A602-8D371210B35F")]
        [RoleId("851F9536-0B23-4536-8060-A547CEF802D5")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        Part Part { get; set; }

        #region Allors
        [Id("EB6EFE43-6584-4460-ACA8-63153FCAECFF")]
        [AssociationId("0F891CC1-146A-4C7D-9F10-3D23A684C0E7")]
        [RoleId("14272F0F-1A1A-4086-B3F0-278A58370DAB")]
        #endregion
        [Derived]
        [Required]
        [Size(256)]
        [Workspace]
        string Name { get; set; }

        #region Allors
        [Id("8573F543-0EB9-4A5E-A68F-CC69CD5CF8F9")]
        [AssociationId("D4523FD7-ADE5-44A6-B982-9738212BD809")]
        [RoleId("2EF751E3-6250-4DDF-BBFF-B8E468A8B7D4")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        Lot Lot { get; set; }

        #region Allors
        [Id("7E14E5BC-0591-49F7-81DE-6D967CC01A83")]
        [AssociationId("A60A81E6-7879-4571-871B-A95C6F4092CB")]
        [RoleId("C4029421-3F0C-405B-AB34-7C762651B5D7")]
        #endregion
        [Derived]
        [Required]
        [Size(256)]
        [Workspace]
        string Sku { get; set; }

        #region Allors
        [Id("D276D126-34D3-4820-884C-EC9944B5E10B")]
        [AssociationId("8AD230B1-A664-4A4D-A58C-FAFB98C11762")]
        [RoleId("730949B3-3CDE-46F7-816B-331AFFF7AEF5")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Derived]
        [Workspace]
        UnitOfMeasure UnitOfMeasure { get; set; }

        #region Allors
        [Id("406E0951-1DAB-4053-88D1-E15AD5D1E833")]
        [AssociationId("5D0AD2E3-D32B-4668-879D-ED663880E7BC")]
        [RoleId("3CFD95F7-18C2-4D0A-BE0A-A46080ED5252")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Derived]
        ProductCategory[] DerivedProductCategories { get; set; }

        #region Allors
        [Id("7DB2CA47-D31C-4489-8E53-15F702EA4DD7")]
        [AssociationId("5A97682E-E7B4-417F-B7FB-0306C1F6A151")]
        [RoleId("78592A7B-7599-4240-95AF-806B0C0C617E")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        Good Good { get; set; }

        #region Allors
        [Id("B316EB62-A654-4429-9699-403B23DB5284")]
        [AssociationId("F3A6EA79-9E12-405A-8195-90FC3973BD65")]
        [RoleId("BA8E7FFA-8557-4452-B97B-1A5E2BFA83D0")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        ProductType ProductType { get; set; }

        #region Allors
        [Id("BC234CEA-DC2E-4BDC-B911-5A12D1D6F354")]
        [AssociationId("DCA4388A-D549-4CEA-931B-074244DE8E18")]
        [RoleId("94231D6C-7699-4428-AFFE-A459C8208394")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Required]
        [Workspace]
        Facility Facility { get; set; }
    }
}