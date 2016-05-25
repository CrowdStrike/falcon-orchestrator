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
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Models.ViewModels
{
    public class TicketViewModel : IValidatableObject
    {

        [Required(ErrorMessage = "A ticket recipient must be selected")]
        public int TicketRecipientId { get; set; }

        [Required]
        public string Creator { get; set; }
        public int TicketId { get; set; }
        public string DetectionId { get; set; }
        public string DispatchDate { get; set; }
        public string CompletionDate { get; set; }
        public string ExternalTicket { get; set; }
        public string Comment { get; set; }
        [Required(ErrorMessage = "A ticket severity must be selected")]
        public int SeverityId { get; set; }
        public Asset Asset { get; set; }

        public bool SendNotification { get; set; }
        public bool NotifyUser { get; set; }
        public bool NotifyManager { get; set; }

        public int AssociatedEventCount { get; set; }

        public string DaysOpen
        {
            get
            {
                if (this.DispatchDate != null && this.CompletionDate != null)
                {
                    return String.Format("{0:0.00}", this.CompareDispatchDateToCloseDate(this.DispatchDate, this.CompletionDate));
                }

                if (this.DispatchDate != null && this.CompletionDate == null)
                {
                    return String.Format("{0:0.00}", this.CompareDispatchDateToNow(this.DispatchDate));
                }

                return null;
            }
        }

        public string Status {
            get{

                if(this.DispatchDate != null && this.CompletionDate != null)
                {
                    return "CLOSED";
                }
                if(this.DispatchDate != null && this.CompletionDate == null)
                {
                    return "PENDING";
                }
                if(this.DispatchDate == null)
                {
                    return "NOT DISPATCHED";
                }
                return null;
            }
        }


        public SelectList RecipientList
        {
            get
            {
                FalconOrchestratorDB db = new FalconOrchestratorDB();
                return new SelectList(db.TicketRecipients, "TicketRecipientId", "Title", this.TicketRecipientId);
            }
        
        }

        public SelectList SeverityList
        {
            get
            {
                FalconOrchestratorDB db = new FalconOrchestratorDB();
                return new SelectList(db.Severities.ToList(), "SeverityId", "SeverityType", this.SeverityId);
            }
        }

        private double CompareDispatchDateToNow(string dispatchDate)
        {
            DateTime dispatch = Convert.ToDateTime(dispatchDate).ToUniversalTime();
            return (DateTime.UtcNow - dispatch).TotalDays;

        }

        private double CompareDispatchDateToCloseDate(string dispatchDate, string closeDate)
        {
            DateTime dispatch = Convert.ToDateTime(dispatchDate).ToUniversalTime();
            DateTime close = Convert.ToDateTime(closeDate).ToUniversalTime();
            return (close - dispatch).TotalDays;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(this.CompletionDate != null && this.DispatchDate == null)
            {
                yield return new ValidationResult("Dispatch date cannot be empty if completion date is set", new[] { "DispatchDate" });
            }

            if(this.CompletionDate != null && (Convert.ToDateTime(this.CompletionDate) < Convert.ToDateTime(this.DispatchDate)))
            {
                yield return new ValidationResult("Completion date cannot be before dispatch date", new[] { "CompletionDate" });
            }

            if((this.NotifyUser == true || this.NotifyManager == true) && this.SendNotification == false)
            {
                yield return new ValidationResult("Notify recipient must be enabled if notify user or manager is enabled", new[] { "SendNotification" });
            }

            if (this.DispatchDate == null && this.SendNotification != false)
            {
                yield return new ValidationResult("Must set dispatch date when sending notification", new[] { "DispatchDate" });
            }

            if (this.NotifyManager == true && this.Asset.Manager == null)
            {
                yield return new ValidationResult("Manager email address must provided in order to include in notification", new[] { "Manager" });
            }

        }
    }



}