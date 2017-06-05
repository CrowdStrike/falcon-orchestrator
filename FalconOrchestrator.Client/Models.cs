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
using System.Linq;
using Newtonsoft.Json;
using FalconOrchestrator.DAL;
using log4net;
using System.Data.Entity;
using System.Data.Entity.Validation;
using FalconOrchestrator.IOC;
using FalconOrchestrator.LDAP;

namespace FalconOrchestrator.Client
{
    class EventModel
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(EventModel));

        [JsonProperty("metadata")]
        public MetadataModel Metadata { get; set; }
        public virtual bool Exists() { return true; }
        public virtual void Save() { }
    }

    class MetadataModel
    {
        [JsonProperty("customerIDString")]
        public string CustomerIdString { get; set; }

        [JsonProperty("offset")]
        public string Offset { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

    }

    class DetectionModel : EventModel
    {
        [JsonProperty("event")]
        public Detect Data { get; set; }

        public override bool Exists()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Detections.Any(x => x.Offset.Equals(Metadata.Offset));
            }
        }

        public override void Save()
        {
            List<Rule> rules = new List<Rule>();
            rules.Add(new AssignResponder(this));
            rules.Add(new Whitelisting(this));
            rules.Add(new DNSLookup(this));
            rules.Add(new ADLookup(this));
            rules.Add(new Taxonomize(this));
            rules.Add(new EmailNotification(this));
            rules.Where(x => x.IsEnabled()).ToList().ForEach(x => x.Execute());

            try
            {
                Persistence db = new Persistence(this.Data);
                db.SaveToDatabase(Metadata.CustomerIdString, Metadata.Offset);
                log.Debug("[" + Metadata.Offset + "] Detection event saved to database");
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

            catch (Exception e)
            {
                log.Fatal("[" + Metadata.Offset + "] Error occured while trying to save detection event to database", e);
                System.Environment.Exit(1);
            }
        }
    }

    class AuthActivityAuditModel : EventModel
    {
        [JsonProperty("event")]
        private AuditEvent Data { get; set; }

        public override bool Exists()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.AuthenticationLogs.Any(x => x.Offset.Equals(Metadata.Offset));
            }
        }

        public override void Save()
        {
            try
            {
                using (FalconOrchestratorDB db = new FalconOrchestratorDB())
                {
                    AuthenticationLog model = new AuthenticationLog();
                    model.Timestamp = Data.FormattedTimestamp;
                    model.Offset = Metadata.Offset;
                    model.OperationName = Data.OperationName;
                    model.ServiceName = Data.ServiceName;
                    model.Success = Data.Success;
                    model.UserId = Data.UserId;
                    model.UserIp = Data.UserIp;
                    model.CustomerId = Persistence.GetCustomerId(Metadata.CustomerIdString);

                    if (Data.AuditKeyValues != null)
                    {
                        model.TargetName = Data.AuditKeyValues.Where(x => x.Key.Equals("target_name")).Select(x => x.ValueString).FirstOrDefault();
                        model.Entitlement = Data.AuditKeyValues.Where(x => x.Key.Equals("entitlement")).Select(x => x.ValueString).FirstOrDefault();
                        model.EntitlementGroup = Data.AuditKeyValues.Where(x => x.Key.Equals("entitlementGroup")).Select(x => x.ValueString).FirstOrDefault();
                    }
                    else
                    {
                        model.TargetName = null;
                        model.Entitlement = null;
                        model.EntitlementGroup = null;
                    }

                    db.AuthenticationLogs.Add(model);
                    db.SaveChanges();
                    AppConfiguration.FALCON_STREAM_LAST_OFFSET = Metadata.Offset;
                    log.Debug("[" + Metadata.Offset + "] Authentication audit event saved to database");
                }
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

            catch (Exception e)
            {
                log.Fatal("[" + Metadata.Offset + "] Error occured while trying to save authentication activity audit event to database", e);
                System.Environment.Exit(1);
            }
        }
    }

    class UserActivityAuditModel : EventModel
    {
        [JsonProperty("event")]
        private AuditEvent Data { get; set; }

        public override bool Exists()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.UserActivityLogs.Any(x => x.Offset.Equals(Metadata.Offset));
            }
        }

        public override void Save()
        {
            try
            {
                using (FalconOrchestratorDB db = new FalconOrchestratorDB())
                {
                    UserActivityLog model = new UserActivityLog();
                    model.CustomerId = Persistence.GetCustomerId(Metadata.CustomerIdString);
                    model.Timestamp = Data.FormattedTimestamp;
                    model.UserId = Data.UserId;
                    model.UserIp = Data.UserIp;
                    model.Offset = Metadata.Offset;
                    model.OperationName = Data.OperationName;
                    model.ServiceName = Data.ServiceName;
                    model.Success = Data.Success;

                    if (Data.AuditKeyValues != null)
                    {
                        model.State = Data.AuditKeyValues.Where(x => x.Key.Equals("new_state")).Select(x => x.ValueString).FirstOrDefault();
                        model.DetectId = Data.AuditKeyValues.Where(x => x.Key.Equals("detects")).Select(x => x.ValueString).FirstOrDefault();
                    }
                    else
                    {
                        model.State = null;
                        model.DetectId = null;
                    }

                    db.UserActivityLogs.Add(model);
                    db.SaveChanges();
                    AppConfiguration.FALCON_STREAM_LAST_OFFSET = Metadata.Offset;
                    log.Debug("[" + Metadata.Offset + "] User activity audit event saved to database");
                }
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                .SelectMany(x => x.ValidationErrors)
                .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }

            catch (Exception e)
            {
                log.Fatal("[" + Metadata.Offset + "] Error occured while trying to save user activity audit event to database", e);
                System.Environment.Exit(1);
            }
        }
    }

    class Detect
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(Detect));
        public string ProcessStartTime { get; set; }
        public string ProcessEndTime { get; set; }
        public string ProcessId { get; set; }
        public string ParentProcessId { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string DetectName { get; set; }
        public string DetectDescription { get; set; }
        public int Severity { get; set; }
        public string SeverityName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string CommandLine { get; set; }
        public string SHA256String { get; set; }
        public string MD5String { get; set; }
        public string SHA1String { get; set; }
        public string MachineDomain { get; set; }
        public string FalconHostLink { get; set; }
        public string SensorId { get; set; }
        public DnsRequestsModel[] DnsRequest { get; set; }
        public DocumentsAccessedModel[] DocumentsAccessed { get; set; }
        public ExecutableWrittenModel[] ExecutablesWritten { get; set; }
        public NetworkAccessesModel[] NetworkAccesses { get; set; }
        public ScanResultsModel[] ScanResults { get; set; }

        public DateTime? FormattedProcessStartTime
        {
            get
            {
                try
                {
                    if (ProcessStartTime == "0" || String.IsNullOrEmpty(ProcessStartTime))
                    {
                        return DateTime.UtcNow;
                    }

                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.AddSeconds(Convert.ToDouble(ProcessStartTime)).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return DateTime.UtcNow;
                }
            }
        }
        public DateTime? FormattedProcessEndTime
        {
            get
            {
                try
                {
                    if (ProcessEndTime == "0" || String.IsNullOrEmpty(ProcessEndTime))
                    {
                        return DateTime.UtcNow;
                    }

                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.AddSeconds(Convert.ToDouble(ProcessStartTime)).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return DateTime.UtcNow;
                }
            }
        }

        public string IPAddress { get; set; }
        public int? ResponderId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ClosedDate { get; set; }
        public List<int> TaxonomyIds { get; set; }
        public AccountModel AccountModel { get; set; }
    }

    class AccountModel
    {
        //These properties are obtained through integration to Active Directory
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string Manager { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public string Country { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string OrganizationalUnit { get; set; }
        public DateTime? LastLogon { get; set; }
        public List<string> Groups { get; set; }

    }

    class NetworkAccessesModel
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(NetworkAccessesModel));

        [JsonProperty("AccessTimestamp")]
        public string Timestamp { get; set; }
        public int AccessType { get; set; }
        public string Protocol { get; set; }
        public string LocalAddress { get; set; }
        public int LocalPort { get; set; }
        public string RemoteAddress { get; set; }
        public int RemotePort { get; set; }
        public int ConnectionDirection { get; set; }
        public bool IsIPV6 { get; set; }
        public DateTime FormattedTimestamp
        {
            get
            {
                try
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.AddSeconds(Convert.ToDouble(Timestamp)).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    log.Warn("Malformed network access timestamp, failing over to current time");
                    return DateTime.UtcNow;
                }
            }
        }
    }

    class DnsRequestsModel
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(DnsRequestsModel));

        [JsonProperty("LoadTime")]
        public string Timestamp { get; set; }
        public bool CausedDetect { get; set; }
        public string DomainName { get; set; }
        public int InterfaceIndex { get; set; }
        public string RequestType { get; set; }
        public DateTime FormattedTimestamp
        {
            get
            {
                try
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.AddSeconds(Convert.ToDouble(Timestamp)).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    log.Warn("Malformed DNS timestamp, failing over to current time");
                    return DateTime.UtcNow;
                }
            }
        }
    }

    class DocumentsAccessedModel
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(DocumentsAccessedModel));

        public string Timestamp { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime FormattedTimestamp
        {
            get
            {
                try
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.AddSeconds(Convert.ToDouble(Timestamp)).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    log.Warn("Malformed Document access timestamp, failing over to current time");
                    return DateTime.UtcNow;
                }
            }
        }
    }

    class ExecutableWrittenModel
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(ExecutableWrittenModel));

        public string Timestamp { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime FormattedTimestamp
        {
            get
            {
                try
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.AddSeconds(Convert.ToDouble(Timestamp)).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    log.Warn("Malformed executable written timestamp, failing over to current time");
                    return DateTime.UtcNow;
                }
            }
        }
    }

    class ScanResultsModel
    {
        public string Engine { get; set; }
        public string ResultName { get; set; }
        public string Version { get; set; }
    }

    class AuditEvent
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(AuditEvent));

        public string UserId { get; set; }
        public string UserIp { get; set; }
        public string OperationName { get; set; }
        public string ServiceName { get; set; }
        public bool Success { get; set; }
        [JsonProperty("UTCTimestamp")]
        public string Timestamp { get; set; }
        public DateTime FormattedTimestamp
        {
            get
            {
                try
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return dateTime.AddSeconds(Convert.ToDouble(Timestamp)).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    log.Warn("Malformed audit event timestamp, failing over to current time");
                    return DateTime.UtcNow;
                }
            }
        }
        public AuditKeyValues[] AuditKeyValues { get; set; }

    }

    class AuditKeyValues
    {
        public string Key { get; set; }
        public string ValueString { get; set; }

    }

}
