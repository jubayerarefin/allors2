namespace Allors.Workspace.Blazor.Bootstrap.Forms.Roles
{
    using System;
    using Microsoft.AspNetCore.Components;

    public class ABSNumberInputBase : RoleField
    {
        public int IntModel { get => (int)this.Model; set => this.Model = value; }

        public BlazorStrap.InputType InputType
        {
            get
            {
                if (this.TextType == "number")
                {
                    return BlazorStrap.InputType.Number;
                }

                return BlazorStrap.InputType.Text;
            }
        }

    }

}