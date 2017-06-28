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
using Newtonsoft.Json;

namespace FalconOrchestrator.Forensics
{
    public class SystemRestorePoint
    {
        public string CreationTime { get; set; }
        public string Description { get; set; }
        public string SequenceNumber { get; set; }
        public string EventType { get; set; }
        public string RestorePointType { get; set; }

        public DateTime CreationTimeDateTime
        {
            get
            {
                return DateTime.ParseExact(CreationTime.Split('-')[0], "yyyyMMddHHmmss.ffffff", null);
            }
        }
    }
    public class ShadowCopy
    {
        public string ID { get; set; }
        public string DeviceObject { get; set; }
        public string InstallDate { get; set; }

        public DateTime InstallDateTime
        {
            get
            {
                return DateTime.ParseExact(InstallDate.Split('-')[0], "yyyyMMddHHmmss.ffffff", null).ToUniversalTime();
            }
        }
    }
    public class Tasks
    {
        public string NextRunTime { get; set; }
        public string Author { get; set; }
        public string Trigger { get; set; }
        public string State { get; set; }
        public string UserId { get; set; }
        public string ComputerName { get; set; }
        public string Name { get; set; }
        public string LastRunTime { get; set; }
        public string LastTaskResult { get; set; }
        public string Description { get; set; }
        public string NumberofMissedRuns { get; set; }
        public string Enabled { get; set; }
        public string Path { get; set; }
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

    public class Process
    {
        public int Id { get; set; }
        public uint ParentProcessId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Owner { get; set; }
        public string Path { get; set; }
        public string ProductVersion { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public string CommandLine { get; set; }
        public double CPU { get; set; }
        public int HandleCount { get; set; }
        public int PagedMemorySize { get; set; }
        public int PagedSystemMemorySize { get; set; }
        public int PrivateMemorySize { get; set; }
        public int VirtualMemorySize { get; set; }
        public int NonpagedSystemMemorySize { get; set; }
        public int PeakPagedMemorySize { get; set; }
        public int PeakWorkingSet { get; set; }
        public int PeakVirtualMemorySize { get; set; }
        public int WorkingSet { get; set; }
    }

    public class FileMetadata
    {
        public string CreationTime { get; set; }
        public string LastAccessTime { get; set; }
        public string LastWriteTime { get; set; }
        public string Attributes { get; set; }
        public string DirectoryName { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Extension { get; set; }
        public string Length { get; set; }
    }
}