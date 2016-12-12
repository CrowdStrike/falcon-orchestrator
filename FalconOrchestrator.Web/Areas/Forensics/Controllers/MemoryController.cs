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
using System.Web.Mvc;
using FalconOrchestrator.Forensics;
using FalconOrchestratorWeb.Controllers;
using System.Collections.Generic;
using System.Net;

namespace FalconOrchestratorWeb.Areas.Forensics.Controllers
{
    public class MemoryController : BaseController
    {

        [HttpPost]
        public ActionResult ProcessDump(string computerName, string pid)
        {
            try
            {
                PSRemoting ps = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                Memory memory = new Memory(ps);
                string command = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Scripts/Out-minidump.ps1"));
                Dictionary<string, string> mapping = new Dictionary<string, string>();
                mapping.Add("{{ProcessId}}", pid);
                command = PSRemoting.CommandMapping(command, mapping);
                memory.ProcessDump(command);
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
