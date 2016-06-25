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
    public class FileSystemBrowser
    {
        private PSRemoting _psr;

        public FileSystemBrowser(PSRemoting psr)
        {
            _psr = psr;
        }

        public List<FileMetadata> GetDirectoryContent(string path)
        {
            List<FileMetadata> files = new List<FileMetadata>();
            string command = "Get-ChildItem -force " + path;
            foreach(PSObject line in _psr.ExecuteCommand(command))
            {
                FileMetadata file = new FileMetadata();
                file.CreationTime = line.Properties["CreationTimeUtc"].Value.ToString();
                file.LastAccessTime = line.Properties["LastAccessTimeUtc"].Value.ToString();
                file.LastWriteTime = line.Properties["LastWriteTimeUtc"].Value.ToString();
                file.Attributes = line.Properties["Attributes"].Value.ToString();
                file.Name = line.Properties["Name"].Value.ToString();
                file.FullName = line.Properties["FullName"].Value.ToString();
                if (!file.Attributes.Contains("Directory"))
                {
                    file.Length = line.Properties["Length"].Value.ToString();
                }
                if(!file.Attributes.Contains("Directory"))
                {
                    file.Extension = line.Properties["Extension"].Value.ToString();
                }

                files.Add(file);
            }
            return files;

        }

    }
}
