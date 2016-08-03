using System.Web.Mvc;
using System.Web.Routing;

namespace Sample.AspNet
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Checkout",
                url: "Checkout/{id}/{action}",
                defaults: new { controller = "Checkout", action = "Index" });

            routes.MapRoute(
                name: "PaymentCreateAjax",
                url: "Payment",
                defaults: new { controller = "Payment", action = "CreateAjax" });

            routes.MapRoute(
                name: "Payment",
                url: "Payment/{transactionId}/{action}",
                defaults: new { controller = "Payment", action = "Index" });

            routes.MapRoute(
                name: "DefaultHome",
                url: "{controller}",
                defaults: new { controller = "Home", action = "Index" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
