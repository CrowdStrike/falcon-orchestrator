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
using FalconOrchestratorWeb.Models.ViewModels;
using FalconOrchestratorWeb.Repository;

namespace FalconOrchestratorWeb.Controllers
{
    public class DetectionController : BaseController
    {
        private DetectionRepository repo = new DetectionRepository();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (repo.Get(id) == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(repo.Get(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetectionEditViewModel model)
        {
            if(ModelState.IsValid)
            {
                repo.Update(model);
                RefreshDetections();
                return RedirectToAction("Index");
            }
            return View(model);
        }


    }
}
