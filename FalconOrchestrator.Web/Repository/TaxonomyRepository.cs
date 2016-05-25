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
using System.Text.RegularExpressions;
using FalconOrchestratorWeb.Models.ViewModels;
using FalconOrchestrator.LDAP;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Repository
{
    public class TaxonomyRepository
    {
        public List<TaxonomyViewModel> GetList()
        {
            List<TaxonomyViewModel> result = new List<TaxonomyViewModel>();
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                foreach (Taxonomy line in db.Taxonomies.ToList())
                {
                    TaxonomyViewModel model = new TaxonomyViewModel();
                    model.Creator = line.Creator;
                    model.Critical = line.Critical;
                    model.Description = line.Description;
                    model.TaxonomyId = line.TaxonomyId;
                    model.Timestamp = line.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    model.Type = line.TaxonomyType.Type;
                    model.TypeId = line.TaxTypeId;
                    model.Value = line.Value;
                    result.Add(model);
                }
            }
            return result;
        }

        public void Create(TaxonomyViewModel model)
        {
            CheckADObjectExists(model);
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Taxonomy rule = new Taxonomy();
                rule.Value = model.Value;
                rule.Timestamp = DateTime.UtcNow;
                rule.TaxTypeId = model.TypeId;
                rule.Description = model.Description;
                rule.Critical = model.Critical;
                rule.Creator = model.Creator;
                db.Taxonomies.Add(rule);
                db.SaveChanges();
                ApplyRetoractively(db, model, rule.TaxonomyId);
            }

        }

        public void Delete(int? id)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<DetectionTaxonomy> matches = db.DetectionTaxonomies.Where(x => x.TaxonomyId == id).ToList();
                foreach(DetectionTaxonomy dt in matches)
                {
                    db.DetectionTaxonomies.Remove(dt);
                }
                db.SaveChanges();

                Taxonomy rule = new Taxonomy() { TaxonomyId = (int)id };
                db.Taxonomies.Attach(rule);
                db.Taxonomies.Remove(rule);
                db.SaveChanges();
            }
        }

        public TaxonomyViewModel Get(int? id)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Taxonomy rule = db.Taxonomies.Find(id);
                TaxonomyViewModel model = new TaxonomyViewModel();
                model.TaxonomyId = rule.TaxonomyId;
                model.Creator = rule.Creator;
                model.Critical = rule.Critical;
                model.Description = rule.Description;
                model.Type = rule.TaxonomyType.Type;
                model.TypeId = rule.TaxTypeId;
                model.Value = rule.Value;
                return model;
            }
        }

        public void Update(TaxonomyViewModel model)
        {
            CheckADObjectExists(model);
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Taxonomy rule = db.Taxonomies.Find(model.TaxonomyId);
                rule.Description = model.Description;
                db.SaveChanges();
            }
        }

        private void ApplyRetoractively(FalconOrchestratorDB db, TaxonomyViewModel model, int taxonomyId)
        {
            List<Detection> detections = db.Detections.Include("Account").Include("DetectionDevice").ToList();

            switch(model.TypeId){
                case 1:
                    detections = detections.Where(x => Regex.IsMatch(x.Account.AccountName, Regex.Replace(model.Value, @"\\", @"\\"))).ToList();
                    break;
                case 2:
                    detections = detections.Where(x => Regex.IsMatch(x.DetectionDevice.Device.Hostname, Regex.Replace(model.Value, @"\\", @"\\"))).ToList();
                    break;
                case 3:
                    detections = detections.Where(x => x.Account.OrganizationalUnit != null && x.Account.OrganizationalUnit.Equals(Regex.Replace(model.Value, @"\\", @"\\"))).ToList();
                    break;
                case 4:
                    var accountGroups = db.AccountGroups.Where(x => x.Group.Name == Regex.Replace(model.Value, @"\\", @"\\")).ToList();
                    var accountIds = accountGroups.Select(x => x.AccountId).ToList();
                    detections = db.Detections.Where(x => accountIds.Contains(x.AccountId)).ToList();    
                   break;
            }

            foreach(Detection line in detections)
            {
                DetectionTaxonomy dt = new DetectionTaxonomy();
                dt.DetectionId = line.DetectionId;
                dt.TaxonomyId = taxonomyId;
                db.DetectionTaxonomies.Add(dt);

                if(model.Critical)
                {
                    line.CustomSeverityId = 5;
                }
            }
            db.SaveChanges();
            
        }

        private void CheckADObjectExists(TaxonomyViewModel model)
        {         
            if(model.TypeId == 3 || model.TypeId == 4)
            {
                AppConfiguration config = new AppConfiguration(System.Configuration.ConfigurationManager.AppSettings["CryptoKey"]);
                LdapUtil ldap = new LdapUtil(config.LDAP_SERVER, config.LDAP_USERNAME, config.LDAP_PASSWORD);
                Boolean exists = ldap.CheckIfDNExists(model.Value);
                if (!exists)
                    throw new NotSupportedException(model.Value + " does not exist in Active Directory database");
            }
        }
    }
}