namespace Intranet.Tests.Orders
{
    using Intranet.Pages.Orders;

    using Xunit;

    [Collection("Test collection")]
    public class OrdersOverviewTest : Test
    {
        public OrdersOverviewTest(TestFixture fixture)
            : base(fixture)
        {
            var dashboard = this.Login();
            dashboard.Sidenav.NavigateToOrders();
        }

        [Fact]
        public void Title()
        {
            Assert.Equal("Sales Orders", this.Driver.Title);
        }

        [Fact]
        public void Search()
        {
            var page = new OrdersOverviewPage(this.Driver);

            page.Company.Text = "acme";
        }
    }
}