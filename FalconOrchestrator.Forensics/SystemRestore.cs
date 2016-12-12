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
using System.Management.Automation;
using System.Linq;

namespace FalconOrchestrator.Forensics
{
    public class SystemRestore
    {
        private PSRemoting _psr;

        public SystemRestore(PSRemoting psr)
        {
            _psr = psr;
        }

        public List<SystemRestorePoint> GetRestorePoints()
        {
            List<SystemRestorePoint> restorePoints = new List<SystemRestorePoint>();
            foreach (PSObject result in _psr.ExecuteCommand("Get-ComputerRestorePoint"))
            {
                SystemRestorePoint point = new SystemRestorePoint();
                point.CreationTime = result.Members["CreationTime"].Value.ToString();
                point.Description = result.Members["Description"].Value.ToString();
                point.SequenceNumber = result.Members["SequenceNumber"].Value.ToString();
                point.EventType = result.Members["EventType"].Value.ToString();
                point.RestorePointType = result.Members["RestorePointType"].Value.ToString();
                point.EventType = EventTypeMapping(point.EventType);
                point.RestorePointType = RestorePointTypeMapping(point.RestorePointType);
                restorePoints.Add(point);
            }
            return restorePoints;
        }

        private string EventTypeMapping(string eventType)
        {

            switch (eventType)
            {
                case "100":
                    return "BEGIN_SYSTEM_CHANGE";
                case "101":
                    return "END_SYSTEM_CHANGE";
                case "102":
                    return "BEGIN_NESTED_SYSTEM_CHANGE";
                case "103":
                    return "END_NESTED_SYSTEM_CHANGE";
                default:
                    return eventType;
            }

        }

        private string RestorePointTypeMapping(string restorePointType)
        {
            switch (restorePointType)
            {
                case "0":
                    return "APPLICATION_INSTALL";
                case "1":
                    return "APPLICATION_UNINSTALL";
                case "6":
                    return "RESTORE";
                case "7":
                    return "CHECKPOINT";
                case "10":
                    return "DEVICE_DRIVER_INSTALLATION";
                case "11":
                    return "FIRST_RUN";
                case "12":
                    return "MODIFY_SETTINGS";
                case "13":
                    return "CANCELLED_OPERATION";
                case "14":
                    return "BACKUP_RECOVERY";
                default:
                    return restorePointType;
            }
        }

        public void RestoreSystem(string sequenceNumber)
        {
            _psr.ExecuteCommand("Restore-Computer -RestorePoint " + sequenceNumber);
        }

        public ShadowCopy GetShadowCopyByTime(DateTime timestamp)
        {
            //When a system restore is taken, a shadow copy is subsequently created.  This almost always occurs within the same minute.
            //The timestamp is used to correlate a shadow copy volume with a restore point
            List<ShadowCopy> copies = GetShadowCopies();
            foreach (var line in copies)
            {
                if ((timestamp - line.InstallDateTime).TotalMinutes < 1)
                {
                    return line;
                }
            }
            return null;
        }

        public List<ShadowCopy> GetShadowCopies()
        {
            List<ShadowCopy> copies = new List<ShadowCopy>();
            foreach (PSObject line in _psr.ExecuteCommand("Get-WmiObject -Class Win32_ShadowCopy | select ID, InstallDate, DeviceObject"))
            {
                ShadowCopy copy = new ShadowCopy();
                line.Properties.ToList().ForEach(x => copy.GetType().GetProperty(x.Name).SetValue(copy, x.Value));
                copies.Add(copy);
            }
            return copies;
        }

        public void MountShadowCopyVolume(string volume, string path = @"C:\shadow")
        {
            FileSystemBrowser filesys = new FileSystemBrowser(_psr);
            if (filesys.CheckFileExists(path))
                filesys.DeleteFile(path);

            _psr.ExecuteCommand("cmd /c mklink /d " + path + " " + volume);
        }

    }


}
