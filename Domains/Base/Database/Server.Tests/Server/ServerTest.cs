//------------------------------------------------------------------------------------------------- 
// <copyright file="ServerTest.cs" company="Allors bvba">
// Copyright 2002-2009 Allors bvba.
// 
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// 
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
// <summary>Defines the DomainTest type.</summary>
//-------------------------------------------------------------------------------------------------

namespace Tests
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using Allors;
    using Allors.Domain;
    using Allors.Meta;
    using Allors.Server;
    using Allors.Services.Base;

    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    using Tests;

    public abstract class ServerTest : IDisposable
    {
        protected ServerTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(new FileInfo("../../..").FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();

            this.ObjectFactory = new ObjectFactory(MetaPopulation.Instance, typeof(User));
            var configuration = new Allors.Adapters.Object.SqlClient.Configuration
            {
                ObjectFactory = this.ObjectFactory,
                ConnectionString = this.Configuration.GetConnectionString("DefaultConnection")
            };

            var database = new Allors.Adapters.Object.SqlClient.Database(configuration);
          
            this.SetUp(database, true);
        }

        public IConfigurationRoot Configuration { get; set; }

        protected ObjectFactory ObjectFactory { get; }

        protected ISession Session { get; private set; }

        protected HttpClient HttpClient { get; set; }

        protected HttpClientHandler HttpClientHandler { get; set; }

        public void Dispose()
        {
            this.Session.Rollback();
            this.Session = null;

            this.HttpClient.Dispose();
            this.HttpClient = null;
        }
        
        protected void SetUp(IDatabase database, bool setup)
        {
            database.Init();

            var timeService = new TimeService();
            var mailService = new TestMailService();
            var securityService = new SecurityService();
            var serviceLocator = new ServiceLocator
                                     {
                                         TimeServiceFactory = () => timeService,
                                         MailServiceFactory = () => mailService,
                                         SecurityServiceFactory = () => securityService
                                     };
            database.SetServiceLocator(serviceLocator.Assert());

            this.Session = database.CreateSession();

            if (setup)
            {
                new Setup(this.Session, null).Apply();
                this.Session.Commit();
            }

            this.HttpClientHandler = new HttpClientHandler();
            this.HttpClient = new HttpClient(this.HttpClientHandler)
                                  {
                                      BaseAddress = new Uri(ServerFixture.Url),
                                  };

            this.HttpClient.DefaultRequestHeaders.Accept.Clear();
            this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async System.Threading.Tasks.Task SignIn(User user)
        {
            var args = new SignInRequest
                           {
                               UserName = user.UserName,
                           };

            var uri = new Uri("TestAuthentication/SignIn", UriKind.Relative);
            var response = await this.PostAsJsonAsync(uri, args);
            var siginInResponse = await this.ReadAsAsync<SignInResponse>(response);
            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", siginInResponse.Token);
        }

        protected Stream GetResource(string name)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream(name);
            return resource;
        }

        protected async Task<HttpResponseMessage> PostAsJsonAsync(Uri uri, object args)
        {
            var json = JsonConvert.SerializeObject(args);
            return await this.HttpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        protected async Task<T> ReadAsAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<T>(json);
            return deserializedObject;
        }
    }
}