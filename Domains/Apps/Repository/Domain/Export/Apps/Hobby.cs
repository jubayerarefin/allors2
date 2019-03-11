namespace Allors.Repository
{
    using System;

    using Attributes;

    #region Allors
    [Id("2f18f79f-dd13-4e89-b3fa-95d789dd383e")]
    #endregion
    public partial class Hobby : Enumeration 
    {
        #region inherited properties
        public LocalisedText[] LocalisedNames { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public Permission[] DeniedPermissions { get; set; }

        public SecurityToken[] SecurityTokens { get; set; }

        public Guid UniqueId { get; set; }

        #endregion


        #region inherited methods


        public void OnBuild(){}

        public void OnPostBuild(){}

        public void OnInit()
        {
            
        }

        public void OnPreDerive(){}

        public void OnDerive(){}

        public void OnPostDerive(){}

        #endregion

        #region Allors
        [Id("8F351AE4-F41D-4188-B6BE-6BBC7AE39E5E")]
        #endregion
        [Workspace]
        public void Delete() { }
    }
}