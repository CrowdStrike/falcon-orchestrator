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
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Models.ViewModels
{
    public class DetectionListViewModel
    {
        public int DT_RowId { get; set; }
        public string Timestamp { get; set; }
        public string DetectionName { get; set; }
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string FileName { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public string Responder { get; set; }
    }

    public class DetectionEditViewModel
    {
        public int AssociatedEventCount { get; set; }
        public int TaxonomyCount { get; set; }
        public List<string> TaxonomyList { get; set; }
        public int DetectionId { get; set; }
        public string DetectId { get; set; }
        public string FalconHostLink { get; set; }
        public string Offset { get; set; }
        public string Handler { get; set; }
        public string Severity { get; set; }
        public string FileName { get; set; }
        public string ClosedDate { get; set; }

        [AllowHtml]
        public string FilePath { get; set; }
        [AllowHtml]
        public string CommandLine { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public string Status { get; set; }
        public string Name { get; set; } 
        public string SHA256 { get; set; }
        public string MD5 { get; set; }
        public Asset Asset { get; set; }

        public int? ResponderId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int StatusId { get; set; }
        [Required(ErrorMessage = "Severity is required")]
        public int CustomSeverityId { get; set; }

        public string Tags { get; set; }
        public string Comment { get; set; } //auto

        public SelectList ResponsersList
        {
            get
            {
                FalconOrchestratorDB db = new FalconOrchestratorDB();
                return new SelectList(db.Responders.ToList().Select(x => new { FullName = x.FirstName + " " + x.LastName, ResponderId = x.ResponderId }), "ResponderId", "FullName", ResponderId);
            }
        }

        public SelectList StatusList
        {
            get
            {
                FalconOrchestratorDB db = new FalconOrchestratorDB();
                return new SelectList(db.Status.ToList(), "StatusId", "StatusType", StatusId);
            }
        }

        public SelectList SeverityList
        {
            get
            {
                FalconOrchestratorDB db = new FalconOrchestratorDB();
                return new SelectList(db.Severities.ToList(), "SeverityId", "SeverityType", CustomSeverityId);
            }
        }
    }

    public class Asset
    {
        [Required(ErrorMessage="Hostname is required")]
        public string Hostname { get; set; }
        public string Domain { get; set; }
        public string HostnameDisplay
        {
            get
            {
                return this.Hostname != this.Domain && this.Domain != null ? this.Domain + "\\" + this.Hostname : this.Hostname;
            }
        }
        [Required(ErrorMessage="Account name is required")]
        public string AccountName { get; set; }
        public string JobTitle { get; set; }
        [EmailAddress(ErrorMessage = "Not a valid email address format")]
        public string Manager { get; set; }
        public string PhoneNumber { get; set; }
        public string StateProvince { get; set; }
        public string StreetAddress { get; set; }
        public string OrganizationalUnit { get; set; }
        [RegularExpression(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}", ErrorMessage = "Not a valid IPv4 format")]
        public string IPAddress { get; set; }
        public string Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }

        [EmailAddress(ErrorMessage="Not a valid email address format")]
        public string EmailAddress { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string LastLogonTime { get; set; }
        public List<string> GroupMemberships { get; set; }

    }

    public class NetworkAccess
    {
        public string Timestamp { get; set; }
        public int AccessType { get; set; }
        public int ConnectionDirection { get; set; }
        public bool IsIPv6 { get; set; }
        public string LocalAddress { get; set; }
        public string LocalPort { get; set; }
        public string RemoteAddress { get; set; }
        public string RemotePort { get; set; }
        public string Protocol { get; set; }
    }

    public class ExecutablesWritten
    {
        public string Timestamp { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class DocumentsAccess
    {
        public string Timestamp { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class DnsRequest
    {
        public string Timestamp { get; set; }
        public bool CausedDetect { get; set; }
        public string DomainName { get; set; }
        public string RequestType { get; set; }
        public int InterfaceIndex { get; set; }

    }

    public class ScanResult
    {
        public string Engine { get; set; }
        public string ResultName { get; set; }
        public string Version { get; set; }
    }

}