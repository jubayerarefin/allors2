namespace Tests
{
    using System;
    using System.Collections.Generic;

    using Allors.Domain;
    using Allors.Meta;
    using Allors.Server;

    using Xunit;

    [Collection("Server")]
    public class PushTests : ServerTest
    {
        [Fact]
        public async void WorkspaceNewObject()
        {
            var administrator = new Users(this.Session).GetUser("administrator");
            await this.SignIn(administrator);

            var uri = new Uri(@"Database/Push", UriKind.Relative);

            var pushRequest = new PushRequest
                                  {
                                      NewObjects = new[] { new PushRequestNewObject { T = "Build", NI = "-1" }, }
                                  };
            var response = await this.PostAsJsonAsync(uri, pushRequest);
            var pushResponse = await this.ReadAsAsync<PushResponse>(response);

            this.Session.Rollback();

            var build = (Build)this.Session.Instantiate(pushResponse.NewObjects[0].I);

            Assert.Equal(new Guid("DCE649A4-7CF6-48FA-93E4-CDE222DA2A94"), build.Guid);
            Assert.Equal("Exist", build.String);
        }


        [Fact]
        public async void DeletedObject()
        {
            var administrator = new Users(this.Session).GetUser("administrator");
            await this.SignIn(administrator);

            var organisation = new OrganisationBuilder(this.Session).Build();
            this.Session.Commit();

            var organisationId = organisation.Id.ToString();
            var organisationVersion = organisation.Strategy.ObjectVersion.ToString();

            organisation.Delete();
            this.Session.Commit();

            var uri = new Uri(@"Database/Push", UriKind.Relative);

            var pushRequest = new PushRequest
                                  {
                                      Objects = new[] { new PushRequestObject { 
                                                                                  I = organisationId, 
                                                                                  V = organisationVersion, 
                                                                                  Roles = new[]
                                                                                      {
                                                                                          new PushRequestRole
                                                                                              {
                                                                                                  T = M.Organisation.Name.PropertyName,
                                                                                                  S = "Acme"
                                                                                              }
                                                                                      }
                                                                                  },
                                                          }
                                  };
            var response = await this.PostAsJsonAsync(uri, pushRequest);

            Assert.True(response.IsSuccessStatusCode);

            var pushResponse = await this.ReadAsAsync<PushResponse>(response);

            Assert.True(pushResponse.HasErrors);
            Assert.Single(pushResponse.MissingErrors);
            Assert.Contains(organisationId, pushResponse.MissingErrors);
        }
    }
}