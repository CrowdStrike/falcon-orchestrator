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
using System.Web.Mvc;
using FalconOrchestratorWeb.Repository;
using FalconOrchestrator.DAL;
using FalconOrchestratorWeb.Models.ViewModels;
using System.Net;


namespace FalconOrchestratorWeb.Controllers
{
    [Authorize]
    public class WhitelistingController : BaseController
    {
        private WhitelistingRepository repo = new WhitelistingRepository();

        [HttpGet]
        public ActionResult Index()
        {
            return View(repo.GetList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            FalconOrchestratorDB db = new FalconOrchestratorDB();
            ViewBag.TypeId = new SelectList(db.WhitelistTypes, "WhitelistTypeId", "Type", String.Empty);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WhitelistingViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    repo.Create(model);
                    RefreshDetections();
                    return RedirectToAction("Index");
                }

                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                }
            }

            FalconOrchestratorDB db = new FalconOrchestratorDB();
            ViewBag.TypeId = new SelectList(db.WhitelistTypes, "WhitelistTypeId", "Type", String.Empty);
            return View(model);
        }


        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            repo.Delete(id);
            Response.StatusCode = 200;
            return new EmptyResult();

        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FalconOrchestratorDB db = new FalconOrchestratorDB();
            WhitelistingViewModel model = repo.Get(id);
            ViewBag.TypeId = new SelectList(db.WhitelistTypes, "WhitelistTypeId", "Type", model.TypeId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WhitelistingViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                }
            }

            FalconOrchestratorDB db = new FalconOrchestratorDB();
            ViewBag.TypeId = new SelectList(db.WhitelistTypes, "WhitelistTypeId", "Type", model.Type);
            return View(model);

        }

    }
}
