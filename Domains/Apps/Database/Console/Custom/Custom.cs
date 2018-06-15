﻿namespace Allors.Console
{
    using Allors.Domain;
    using Allors.Services;

    using Microsoft.Extensions.Logging;

    public class Custom 
    {
        private readonly IDatabase database;
        private readonly ILogger<Custom> logger;

        public Custom(IDatabaseService databaseService, ILogger<Custom> logger)
        {
            this.database = databaseService.Database;
            this.logger = logger;
        }

        public int Execute()
        {
            using (var session = this.database.CreateSession())
            {
                this.logger.LogInformation("Begin");

                var administrator = new Users(session).GetUser("administrator");
                session.SetUser(administrator);

                session.Derive();
                session.Commit();

                this.logger.LogInformation("End");
            }

            return 0;
        }
    }
}
