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
    public class ProcessesController : BaseController
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
                ProcessManagement process = new ProcessManagement(ps);           
                string command = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Scripts/Get-ProcessListing.ps1"));
                List<Process> model = process.ListProcesses(command);
                ViewBag.ComputerName = viewModel.ComputerName;
                return PartialView("_ProcessListing", model);
            }
            catch (Exception e)
            {
                return PartialView("_Error", e.Message);
            }
        }

        [HttpPost]
        public ActionResult Kill(string computerName, string pid)
        {
            try
            {
                PSRemoting ps = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                ProcessManagement process = new ProcessManagement(ps);
                process.Kill(Convert.ToInt32(pid));
                Response.StatusCode = (int)HttpStatusCode.OK;
                return new EmptyResult();

            }
            catch (Exception e)
            {
                //Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //return Content(e.Message);
                throw new HttpException(500, e.Message);
            }
        }

    }
}
