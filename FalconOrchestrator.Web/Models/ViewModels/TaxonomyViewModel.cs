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
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace FalconOrchestratorWeb.Models.ViewModels
{
    public class TaxonomyViewModel
    {
        public int TaxonomyId { get; set; }
        public string Timestamp { get; set; }
        [Required(ErrorMessage = "Creator is required, this is based off logged in username")]
        public string Creator { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public int TypeId { get; set; }
        [Required(ErrorMessage = "Value is required")]
        [AllowHtml]
        public string Value { get; set; }
        [Required(ErrorMessage = "Critical is required")]
        public Boolean Critical { get; set; }

    }
}