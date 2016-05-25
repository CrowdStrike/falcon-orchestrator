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
using System.Management.Automation;

namespace FalconOrchestrator.Forensics
{
    public class SoftwareManagement
    {
        private PSRemoting _psr;

        public SoftwareManagement(PSRemoting psr)
        {
            _psr = psr;
        }

        public List<InstalledSoftware> GetInstalledSoftware(string command)
        {
            List<InstalledSoftware> list = new List<InstalledSoftware>();        
            foreach(PSObject line in _psr.ExecuteCommand(command))
            {
                InstalledSoftware item = new InstalledSoftware();

                if(line.Properties["InstalledDate"].Value != null)
                {
                    item.InstallDate = line.Properties["InstalledDate"].Value.ToString();
                }

                if(line.Properties["AppName"].Value != null)
                {                
                    item.DisplayName = line.Properties["AppName"].Value.ToString();
                }

                if(line.Properties["AppVersion"].Value != null)
                {
                    item.DisplayVersion = line.Properties["AppVersion"].Value.ToString();
                }
                if (line.Properties["AppVendor"].Value != null)
                {
                    item.Publisher = line.Properties["AppVendor"].Value.ToString();
                }
                if (line.Properties["UninstallKey"].Value != null)
                {
                    item.UninstallKey = line.Properties["UninstallKey"].Value.ToString();
                }
                if (line.Properties["AppGuid"].Value != null)
                {
                    item.Guid = line.Properties["AppGuid"].Value.ToString();
                }
                if (line.Properties["Softwarearchitecture"].Value != null)
                {
                    item.Architecture = line.Properties["Softwarearchitecture"].Value.ToString();
                }

                list.Add(item);
            }
            return list;
        }
    }
}
