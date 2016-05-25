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
using System.Globalization;
using FalconOrchestratorWeb.Models.ViewModels;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Repository
{
    public class WhitelistingRepository
    {
        public List<WhitelistingViewModel> GetList()
        {
            List<WhitelistingViewModel> result = new List<WhitelistingViewModel>();
            using(FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                foreach(Whitelist line in db.Whitelists.ToList())
                {
                    WhitelistingViewModel model = new WhitelistingViewModel();
                    model.WhitelistId = line.WhitelistId;
                    model.Creator = line.Creator;
                    model.Reason = line.Reason;
                    model.Timestamp = line.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    model.Type = line.WhitelistType.Type;
                    model.Value = line.Value;
                    result.Add(model);
                }
            }
            return result;
        }

        public void Create(WhitelistingViewModel model)
        {
            using(FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Whitelist rule = new Whitelist();
                rule.Reason = model.Reason;
                rule.Creator = model.Creator;
                rule.Timestamp = DateTime.UtcNow;
                rule.Value = model.Value;
                rule.WhitelistTypeId = model.TypeId;
                db.Whitelists.Add(rule);
                db.SaveChanges();
                ApplyRetroactively(db, model);
            }

        }

        public void Delete(int? whitelistId)
        {
            using(FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Whitelist rule = new Whitelist() { WhitelistId = (int)whitelistId };
                db.Whitelists.Attach(rule);
                db.Whitelists.Remove(rule);
                db.SaveChanges();
            }
        }

        public WhitelistingViewModel Get(int? whitelistId)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Whitelist rule = db.Whitelists.Find(whitelistId);
                WhitelistingViewModel model = new WhitelistingViewModel();
                model.WhitelistId = rule.WhitelistId;
                model.Value = rule.Value;
                model.Type = rule.WhitelistType.Type;
                model.TypeId = rule.WhitelistTypeId;
                model.Reason = rule.Reason;
                return model;
            }
        }

        public void Update(WhitelistingViewModel model)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Whitelist rule = db.Whitelists.Find(model.WhitelistId);
                rule.Creator = model.Creator;
                rule.Reason = model.Reason;
                rule.Value = model.Value;
                rule.WhitelistTypeId = model.TypeId;
                db.SaveChanges();
            }
        }

        private void ApplyRetroactively(FalconOrchestratorDB db, WhitelistingViewModel model)
        {
            
            string name = db.WhitelistTypes.Where(x => x.WhitelistTypeId == model.TypeId).Select(x => x.Type).FirstOrDefault();
            string prop = name.Replace(" ", String.Empty);

            System.Reflection.PropertyInfo property = typeof(Detection).GetProperty(prop);
            List<Detection> matchingDetects = db.Detections.ToList().Where(x => System.Text.RegularExpressions.Regex.IsMatch(property.GetValue(x).ToString(), System.Text.RegularExpressions.Regex.Replace(model.Value,@"\\",@"\\"))).ToList();
            foreach(Detection line in matchingDetects)
            {
                line.StatusId = 7;
                line.ClosedDate = DateTime.UtcNow;
            }
            db.SaveChanges();
        }

    }
}