using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FalconOrchestratorWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Detection", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                 name: "Admin",
                 url: "admin/{controller}/{action}/{id}",
                 defaults: new { controller = "Detection", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}