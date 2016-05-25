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


using System.ComponentModel.DataAnnotations;

namespace FalconOrchestratorWeb.Areas.Forensics.Models
{
    public class FileExtractionViewModel
    {
        [Required(ErrorMessage = "A hostname or IP address is required")]
        public string ComputerName { get; set; }
        [Required(ErrorMessage = "A file path is required")]
        public string FilePath { get; set; }
    }

    public class FileBrowserViewModel
    {
        [Required(ErrorMessage = "A hostname or IP address is required")]
        public string ComputerName { get; set; }
        [Required(ErrorMessage = "A directory is required")]
        public string Directory { get; set; }
    }

    public class SoftwareInventoryViewModel
    {
        [Required(ErrorMessage = "A hostname or IP address is required")]
        public string ComputerName { get; set; }
    }
}