﻿// <copyright file="Custom.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Allors;
    using Allors.Domain;
    using Allors.Services;

    using McMaster.Extensions.CommandLineUtils;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    [Command(Description = "Execute custom code")]
    public class Custom
    {
        private readonly IDatabaseService databaseService;

        private readonly ILogger<Custom> logger;

        public Custom(IDatabaseService databaseService, ILogger<Custom> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
        }

        private Commands Parent { get; set; }

        public int OnExecute(CommandLineApplication app) => this.PrintSalesInvoice();

        private int PrintPurchaseInvoice()
        {
            using (var session = this.databaseService.Database.CreateSession())
            {
                this.logger.LogInformation("Begin");

                var administrator = new Users(session).GetUser("administrator");
                session.SetUser(administrator);

                var templateFilePath = "domain/templates/PurchaseInvoice.odt";
                var templateFileInfo = new FileInfo(templateFilePath);
                var prefix = string.Empty;
                while (!templateFileInfo.Exists)
                {
                    prefix += "../";
                    templateFileInfo = new FileInfo(prefix + templateFilePath);
                }

                var invoice = new PurchaseInvoices(session).Extent().Last();
                var template = invoice.BilledTo.PurchaseInvoiceTemplate;

                using (var memoryStream = new MemoryStream())
                using (var fileStream = new FileStream(
                    templateFileInfo.FullName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    fileStream.CopyTo(memoryStream);
                    template.Media.InData = memoryStream.ToArray();
                }

                session.Derive();

                var images = new Dictionary<string, byte[]> { { "Logo", session.GetSingleton().LogoImage.MediaContent.Data }, };

                if (invoice.ExistInvoiceNumber)
                {
                    var barcodeService = session.ServiceProvider.GetRequiredService<IBarcodeService>();
                    var barcode = barcodeService.Generate(invoice.InvoiceNumber, BarcodeType.CODE_128, 320, 80);
                    images.Add("Barcode", barcode);
                }

                var printModel = new Allors.Domain.Print.PurchaseInvoiceModel.Model(invoice);
                invoice.RenderPrintDocument(template, printModel, images);

                session.Derive();

                var result = invoice.PrintDocument;

                var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var outputFile = File.Create(Path.Combine(desktopDir, "purchaseInvoice.odt"));
                using (var stream = new MemoryStream(result.Media.MediaContent.Data))
                {
                    stream.CopyTo(outputFile);
                }

                this.logger.LogInformation("End");
            }

            return ExitCode.Success;
        }

        private int PrintSalesInvoice()
        {
            using (var session = this.databaseService.Database.CreateSession())
            {
                this.logger.LogInformation("Begin");

                var administrator = new Users(session).GetUser("administrator");
                session.SetUser(administrator);

                var templateFilePath = "domain/templates/SalesInvoice.odt";
                var templateFileInfo = new FileInfo(templateFilePath);
                var prefix = string.Empty;
                while (!templateFileInfo.Exists)
                {
                    prefix += "../";
                    templateFileInfo = new FileInfo(prefix + templateFilePath);
                }

                var invoice = new SalesInvoices(session).Extent().Last();
                var template = invoice.BilledFrom.SalesInvoiceTemplate;

                using (var memoryStream = new MemoryStream())
                using (var fileStream = new FileStream(
                    templateFileInfo.FullName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    fileStream.CopyTo(memoryStream);
                    template.Media.InData = memoryStream.ToArray();
                }

                session.Derive();

                var images = new Dictionary<string, byte[]> { { "Logo", session.GetSingleton().LogoImage.MediaContent.Data }, };

                if (invoice.ExistInvoiceNumber)
                {
                    var barcodeService = session.ServiceProvider.GetRequiredService<IBarcodeService>();
                    var barcode = barcodeService.Generate(invoice.InvoiceNumber, BarcodeType.CODE_128, 320, 80);
                    images.Add("Barcode", barcode);
                }

                var printModel = new Allors.Domain.Print.SalesInvoiceModel.Model(invoice);
                invoice.RenderPrintDocument(template, printModel, images);

                session.Derive();

                var result = invoice.PrintDocument;

                var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var outputFile = File.Create(Path.Combine(desktopDir, "salesInvoice.odt"));
                using (var stream = new MemoryStream(result.Media.MediaContent.Data))
                {
                    stream.CopyTo(outputFile);
                }

                this.logger.LogInformation("End");
            }

            return ExitCode.Success;
        }

        private int PrintWorkTask()
        {
            using (var session = this.databaseService.Database.CreateSession())
            {
                this.logger.LogInformation("Begin");

                var administrator = new Users(session).GetUser("administrator");
                session.SetUser(administrator);

                var templateFilePath = "domain/templates/WorkTask.odt";
                var templateFileInfo = new FileInfo(templateFilePath);
                var prefix = string.Empty;
                while (!templateFileInfo.Exists)
                {
                    prefix += "../";
                    templateFileInfo = new FileInfo(prefix + templateFilePath);
                }

                var workTasks = new WorkTasks(session).Extent();
                var workTask = workTasks.First(v => v.Name.Equals("maintenance"));
                var template = workTask.TakenBy.WorkTaskTemplate;

                using (var memoryStream = new MemoryStream())
                using (var fileStream = new FileStream(
                    templateFileInfo.FullName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    fileStream.CopyTo(memoryStream);
                    template.Media.InData = memoryStream.ToArray();
                }

                session.Derive();

                var images = new Dictionary<string, byte[]> { { "Logo", session.GetSingleton().LogoImage.MediaContent.Data }, };

                if (workTask.ExistWorkEffortNumber)
                {
                    var barcodeService = session.ServiceProvider.GetRequiredService<IBarcodeService>();
                    var barcode = barcodeService.Generate(workTask.WorkEffortNumber, BarcodeType.CODE_128, 320, 80);
                    images.Add("Barcode", barcode);
                }

                var printModel = new Allors.Domain.Print.WorkTaskModel.Model(workTask);
                workTask.RenderPrintDocument(template, printModel, images);

                session.Derive();

                var result = workTask.PrintDocument;

                var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var outputFile = File.Create(Path.Combine(desktopDir, "workTask.odt"));
                using (var stream = new MemoryStream(result.Media.MediaContent.Data))
                {
                    stream.CopyTo(outputFile);
                }

                this.logger.LogInformation("End");
            }

            return ExitCode.Success;
        }

        private int MonthlyScheduler()
        {
            using (var session = this.databaseService.Database.CreateSession())
            {
                this.logger.LogInformation("Begin");

                var administrator = new Users(session).GetUser("administrator");
                session.SetUser(administrator);

                WorkTasks.BaseMonthly(session);

                var validation = session.Derive(false);

                if (validation.HasErrors)
                {
                    foreach (var error in validation.Errors)
                    {
                        this.logger.LogError("Validation error: {error}", error);
                    }

                    session.Rollback();
                }
                else
                {
                    session.Commit();
                }

                session.Derive();
                session.Commit();

                this.logger.LogInformation("End");
            }

            return ExitCode.Success;
        }
    }
}
