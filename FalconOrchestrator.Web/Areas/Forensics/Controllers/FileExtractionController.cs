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
using System.Web;
using System.Web.Mvc;
using System.IO;
using Ionic.Zip;
using FalconOrchestratorWeb.Controllers;
using FalconOrchestratorWeb.Areas.Forensics.Models;
using FalconOrchestrator.Forensics;

namespace FalconOrchestratorWeb.Areas.Forensics.Controllers
{

    public class FileExtractionController : BaseController
    {
        public ActionResult Index()
        {
            return View(new FileExtractionViewModel());
        }

        [HttpPost]
        public ActionResult Receiver(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                CompressAndEncrypt(file);
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult DownloadFile(string fileName)
        {
            string path = Path.Combine(Server.MapPath("~/App_Data/Artifacts/" + fileName + ".zip"));
            FileInfo file = new FileInfo(path);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".zip");
            Response.AddHeader("Content-Length", file.Length.ToString());
            return File(file.FullName, System.Net.Mime.MediaTypeNames.Application.Octet);
        }

        //Invoke controllers need refactoring to remain DRY
        [HttpPost]
        public ActionResult Invoke(FileExtractionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string protocol = "\"http://";
                    if (Request.IsSecureConnection)
                        protocol = "\"https://";
                    string url = protocol + Request.Headers["Host"] + "/forensics/file-extraction/receiver\"";
                    PSRemoting ps = new PSRemoting(viewModel.ComputerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                    FileExtraction extraction = new FileExtraction(ps);

                    Dictionary<string, string> mapper = new Dictionary<string, string>();
                    mapper.Add("{{FilePath}}", "\"" + viewModel.FilePath + "\"");
                    mapper.Add("{{URL}}", url);

                    var model = GetFileMetadata(mapper, extraction);
                    this.UploadFile(mapper, extraction);
                    ViewBag.FileName = viewModel.FilePath.Split('\\').Last();
                    return PartialView("~/Areas/Forensics/Views/FileExtraction/_DownloadLink.cshtml", model);

                }
                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                }
        }
            List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
            return PartialView("_ValidationError", errors);          
        }

        [HttpGet]
        public ActionResult InvokeFromDetection(string computerName, string filePath)
        {
            try
            {
                string protocol = "\"http://";
                if (Request.IsSecureConnection)
                    protocol = "\"https://";
                string url = protocol + Request.Headers["Host"] + "/forensics/file-extraction/receiver\"";

                PSRemoting ps = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                FileExtraction extraction = new FileExtraction(ps);


                ////When submitted from a detection edit view, you need to convert the full device path to the appropriate drive letter first
                string command = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Scripts/Get-DevicePath.ps1"));
                Dictionary<string, string> map = extraction.GetDevicePaths(command);
                string driveLetter = map[string.Join("\\", filePath.Split('\\').Take(3).ToArray())];
                string path = driveLetter + "\\" + string.Join("\\", filePath.Split('\\').Skip(3).ToArray());

                Dictionary<string, string> mapper = new Dictionary<string, string>();
                mapper.Add("{{FilePath}}", "\"" + path + "\"");
                mapper.Add("{{URL}}", url);

                this.UploadFile(mapper, extraction);
                return new EmptyResult();
            }
            catch(Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                return PartialView("_ValidationError", errors);
            }
            
        }

        public ActionResult InvokeFromBrowser(string computerName, string filePath)
        {
            try
            {
                string protocol = "\"http://";
                if (Request.IsSecureConnection)
                    protocol = "\"https://";
                string url = protocol + Request.Headers["Host"] + "/forensics/file-extraction/receiver\"";
                PSRemoting ps = new PSRemoting(computerName, config.FALCON_FORENSICS_USERNAME, config.FALCON_FORENSICS_PASSWORD, config.FALCON_FORENSICS_DOMAIN);
                FileExtraction extraction = new FileExtraction(ps);

                Dictionary<string, string> mapper = new Dictionary<string, string>();
                mapper.Add("{{FilePath}}", "\"" + filePath + "\"");
                mapper.Add("{{URL}}", url);

                this.UploadFile(mapper, extraction);
                return new EmptyResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                List<string> errors = ModelState.Where(x => x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                return PartialView("_ValidationError", errors);
            }
        }

        private void UploadFile(Dictionary<string, string> mapping, FileExtraction extraction)
        {
            string command = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Scripts/Get-RemoteFile.ps1"));
            command = PSRemoting.CommandMapping(command, mapping);
            extraction.UploadFile(command);
        }

        private FileMetadata GetFileMetadata(Dictionary<string, string> mapping, FileExtraction extraction)
        {
            string command = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/Scripts/Get-FileAttributes.ps1"));
            command = PSRemoting.CommandMapping(command, mapping);                     
            return extraction.GetFileMetadata(command);
        }

        private void CompressAndEncrypt(HttpPostedFileBase file)
        {
            //compress, encrypt, password protect file
            using (ZipFile zip = new ZipFile())
            {
                 zip.Password = config.FALCON_FORENSICS_ENCRYPTION_PASSWORD;
                 zip.AddEntry(file.FileName, file.InputStream);
                 string path = Path.Combine(Server.MapPath("~/App_Data/Artifacts/"), file.FileName + ".zip");
                 zip.Save(path);
            }
        }
    }
}
