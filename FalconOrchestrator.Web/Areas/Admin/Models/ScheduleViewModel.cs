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


using System.Linq;
using System.Web.Mvc;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Areas.Admin.Models
{
    public class ScheduleViewModel
    {
        public SelectList ResponderList {
            get
            {
                FalconOrchestratorDB db = new FalconOrchestratorDB();
                return new SelectList(db.Responders.Select(x => new { FullName = x.FirstName + " " + x.LastName, ResponderId = x.ResponderId}),"ResponderId","FullName", ResponderId);          
            }
        }
        public string DayOfWeek { get; set; }
        public string ResponderId { get; set; }
        public string FullName { get; set; }
        public int ScheduleId { get; set; }


    }
}