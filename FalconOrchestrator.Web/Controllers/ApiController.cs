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
using System.Globalization;
using FalconOrchestratorWeb.Models.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FalconOrchestrator.LDAP;
using FalconOrchestrator.DAL;


namespace FalconOrchestratorWeb.Controllers
{
    public class ApiController : BaseController
    {
        public ActionResult DataTableHandler(JQueryDataTablesModel parameters)
        {
            FalconOrchestratorDB db = new FalconOrchestratorDB();
            List<Detection> model = HttpContext.Application["Detections"] as List<Detection>;

            if (!String.IsNullOrEmpty(parameters.sSearch))
            {
                model = model.Where(x => x.DetectionDevice.Device.Hostname.Contains(parameters.sSearch.ToLower())
                                                       || x.Account.AccountName.ToLower().Contains(parameters.sSearch.ToLower())
                                                       || x.Name.ToLower().Contains(parameters.sSearch.ToLower())
                                                       || x.FileName.ToLower().Contains(parameters.sSearch.ToLower())).ToList();
            }

            int sortingColumnIndex = parameters.iSortCol_0;
            string sortDirection = parameters.sSortDir_0;

            Func<Detection, string> orderingFunction = (x => sortingColumnIndex == 0 ? x.ProcessStartTime.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture) :
                                                                   sortingColumnIndex == 1 ? x.Severity.SeverityType :
                                                                   sortingColumnIndex == 2 ? x.Name :
                                                                   sortingColumnIndex == 3 ? x.DetectionDevice.Device.Hostname :
                                                                   sortingColumnIndex == 4 ? x.Account.AccountName :
                                                                   sortingColumnIndex == 5 ? x.FileName :
                                                                   sortingColumnIndex == 6 ? x.Status.StatusType :
                                                                   x.Responder != null ? x.Responder.FirstName + " " + x.Responder.LastName : null);

            if (sortDirection == "asc")
                model = model.OrderBy(orderingFunction).ToList();
            if (sortDirection == "desc")
                model = model.OrderByDescending(orderingFunction).ToList();

            var filteredModel = model.Skip(parameters.iDisplayStart)
                                 .Take(parameters.iDisplayLength).ToList();

            var data = filteredModel.Select(x => new DetectionListViewModel
            {
                DT_RowId = x.DetectionId,
                DetectionName = x.Name,
                FileName = x.FileName,
                Responder = x.Responder!= null ? x.Responder.FirstName + " " + x.Responder.LastName : null,
                Hostname = x.DetectionDevice.Device.Domain != x.DetectionDevice.Device.Hostname ? x.DetectionDevice.Device.Domain + "\\" + x.DetectionDevice.Device.Hostname : x.DetectionDevice.Device.Hostname,
                Severity = x.Severity.SeverityType,
                Status = x.Status.StatusType,
                Timestamp = x.ProcessStartTime.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                Username = x.Account.AccountName
            });

            return Json(new
            {
                sEcho = parameters.sEcho,
                iTotalRecords = model.Count,
                iTotalDisplayRecords = model.Count,
                aaData = data
            },
                JsonRequestBehavior.AllowGet);                        
        }

        [HttpPost]
        public ActionResult Containment(string username, Boolean pwdReset, Boolean disableAccount)
        {
            try
            {
                AppConfiguration config = new AppConfiguration(System.Configuration.ConfigurationManager.AppSettings["CryptoKey"]);
                LdapUtil util = new LdapUtil(config.LDAP_SERVER, config.LDAP_USERNAME, config.LDAP_PASSWORD);
                UserManager user = new UserManager(util, username);
                if (pwdReset)
                    user.EnforcePasswordReset();
                if (disableAccount)
                    user.DisableAccount(config.LDAP_DESCRIPTION);
                Response.StatusCode = 200;
                return new EmptyResult();
            }
            catch(Exception e)
            {
                Response.StatusCode = 500;
                return Content(e.Message);
            }

        }

        public ActionResult Refresh()
        {
            RefreshDetections();
            return RedirectToAction("index", "detection");
        }
    }

    public class JsonDotNetResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };

        public override void ExecuteResult(ControllerContext context)
        {
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("GET request not allowed");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data == null)
            {
                return;
            }

            response.Write(JsonConvert.SerializeObject(this.Data, Settings));
        }
    }
}
