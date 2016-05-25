using System.Web.Mvc;

namespace FalconOrchestratorWeb.Areas.Forensics
{
    public class ForensicsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "forensics";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SoftwareManagement",
                "forensics/software-management/{action}/{id}",
                new { Controller = "SoftwareManagement", action = "Index", id = UrlParameter.Optional }
                );

            context.MapRoute(
                "FileExtraction",
                "forensics/file-extraction/{action}/{id}",
                new { Controller = "FileExtraction", action = "Index", id = UrlParameter.Optional }
                );

            context.MapRoute(
                "FileBrowser",
                "forensics/file-browser/{action}/{id}",
                new { Controller = "FileBrowser", action = "Index", id = UrlParameter.Optional }
                );

            context.MapRoute(
                "forensics_default",
                "forensics/{controller}/{action}/{id}",
                new { Controller = "forensics", action = "index", id = UrlParameter.Optional }
            );
        }
    }
}
