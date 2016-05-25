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

namespace FalconOrchestrator.Forensics
{
    public class SystemRestorePoints
    {
        public DateTime CreationTime { get; set; }
        public string Description { get; set; }
        public string SequenceNumber { get; set; }
        public string EventType { get; set; }
        public string RestorePointType { get; set; }
    }

    public class InstalledSoftware
    {
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string Publisher { get; set; }
        public string InstallDate { get; set; }
        public string Architecture { get; set; }
        public string UninstallKey { get; set; }
        public string Guid { get; set; }
    }
}
