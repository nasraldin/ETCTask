using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ETCTask
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Database.SetInitializer<AppDbContext>(new DropCreateDatabaseAlways<AppDbContext>());
            //using (var context = new AppDbContext())
            //{
            //    context.Database.Initialize(force: true);
            //}
        }
    }
}