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
using FalconOrchestrator.DAL;
using FalconOrchestratorWeb.Areas.Admin.Models;

namespace FalconOrchestratorWeb.Areas.Admin.Controllers
{
    public class ScheduleController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            FalconOrchestratorDB db = new FalconOrchestratorDB();
            List<ResponderSchedule> handlerschedules = db.ResponderSchedules.ToList();
            List<ScheduleViewModel> result = new List<ScheduleViewModel>();

            foreach (var line in handlerschedules)
            {
                ScheduleViewModel model = new ScheduleViewModel();
                model.DayOfWeek = line.DayOfWeek;
                model.ScheduleId = line.ScheduleId;
                model.FullName = line.Responder != null ? line.Responder.FirstName + " " + line.Responder.LastName : null;
                model.ResponderId = line.ResponderId != null ? line.ResponderId.ToString() : null;
                result.Add(model);
            }
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ScheduleViewModel[] schedules)
        {
                if (ModelState.IsValid)
                {
                    try
                    {
                        FalconOrchestratorDB db = new FalconOrchestratorDB();
                        foreach (var line in schedules)
                        {
                            ResponderSchedule row = db.ResponderSchedules.Find(line.ScheduleId);
                            row.ResponderId = line.ResponderId != null ? (int?)Convert.ToInt32(line.ResponderId) : null;
                            db.Entry(row).State = System.Data.Entity.EntityState.Modified;
                        }
                        db.SaveChanges();
                        return PartialView("_Success", "Schedule has been updated");
                    }
                    catch(Exception e)
                    {
                        return PartialView("_Error", e.Message);
                    }
                }

                List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                return PartialView("_ValidationError", errors);
        }
    }
}
