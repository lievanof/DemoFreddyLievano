using Demo.DataAccessLayer.Context;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Frontend.Identity
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Data.Entity.Database.SetInitializer(new
                MigrateDatabaseToLatestVersion<DemoContext,
                    Demo.DataAccessLayer.Migrations.Configuration>());

            using (var context = new DemoContext())
            {
                context.Database.Initialize(false);
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
