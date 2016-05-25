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


using System.Net;
using System.Web.Mvc;
using FalconOrchestratorWeb.Areas.Admin.Models;
using FalconOrchestratorWeb.Areas.Admin.Repository;


namespace FalconOrchestratorWeb.Areas.Admin.Controllers 
{
    public class RecipientController : Controller
    {
        private readonly RecipientRepository repo = new RecipientRepository();

        [HttpGet]
        public ActionResult Index()
        {
            return View(repo.GetList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new RecipientViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RecipientViewModel model)
        {
            if (ModelState.IsValid)
            {
                repo.Create(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RecipientViewModel model = repo.Get(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RecipientViewModel model)
        {
            if (ModelState.IsValid)
            {
                repo.Update(model);
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }
    }
}
