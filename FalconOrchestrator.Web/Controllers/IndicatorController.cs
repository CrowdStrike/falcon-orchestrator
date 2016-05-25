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
using System.Net;
using FalconOrchestratorWeb.Models.ViewModels;
using FalconOrchestrator.IOC;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Controllers
{
    public class IndicatorController : Controller
    {
        private static readonly AppConfiguration config = new AppConfiguration(System.Configuration.ConfigurationManager.AppSettings["CryptoKey"]);

        private readonly ApiUtil util = new ApiUtil(config.FALCON_QUERY_USERNAME, config.FALCON_QUERY_PASSWORD, config.FALCON_QUERY_URL);

        [HttpGet]
        public ActionResult Index()
        {
            IndicatorsAPI api = new IndicatorsAPI(util);
            return View(api.List());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new IndicatorViewModel { ExpirationDays = 30 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IndicatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IndicatorsAPI api = new IndicatorsAPI(util);
                    List<Indicator> data = new List<Indicator>();

                    Indicator ioc = new Indicator();
                    ioc.Description = model.Description;
                    ioc.Policy = model.Policy;
                    ioc.Source = model.Source;
                    ioc.Type = model.Type;
                    ioc.Value = model.Value;
                    ioc.ExpirationDays = model.ExpirationDays;

                    data.Add(ioc);
                    api.Upload(data);
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
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                IndicatorsAPI api = new IndicatorsAPI(util);
                api.Delete(id);
                Response.StatusCode = 200;
                return new EmptyResult();
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Content(e.Message);
            }

        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            IndicatorsAPI api = new IndicatorsAPI(util);
            Indicator ioc = api.Get(id);

            IndicatorViewModel model = new IndicatorViewModel();
            model.Description = ioc.Description;
            model.Policy = ioc.Policy;
            model.ShareLevel = ioc.ShareLevel;
            model.Source = ioc.Source;
            model.Type = ioc.Type;
            model.Value = ioc.Value;

            TimeSpan? diff = ioc.ExpirationTimestamp - ioc.CreatedTimestamp;
            model.ExpirationDays = diff.Value.Days + 1;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IndicatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IndicatorsAPI api = new IndicatorsAPI(util);
                
                    Indicator ioc = new Indicator();
                    ioc.Description = model.Description;
                    ioc.Policy = model.Policy;
                    ioc.Source = model.Source;
                    ioc.ExpirationDays = model.ExpirationDays;
                    api.Update(ioc, model.Type + ":" + model.Value);
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    ModelState.AddModelError(String.Empty, e.Message);
                }
            }
            return View(model);

        }
    }
}

