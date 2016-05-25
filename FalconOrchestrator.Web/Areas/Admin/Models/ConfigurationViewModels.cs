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
using System.ComponentModel.DataAnnotations;


namespace FalconOrchestratorWeb.Areas.Admin.Models
{
    public class ConfigurationItemsViewModel
    {
        public LDAPConfigViewModel LDAP { get; set; }
        public FalconStreamingAPIConfigViewModel FalconStream { get; set; }
        public FalconForensicsConfigViewModel FalconForensics { get; set; }
        public EmailConfigViewModel Email { get; set; }
        public FalconQueryAPIConfigViewModel FalconQuery { get; set; }
        public ETLRulesViewModel ETLRules { get; set; }
    }


    public class LDAPConfigViewModel
    {
        [Required(ErrorMessage = "A server hostname or IP address is required")]
        public string Server { get; set; }
        [Required(ErrorMessage = "A Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "A Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "A Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Days valid is required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Days valid must be a number")]
        public int DaysValid { get; set; }
    }

    public class ETLRulesViewModel : IValidatableObject
    {
        public Boolean Whitelisting { get; set; }
        public Boolean ADLookup { get; set; }
        public Boolean Taxonomize { get; set; }
        public Boolean DNSLookup { get; set; }
        public Boolean AssignResponder{ get; set; }
        public Boolean Notification { get; set; }
        public string NotificationThreshold { get; set; }
        public List<SelectListItem> SeverityList
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = "Informational", Value = "1" });
                items.Add(new SelectListItem() { Text = "Low", Value = "2" });
                items.Add(new SelectListItem() { Text = "Medium", Value = "3" });
                items.Add(new SelectListItem() { Text = "High", Value = "4" });
                items.Add(new SelectListItem() { Text = "Critical", Value = "5" });
                return items;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Notification == true && this.NotificationThreshold == null)
            {
                yield return new ValidationResult("A notification threshold must be defined", new[] { "NotificationThreshold" });
            }
        }
    }

    public class FalconStreamingAPIConfigViewModel
    {
        [Url(ErrorMessage = "Not a valid URL format")]
        [Required(ErrorMessage = "API URL is required")]
        public string Url { get; set; }
        [Required(ErrorMessage = "API Username is required")]
        public string Uuid { get; set; }
        [Required(ErrorMessage = "API Password required")]
        public string Key { get; set; }

        public string Offset { get; set; }
    }

    public class FalconQueryAPIConfigViewModel
    {
        [Url(ErrorMessage = "Not a valid URL format")]
        [Required(ErrorMessage = "API URL is required")]
        public string Url { get; set; }
        [Required(ErrorMessage = "API Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "API Password required")]
        public string Password { get; set; }
    }


    public class FalconForensicsConfigViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        public string Domain { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Zip password is required")]
        public string ZipPassword { get; set; }
    }

    public class EmailConfigViewModel
    {
        [Required(ErrorMessage = "Server Hostname or IP is required")]
        public string Server { get; set; }

        [Required(ErrorMessage = "Port  is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Port must be numeric")]
        public string Port { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Alert subject is required")]
        public string AlertSubject { get; set; }

        [Required(ErrorMessage = "Alert template path is required")]
        public string AlertTemplatePath { get; set; }

        [Required(ErrorMessage = "Ticket subject is required")]
        public string TicketSubject { get; set; }

        [Required(ErrorMessage = "Ticket template path is required")]
        public string TicketTemplatePath { get; set; }

        [Required(ErrorMessage = "SSL is required")]
        public Boolean SSL { get; set; }

        [EmailAddress(ErrorMessage="Not a valid email address format")]
        [Required(ErrorMessage = "Sender email address is required")]
        public string SenderAddress { get; set; }

        [Required(ErrorMessage = "Team email address is required")]
        public string TeamAddress { get; set; }

        [Required(ErrorMessage = "Home URL is required")]
        public string HomeURL { get; set; }

    }


}