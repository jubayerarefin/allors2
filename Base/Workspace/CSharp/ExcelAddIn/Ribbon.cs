﻿// <copyright file="Ribbon.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ExcelAddIn
{
    using System;
    using Allors.Excel;
    using Microsoft.Office.Tools.Ribbon;
    using Nito.AsyncEx;

    public partial class Ribbon
    {
        private Commands Commands { get; set; }

        private Sheets Sheets { get; set; }

        public void Init(Commands commands, Sheets sheets, Mediator mediator)
        {
            this.Sheets = sheets;
            this.Commands = commands;

            mediator.StateChanged += this.MediatorOnStateChanged;
        }

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private async void SaveButton_Click(object sender, RibbonControlEventArgs eventArgs)
        {
            try
            {
                this.EnsureAddInManager();

                AsyncContext.Run(
                    async () =>
                        {
                            if (this.Commands != null)
                            {
                                await this.Commands.Save();
                            }
                        });
            }
            catch (Exception e)
            {
                e.Handle();
            }
        }

        private async void RefreshButton_Click(object sender, RibbonControlEventArgs eventArgs)
        {
            try
            {
                this.EnsureAddInManager();

                AsyncContext.Run(
                    async () =>
                        {
                            if (this.Commands != null)
                            {
                                await this.Commands.Refresh();
                            }
                        });
            }
            catch (Exception e)
            {
                e.Handle();
            }
        }

        private async void PeopleInitializeButton_Click(object sender, RibbonControlEventArgs eventArgs)
        {
            try
            {
                this.EnsureAddInManager();

                AsyncContext.Run(
                    async () =>
                    {
                        if (this.Commands != null)
                        {
                            await this.Commands.PeopleNew();
                        }
                    });
            }
            catch (Exception e)
            {
                e.Handle();
            }
            finally
            {
                this.RibbonUI.ActivateTab(this.baseTab.ControlId.ToString());
            }
        }

        private void MediatorOnStateChanged(object sender, EventArgs eventArgs)
        {
            var sheet = this.Sheets.ActiveSheet;
            var existSheet = sheet != null;

            this.saveButton.Enabled = existSheet && this.Commands.CanSave;
            this.refreshButton.Enabled = existSheet && this.Commands.CanRefresh;
        }

        private async void PurchaseInvoicesInitializeButton_Click(object sender, RibbonControlEventArgs eventArgs)
        {
            try
            {
                this.EnsureAddInManager();

                AsyncContext.Run(
                    async () =>
                    {
                        if (this.Commands != null)
                        {
                            await this.Commands.PurchaseInvoicesNew();
                        }
                    });
            }
            catch (Exception e)
            {
                e.Handle();
            }
            finally
            {
                this.RibbonUI.ActivateTab(this.baseTab.ControlId.ToString());
            }
        }

        private async void CustomerInitializeButton_Click(object sender, RibbonControlEventArgs eventArgs)
        {
            try
            {
                this.EnsureAddInManager();

                AsyncContext.Run(
                    async () =>
                    {
                        if (this.Commands != null)
                        {
                            await this.Commands.CustomersNew();
                        }
                    });
            }
            catch (Exception e)
            {
                e.Handle();
            }
            finally
            {
                this.RibbonUI.ActivateTab(this.baseTab.ControlId.ToString());
            }
        }

        private async void SalesInvoicesOverdueButton_Click(object sender, RibbonControlEventArgs eventArgs)
        {
            try
            {
                this.EnsureAddInManager();

                AsyncContext.Run(
                    async () =>
                    {
                        if (this.Commands != null)
                        {
                            await this.Commands.SalesInvoicesOverdueNew();
                        }
                    });
            }
            catch (Exception e)
            {
                e.Handle();
            }
            finally
            {
                this.RibbonUI.ActivateTab(this.baseTab.ControlId.ToString());
            }
        }

        private void EnsureAddInManager() =>
            AsyncContext.Run(
                async () =>
                {
                    if (Globals.ThisAddIn.AddInManager == null || Globals.ThisAddIn.AddInManager.IsLoggedIn == false)
                    {
                        Globals.ThisAddIn.InitAddInManager();
                    }
                });

        private void ButtonLogoff_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.AddInManager?.Logoff();
            this.RibbonUI.ActivateTab(this.baseTab.ControlId.ToString());
        }
    }
}
