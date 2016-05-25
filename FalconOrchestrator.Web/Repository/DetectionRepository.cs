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
using AutoMapper;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Repository
{
    public class DetectionRepository
    {
        public List<Detection> GetList()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Detections.ToList();
            }
        }

        public DetectionEditViewModel Get(int? id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Detection, DetectionEditViewModel>()
                    .ForMember(x => x.Handler, y => y.MapFrom(v => v.Responder != null ? v.Responder.FirstName + " " + v.Responder.LastName : "Unassigned"))
                    .ForMember(x => x.DetectId, y => y.MapFrom(v => v.FalconHostLink.Split('/')[4]))
                    .ForMember(x => x.Severity, y => y.MapFrom(v => v.Severity.SeverityType))
                    .ForMember(x => x.Status, y => y.MapFrom(v => v.Status.StatusType))
                    .ForMember(x => x.StartTime, y => y.MapFrom(v => v.ProcessStartTime.HasValue ? v.ProcessStartTime.Value.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture) : null))
                    .ForMember(x => x.StopTime, y => y.MapFrom(v => v.ProcessEndTime.HasValue ? v.ProcessEndTime.Value.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture) : null))
                    .ForMember(x => x.ClosedDate, y => y.MapFrom(v => v.ClosedDate.HasValue ? v.ClosedDate.Value.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture) : null))
                    .ForMember(x => x.Asset, y => y.MapFrom(v => new Asset
                    {
                        AccountName = v.Account.AccountName,
                        JobTitle = v.Account.JobTitle,
                        Department = v.Account.Department,
                        City = v.Account.City,
                        Country = v.Account.Country,
                        EmailAddress = v.Account.EmailAddress,
                        IPAddress =  v.DetectionDevice.IPAddress,
                        FirstName = v.Account.FirstName,
                        LastName = v.Account.LastName,
                        Manager = v.Account.Manager,
                        PhoneNumber = v.Account.PhoneNumber,
                        StateProvince = v.Account.StateProvince,
                        StreetAddress = v.Account.StreetAddress,
                        OrganizationalUnit = v.Account.OrganizationalUnit,
                        LastLogonTime = v.Account.LastLogon != null ? v.Account.LastLogon.Value.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss" ,CultureInfo.InvariantCulture) : null,
                        Hostname = v.DetectionDevice.Device.Hostname,
                        Domain = v.DetectionDevice.Device.Domain
                    }));
            });

            using(FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                Detection data = db.Detections.Find(id);
                
                //if provided detection id does not exist return null immediately 
                if (data == null)
                    return null;

                DetectionEditViewModel model = config.CreateMapper().Map<Detection, DetectionEditViewModel>(data);

                //SHOULD BE REFACTORED TO USE AUTO MAPPER APPROACH

                //Add tags to view model
                List<string> tags = db.DetectionTags.Where(x => x.DetectionId == id).Select(y => y.Tag.Name).ToList();
                foreach(string tag in tags)
                {
                    model.Tags += tag + ",";
                }

                //Get Groups Memeberships
                model.Asset.GroupMemberships = db.AccountGroups.Where(x => x.AccountId == data.AccountId).Select(y => y.Group.Name).ToList();


                //Get associated event count
                model.AssociatedEventCount = db.Detections.Where(x => x.FalconHostLink == data.FalconHostLink).Count() - 1;

                //Check if any taxonomies are applies
                model.TaxonomyCount = db.DetectionTaxonomies.Where(x => x.DetectionId == id).Count();
                if(model.TaxonomyCount > 0)
                    model.TaxonomyList = db.DetectionTaxonomies.Where(x => x.DetectionId == id).Select(y => y.Taxonomy.Description).ToList();

                return model;
            }
        }

        public void Update(DetectionEditViewModel model)
        {
            using(FalconOrchestratorDB db = new FalconOrchestratorDB())
            {

                if (model.Tags != null)
                {
                    int tagId;
                    foreach (string line in model.Tags.Split(','))
                    {
                        //if tag already exists, get ID
                        if (db.Tags.Any(x => x.Name.Equals(line.ToLower())))
                        {
                            tagId = db.Tags.Where(x => x.Name.Equals(line.ToLower())).Select(x => x.TagId).FirstOrDefault();
                        }
                        //otherwise create a new tag and return that ID
                        else
                        {
                            Tag tag = new Tag();
                            tag.Name = line.ToLower();
                            db.Tags.Add(tag);
                            db.SaveChanges();
                            tagId = tag.TagId;
                        }

                        //Associate the tag with the detection
                        DetectionTag dt = new DetectionTag();
                        dt.DetectionId = model.DetectionId;
                        dt.TagId = tagId;
                        db.DetectionTags.Add(dt);
                        db.SaveChanges();
                    }
                }
                
                Detection detection = db.Detections.Find(model.DetectionId);
                detection.CustomSeverityId = model.CustomSeverityId;
                detection.ResponderId = model.ResponderId;
                detection.Comment = model.Comment;  
                
                //If status in DB is currently of type open but view model is of type closed, set closed date to current time
                if((detection.StatusId.Equals(1) || detection.StatusId.Equals(2) || detection.StatusId.Equals(3) || detection.StatusId.Equals(4)) 
                    && model.StatusId.Equals(5) || model.StatusId.Equals(6) || model.StatusId.Equals(7))
                {
                    detection.ClosedDate = DateTime.UtcNow;
                }

                //If status in DB is currently of type closed but view model is of type open, set closed date to null
                if ((detection.StatusId.Equals(5) || detection.StatusId.Equals(6) || detection.StatusId.Equals(7))
                     && model.StatusId.Equals(1) || model.StatusId.Equals(2) || model.StatusId.Equals(3) || detection.StatusId.Equals(4))
                {
                    detection.ClosedDate = null;
                }

                //order matters, set status after closed date handling logic
                detection.StatusId = model.StatusId;
                db.Entry(detection).State = System.Data.Entity.EntityState.Modified;

                //if IP manually provided update db record with the new IP
                if(model.Asset.IPAddress !=null)
                {
                    DetectionDevice dd = db.DetectionDevices.Find(detection.DetectionDeviceId);
                    dd.IPAddress = model.Asset.IPAddress;
                    db.Entry(dd).State = System.Data.Entity.EntityState.Modified;
                }

                //if IP already exists and user supplies empty value update with null value
                if(detection.DetectionDevice.IPAddress != null && model.Asset.IPAddress == null)
                {
                    DetectionDevice dd = db.DetectionDevices.Find(detection.DetectionDeviceId);
                    dd.IPAddress = null;
                    db.Entry(dd).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }

        }

    }

}