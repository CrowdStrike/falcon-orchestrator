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
using System.Linq;
using FalconOrchestrator.DAL;
using log4net;

namespace FalconOrchestrator.Client
{
    class Persistence
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Persistence));

        private Detect data;

        public Persistence(Detect _data)
        {
            log4net.Config.XmlConfigurator.Configure();
            data = _data;
        }

        private int GetDetectionDeviceId()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                int detectDeviceId;
                if (db.Devices.Any(x => x.Hostname.Equals(data.ComputerName)))
                {
                    int deviceId = db.Devices.Where(x => x.Hostname.Equals(data.ComputerName)).Select(x => x.DeviceId).Single();
                    DetectionDevice detectDevice = new DetectionDevice();
                    detectDevice.DeviceId = deviceId;
                    detectDevice.IPAddress = data.IPAddress;
                    db.DetectionDevices.Add(detectDevice);
                    db.SaveChanges();
                    detectDeviceId = detectDevice.DetectionDeviceId;
                }
                else
                {
                    Device device = new Device();
                    device.Hostname = data.ComputerName;
                    device.SensorId = data.SensorId;
                    device.Domain = data.MachineDomain;
                    db.Devices.Add(device);

                    DetectionDevice detectDevice = new DetectionDevice();
                    detectDevice.DeviceId = device.DeviceId;
                    db.DetectionDevices.Add(detectDevice);
                    db.SaveChanges();
                    detectDeviceId = detectDevice.DetectionDeviceId;
                }
                return detectDeviceId;
            }
        }

        private int GetGroupId(string groupName)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                int groupId;
                if (db.Groups.Any(x => x.Name.Equals(groupName)))
                {
                    groupId = db.Groups.Where(x => x.Name.Equals(groupName)).Select(x => x.GroupId).Single();
                }
                else
                {
                    Group group = new Group();
                    group.Name = groupName;
                    db.Groups.Add(group);
                    db.SaveChanges();
                    groupId = group.GroupId;
                }
                return groupId;
            }
        }

        private int GetAccountId()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                int accountId;

                if (db.Accounts.Any(x => x.AccountName.Equals(data.UserName)))
                {
                    accountId = db.Accounts.Where(x => x.AccountName.Equals(data.UserName)).Select(x => x.AccountId).FirstOrDefault();
                }

                else
                {
                    Account user = new Account();
                    user.AccountName = data.UserName;
                    user.Timestamp = DateTime.UtcNow;

                    db.Accounts.Add(user);
                    db.SaveChanges();
                    accountId = user.AccountId;

                    if (data.AccountModel != null)
                    {
                        user.City = data.AccountModel.City;
                        user.Country = data.AccountModel.Country;
                        user.Department = data.AccountModel.Department;
                        user.EmailAddress = data.AccountModel.EmailAddress;
                        user.FirstName = data.AccountModel.FirstName;
                        user.JobTitle = data.AccountModel.JobTitle;
                        user.LastLogon = data.AccountModel.LastLogon != DateTime.MinValue ? data.AccountModel.LastLogon : null;
                        user.LastName = data.AccountModel.LastName;
                        user.Manager = data.AccountModel.Manager;
                        user.PhoneNumber = data.AccountModel.PhoneNumber;
                        user.StateProvince = data.AccountModel.StateProvince;
                        user.StreetAddress = data.AccountModel.StreetAddress;
                        user.OrganizationalUnit = data.AccountModel.OrganizationalUnit;

                        if (data.AccountModel.Groups != null)
                        {
                            foreach (string line in data.AccountModel.Groups)
                            {
                                int groupId = GetGroupId(line);
                                AccountGroup accountGroup = new AccountGroup();
                                accountGroup.AccountId = accountId;
                                accountGroup.GroupId = groupId;
                                db.AccountGroups.Add(accountGroup);
                            }
                        }
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                }

                return accountId;
            }

        }

        private int GetSeverityId()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                int severityId;
                if (db.Severities.Any(x => x.SeverityType.Equals(data.SeverityName)))
                {
                    severityId = db.Severities.Where(x => x.SeverityType.Equals(data.SeverityName)).Select(x => x.SeverityId).Single();
                }
                else
                {
                    Severity severity = new Severity();
                    severity.SeverityId = data.Severity;
                    severity.SeverityType = data.SeverityName;
                    db.Severities.Add(severity);
                    db.SaveChanges();
                    severityId = severity.SeverityId;
                }
                return severityId;
            }

        }

        public static int GetCustomerId(string cid)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                int customerId;
                if (db.Customers.Any(x => x.CustomerIdString.Equals(cid)))
                {
                    customerId = db.Customers.Where(x => x.CustomerIdString.Equals(cid)).Select(x => x.CustomerId).Single();
                }
                else
                {
                    Customer customer = new Customer();
                    customer.CustomerIdString = cid;
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    customerId = customer.CustomerId;
                }
                return customerId;
            }
        }

        public static Responder GetAssignedResponder(string dayOfWeek)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.ResponderSchedules.Where(x => x.DayOfWeek.Equals(dayOfWeek)).Select(x => x.Responder).FirstOrDefault();
            }
        }

        public void SaveToDatabase(string cid, string offset)
        {
            Detection detection = new Detection();
            detection.AccountId = GetAccountId();
            detection.CommandLine = data.CommandLine;
            detection.CustomerId = GetCustomerId(cid);
            detection.CustomSeverityId = detection.VendorSeverityId = GetSeverityId();
            detection.Description = data.DetectDescription;
            detection.DetectionDeviceId = GetDetectionDeviceId();
            detection.FalconHostLink = data.FalconHostLink;
            detection.FileName = data.FileName;
            detection.FilePath = data.FilePath;
            detection.Name = data.DetectName;
            detection.ResponderId = data.ResponderId;
            detection.MD5 = data.MD5String;
            detection.Offset = offset;
            detection.ParentProcessId = data.ParentProcessId;
            detection.ProcessEndTime = data.FormattedProcessEndTime;
            detection.ProcessStartTime = data.FormattedProcessStartTime;
            detection.ProcessId = data.ProcessId;
            detection.SHA1 = data.SHA1String;
            detection.SHA256 = data.SHA256String;
            detection.Timestamp = DateTime.UtcNow;
            if (data.StatusId.HasValue) { detection.StatusId = (int)data.StatusId; } else { detection.StatusId = 1; }

            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                db.Detections.Add(detection);
                db.SaveChanges();

                if (data.TaxonomyIds != null)
                {
                    foreach (int line in data.TaxonomyIds)
                    {
                        DetectionTaxonomy dt = new DetectionTaxonomy();
                        dt.DetectionId = detection.DetectionId;
                        dt.TaxonomyId = line;
                        db.DetectionTaxonomies.Add(dt);
                    }
                }

                if (data.DnsRequest != null)
                {
                    foreach (DnsRequestsModel line in data.DnsRequest)
                    {
                        DnsRequest dns = new DnsRequest();
                        dns.CausedDetect = line.CausedDetect;
                        dns.DetectionId = detection.DetectionId;
                        dns.DomainName = line.DomainName;
                        dns.InterfaceIndex = line.InterfaceIndex;
                        dns.RequestType = line.RequestType;
                        dns.Timestamp = line.FormattedTimestamp;
                        db.DnsRequests.Add(dns);
                    }
                }

                if (data.DocumentsAccessed != null)
                {
                    foreach (DocumentsAccessedModel line in data.DocumentsAccessed)
                    {
                        DocumentsAccess doc = new DocumentsAccess();
                        doc.Timestamp = line.FormattedTimestamp;
                        doc.FileName = line.FileName;
                        doc.FilePath = line.FilePath;
                        doc.DetectionId = detection.DetectionId;
                        db.DocumentsAccesses.Add(doc);
                    }
                }

                if (data.ExecutablesWritten != null)
                {
                    foreach (ExecutableWrittenModel line in data.ExecutablesWritten)
                    {
                        ExecutablesWritten exe = new ExecutablesWritten();
                        exe.Timestamp = line.FormattedTimestamp;
                        exe.FileName = line.FileName;
                        exe.FilePath = line.FilePath;
                        exe.DetectionId = detection.DetectionId;
                        db.ExecutablesWrittens.Add(exe);
                    }
                }

                if (data.NetworkAccesses != null)
                {
                    foreach (NetworkAccessesModel line in data.NetworkAccesses)
                    {
                        NetworkAccess network = new NetworkAccess();
                        network.Timestamp = line.FormattedTimestamp;
                        network.AccessType = line.AccessType;
                        network.ConnectionDirection = line.ConnectionDirection;
                        network.IsIPv6 = line.IsIPV6;
                        network.LocalAddress = line.LocalAddress;
                        network.LocalPort = line.LocalPort;
                        network.Protocol = line.Protocol;
                        network.RemoteAddress = line.RemoteAddress;
                        network.RemotePort = line.RemotePort;
                        network.DetectionId = detection.DetectionId;
                        db.NetworkAccesses.Add(network);
                    }
                }

                if (data.ScanResults != null)
                {
                    foreach (ScanResultsModel line in data.ScanResults)
                    {
                        ScanResult scan = new ScanResult();
                        scan.DetectionId = detection.DetectionId;
                        scan.Engine = line.Engine;
                        scan.ResultName = line.ResultName;
                        scan.Version = line.Version;
                        db.ScanResults.Add(scan);
                    }
                }
                db.SaveChanges();
                AppConfiguration.FALCON_STREAM_LAST_OFFSET = offset;           
            }
        }

    }
}
