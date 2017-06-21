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
using System.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net;
using FalconOrchestratorWeb.Areas.Forensics.Models;
using FalconOrchestrator.Forensics;
using FalconOrchestratorWeb.Controllers;

namespace FalconOrchestratorWeb.Areas.Forensics.Controllers
{
    public class ScheduledController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(AssetViewModel viewModel)
        {
            try
            {
                PSRemoting ps = new PSRemoting(viewModel.ComputerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                TaskManagement tasks = new TaskManagement(ps);
                string command = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Scripts/Get-ScheduledTask.ps1"));
                List<Tasks> model = tasks.ListTasks(command);
                ViewBag.ComputerName = viewModel.ComputerName;
                return PartialView("_ScheduleTasks", model);
            }
            catch (Exception e)
            {
                return PartialView("_Error", e.Message);
            }
        }

        [HttpPost]
        public ActionResult Kill(string computerName, string path) 
        {
            try
            {
                PSRemoting ps = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                TaskManagement tasks = new TaskManagement(ps);
                tasks.Kill(path);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return new EmptyResult();
            }
            catch (Exception e)
            {
                throw new HttpException(500, e.Message);
            }
        }

    }
}
