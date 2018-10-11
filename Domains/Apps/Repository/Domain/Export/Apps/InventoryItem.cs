namespace Allors.Repository
{
    using Attributes;

    #region Allors
    [Id("61af6d19-e8e4-4b5b-97e8-3610fbc82605")]
    #endregion
    public partial interface InventoryItem : UniquelyIdentifiable, Transitional, Deletable
    {
        #region SerialisedInventoryItemState

        #region Allors
        [Id("CCB71B4F-1A3F-4D08-B3E4-380FB2D513FF")]
        [AssociationId("D35F6D66-DAA2-4044-B4E9-FBCFBC7D2CD9")]
        [RoleId("35C5FABD-1F83-4D6C-8268-F027CC9F7B51")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Derived]
        InventoryItemState PreviousInventoryItemState { get; set; }

        #region Allors
        [Id("72A268C1-4A32-48C1-BB2D-837AC1DF361E")]
        [AssociationId("0ED35F86-9400-4F89-8F9D-A8D6A7408A78")]
        [RoleId("DF809B37-E9DA-463C-B532-02E44BC0394F")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Derived]
        InventoryItemState LastInventoryItemState { get; set; }

        #region Allors
        [Id("7E757767-61AC-49E9-97CF-DE929C015D5B")]
        [AssociationId("60B25B4C-B160-498C-A3CF-EBB057EACACC")]
        [RoleId("87B18D10-A205-40E7-8403-733791AF3FD9")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        InventoryItemState InventoryItemState { get; set; }

        #endregion SerialisedInventoryItemState

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
        [Required]
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        Part Part { get; set; }

        #region Allors
        [Id("EB6EFE43-6584-4460-ACA8-63153FCAECFF")]
        [AssociationId("0F891CC1-146A-4C7D-9F10-3D23A684C0E7")]
        [RoleId("14272F0F-1A1A-4086-B3F0-278A58370DAB")]
        #endregion
        [Derived]
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