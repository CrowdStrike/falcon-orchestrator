using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FalconOrchestratorWeb.Utility;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb
{

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ExtendedRazorViewEngine engine = new ExtendedRazorViewEngine();
            ViewEngines.Engines.Add(engine);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            FalconOrchestratorDB db = new FalconOrchestratorDB();
            Application["Detections"] = db.Detections.Include("DetectionDevice").Include("Account").ToList();
        }
    }
}