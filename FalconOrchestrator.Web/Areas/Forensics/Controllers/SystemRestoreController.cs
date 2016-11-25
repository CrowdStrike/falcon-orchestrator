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
using System.Web.Mvc;
using FalconOrchestratorWeb.Controllers;
using FalconOrchestrator.Forensics;
using FalconOrchestratorWeb.Areas.Forensics.Models;


namespace FalconOrchestratorWeb.Areas.Forensics.Controllers
{
    public class SystemRestoreController : BaseController
    {
        private static List<SystemRestorePoint> points;

        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult RestorePoints(AssetViewModel viewModel)
        {
            try
            {
                PSRemoting psr = new PSRemoting(viewModel.ComputerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                SystemRestore restore = new SystemRestore(psr);
                points = restore.GetRestorePoints();
                return PartialView("_RestorePoints", points);
            }
            catch (Exception e)
            {
                return PartialView("_Error", e.Message);
            }
        }

        public PartialViewResult MountShadowCopy(string computerName, string timestamp)
        {
            try
            {
                PSRemoting psr = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                SystemRestore restore = new SystemRestore(psr);
                ShadowCopy copy = restore.GetShadowCopyByTime(Convert.ToDateTime(timestamp));
                restore.MountShadowCopyVolume(copy.DeviceObject + @"\");
                return BrowserRefresh(computerName, @"C:\shadow", copy.DeviceObject);

            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return PartialView("_Error", e.Message);
            }
        }

        public PartialViewResult BrowserRefresh(string computerName, string path, string deviceObject)
        {
            try
            {
                PSRemoting psr = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                FileSystemBrowser browser = new FileSystemBrowser(psr);
                List<FileMetadata> model = browser.GetDirectoryContent(path);
                ViewBag.DeviceObject = deviceObject.Replace("\\", "\\\\");
                ViewBag.Path = path;
                ViewBag.ComputerName = computerName;
                return PartialView("~/Areas/Forensics/Views/SystemRestore/_ShadowCopyData.cshtml", model);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return PartialView("_Error", e.Message);
            }
        }

        public PartialViewResult RecoverFile(string computerName, string source, string attributes)
        {
            try
            {
                PSRemoting psr = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                FileSystemBrowser browser = new FileSystemBrowser(psr);
                string destination = source.Replace(@"\shadow", "");

                if (attributes.Contains("Directory"))
                    source += @"\*";
                browser.CopyFile(source, destination);
                return PartialView("_Success", source + " has been successfully recovered");
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return PartialView("_Error", e.Message);
            }
        }

        public PartialViewResult ExecuteRestore(string computerName, string sequenceNumber)
        {
            try
            {
                PSRemoting psr = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                SystemRestore restore = new SystemRestore(psr);
                restore.RestoreSystem(sequenceNumber);
                return PartialView("_Success", "Restore has been successfully initialized on host " + computerName);
            }
            catch (Exception e)
            {
                return PartialView("_Error", e.Message);
            }
        }

    }
}
