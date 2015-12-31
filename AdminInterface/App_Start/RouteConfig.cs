using System.Web.Mvc;
using System.Web.Routing;

namespace AdminInterface
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "admin", action = "PurchaseListing", id = UrlParameter.Optional }
            //);

        }
    }
}
