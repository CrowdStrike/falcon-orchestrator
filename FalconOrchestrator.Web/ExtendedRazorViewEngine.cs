using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Razor;
using System.Web.Mvc;

namespace FalconOrchestratorWeb
{
    public class ExtendedRazorViewEngine : RazorViewEngine
    {
        public void AddViewLocationFormat(string paths)
        {
            List<string> existingPaths = new List<string>(ViewLocationFormats);
            existingPaths.Add(paths);

            ViewLocationFormats = existingPaths.ToArray();
        }

        public void AddPartialViewLocationFormat(string paths)
        {
            List<string> existingPaths = new List<string>(PartialViewLocationFormats);
            existingPaths.Add(paths);

            PartialViewLocationFormats = existingPaths.ToArray();
        }
    }

}