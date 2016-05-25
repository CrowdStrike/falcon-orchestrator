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
using System.Net;
using System.Web.Mvc;
using System.Threading.Tasks;
using FalconOrchestratorWeb.Repository;
using FalconOrchestratorWeb.Models.ViewModels;

namespace FalconOrchestratorWeb.Controllers
{
    public class TicketingController : Controller
    {
        private TicketingRepository repo = new TicketingRepository();

        [HttpGet]
        public ActionResult Index()
        {
            return View(repo.GetList());
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(repo.Get(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                }
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return View(new TicketViewModel());
            }
            DetectionRepository detectRepo = new DetectionRepository();
            DetectionEditViewModel detect = detectRepo.Get(id);

            var model = new TicketViewModel();
            model.Asset = detect.Asset;
            model.DetectionId = detect.DetectionId.ToString();
            model.AssociatedEventCount = detect.AssociatedEventCount;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await repo.Create(model);
                    return RedirectToAction("Index");
                }

                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                }
            }
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

    }
}
