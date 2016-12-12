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
    public class FileExtraction
    {
        private PSRemoting ps;

        public FileExtraction(PSRemoting _ps)
        {
            ps = _ps;
        }

        public void UploadFile(string command)
        {           
            string response = null;
            foreach(PSObject line in ps.ExecuteCommand(command))
            {
                response += line.ToString();
            }     
        }

        public FileMetadata GetFileMetadata(string command)
        {
            var file = new FileMetadata();
            foreach(PSObject line in ps.ExecuteCommand(command))
            {
                file.CreationTime = line.Properties["CreationTimeUtc"].Value.ToString();
                file.LastAccessTime = line.Properties["LastAccessTimeUtc"].Value.ToString();
                file.LastWriteTime = line.Properties["LastWriteTimeUtc"].Value.ToString();
                file.DirectoryName = line.Properties["DirectoryName"].Value.ToString();
                file.Attributes = line.Properties["Attributes"].Value.ToString();
                file.Name = line.Properties["Name"].Value.ToString();
                file.FullName = line.Properties["FullName"].Value.ToString();
                file.Length = line.Properties["Length"].Value.ToString();
                file.Extension = line.Properties["Extension"].Value.ToString();
            }     
            return file;
        }

        public Dictionary<string,string> GetDevicePaths(string command)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (PSObject line in ps.ExecuteCommand(command))
            {
                result.Add(line.Properties["DevicePath"].Value.ToString(),line.Properties["DriveLetter"].Value.ToString());
            }
            return result;
        }
    }
}
