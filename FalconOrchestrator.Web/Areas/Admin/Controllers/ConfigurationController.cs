//<Falcon Orchestrator provides automated workflow and response capabilities>
//    Copyright(C) 2016 CrowdStrike

//   This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or(at your option) any later version.

//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FalconOrchestratorWeb.Areas.Admin.Models;
using FalconOrchestratorWeb.Areas.Admin.Repository;

namespace FalconOrchestratorWeb.Areas.Admin.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ConfigurationRepository repo = new ConfigurationRepository();

        [HttpGet]
        public ActionResult Index()
        {
             ConfigurationItemsViewModel model = repo.MapExisitingValues();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LDAPSettings(LDAPConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repo.UpdateLDAPSettings(model);
                    return PartialView("_Success", "Active Directory settings have been updated");
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.Message);
                }
            }

            List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
            return PartialView("_ValidationError", errors);
          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailSettings(EmailConfigViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    repo.UpdateEmailSettings(model);
                    return PartialView("_Success", "Email settings have been updated");
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.Message);
                }
            }

            List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
            return PartialView("_ValidationError", errors);
                   
        }

        [HttpPost]
        public ActionResult FalconStreamingAPISettings(FalconStreamingAPIConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repo.UpdateFalconStreamingAPISettings(model);
                    return PartialView("_Success", "Falcon Streaming API settings have been updated");
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.Message);
                }
            }

            List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
            return PartialView("_ValidationError", errors);
          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FalconForensicsSettings(FalconForensicsConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repo.UpdateFalconForensicsSettings(model);
                    return PartialView("_Success", "Forensics settings have been updated");
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.Message);
                }
            }

            List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
            return PartialView("_ValidationError", errors);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FalconQueryAPISettings(FalconQueryAPIConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repo.UpdateFalconQueryAPISettings(model);
                    return PartialView("_Success", "Falcon Query API settings have been updated");
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.Message);
                }
            }
            List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
            return PartialView("_ValidationError", errors);          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult ETLRules(ETLRulesViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repo.UpdateETLRules(model);
                    return PartialView("_Success", "Processing rules have been updated");
                }
                catch (Exception e)
                {
                    return PartialView("_Error", e.Message);
                }
            }

            List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
            return PartialView("_ValidationError", errors);          
        }

    }
}
