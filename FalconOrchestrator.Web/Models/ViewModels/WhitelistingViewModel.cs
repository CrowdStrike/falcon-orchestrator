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


using System.Collections.Generic;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;


namespace FalconOrchestratorWeb.Models.ViewModels
{
    public class WhitelistingViewModel : IValidatableObject
    {
        public int WhitelistId { get; set; }
        public string Timestamp { get; set; }
        [Required(ErrorMessage="Creator is required, this is based off logged in username")]
        public string Creator { get; set; }
        [Required(ErrorMessage = "Reason for rule is required")]
        public string Reason { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public int TypeId { get; set; }
        [Required(ErrorMessage = "Value is required")]

        [AllowHtml]
        public string Value { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.TypeId == 1 && !Regex.IsMatch(this.Value, @"\b(?:[a-fA-F0-9][\s]*){63}[a-fA-F0-9]\b"))
            {
                yield return new ValidationResult("Not a valid SHA256 hash format", new[] { "Value" });
            }
        }
    }
}
