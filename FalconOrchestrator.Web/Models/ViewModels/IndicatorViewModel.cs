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
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace FalconOrchestratorWeb.Models.ViewModels
{
    public class IndicatorViewModel : IValidatableObject
    {
        [Required]
        public String Type { get; set; }
        [Required]
        public String Value { get; set; }
        public String Description { get; set; }
        public String ShareLevel { get; set; }
        public String Source { get; set; }
        [Required]
        public String Policy { get; set; }
        public DateTime? CreatedTimestamp { get; set; }
        public DateTime? ExpirationTimestamp { get; set; }
        public int ExpirationDays { get; set; }
        public List<SelectListItem> TypeList
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = "DOMAIN", Value = "DOMAIN" });
                items.Add(new SelectListItem() { Text = "SHA256", Value = "SHA256" });
                items.Add(new SelectListItem() { Text = "SHA1", Value = "SHA1" });
                items.Add(new SelectListItem() { Text = "MD5", Value = "MD5" });
                items.Add(new SelectListItem() { Text = "IPV4", Value = "IPV4" });
                items.Add(new SelectListItem() { Text = "IPV6", Value = "IPV6" });
                return items;
            }
        }
        public List<SelectListItem> PolicyList
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = "DETECT", Value = "DETECT" });
                items.Add(new SelectListItem() { Text = "NONE", Value = "NONE" });
                return items;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (this.Type.ToUpper().Equals("DOMAIN") && (this.Value.Length < 1 || this.Value.Length > 200))
            {
                yield return new ValidationResult("Domain must be at least 1 character and no more than 200", new[] { "Value" });
            }

            if (this.Type.ToUpper().Equals("SHA256") && this.Value.Length != 64)
            {
                yield return new ValidationResult("Not a valid SHA256, must be 64 characters", new[] { "Value" });
            }

            if (this.Type.ToUpper().Equals("SHA1") && this.Value.Length != 40)
            {
                yield return new ValidationResult("Not a valid SHA1, must be 40 characters", new[] { "Value" });
            }

            if (this.Type.ToUpper().Equals("MD5") && this.Value.Length != 32)
            {
                yield return new ValidationResult("Not a valid MD5, must be 32 characters", new[] { "Value" });
            }

            if (this.Type.ToUpper().Equals("IPV4") && !Regex.IsMatch(this.Value,@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"))
            {
                yield return new ValidationResult("Not a valid IPv4 address", new[] { "Value" });
            }

        }


    }

}