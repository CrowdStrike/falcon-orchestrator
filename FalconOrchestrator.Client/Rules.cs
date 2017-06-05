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
using System.Text;
using System.IO;
using System.Net;
using System.Net.Mail;
using log4net;
using System.Configuration;
using System.Text.RegularExpressions;
using FalconOrchestrator.LDAP;
using FalconOrchestrator.DAL;

namespace FalconOrchestrator.Client
{

    abstract class Rule
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Rule));
        protected AppConfiguration config;

        public Rule()
        {
            log4net.Config.XmlConfigurator.Configure();
            config = new AppConfiguration(ConfigurationManager.AppSettings["CryptoKey"]);
        }

        public abstract bool IsEnabled();
        public abstract void Execute();

    }

    class AssignResponder : Rule
    {
        private DetectionModel model;

        public AssignResponder(DetectionModel _model)
        {
            model = _model;
        }

        public override bool IsEnabled()
        {
            return Convert.ToBoolean(config.RULE_ASSIGN_RESPONDER);
        }

        public override void Execute()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                string dayOfWeek = model.Data.FormattedProcessStartTime.Value.DayOfWeek.ToString();
                Responder responder = Persistence.GetAssignedResponder(dayOfWeek);
                if (responder != null)
                {
                    model.Data.ResponderId = responder.ResponderId;
                    log.Debug("Assign responder rule enabled, schedule has " + responder.FirstName + " " + responder.LastName + " assigned for " + dayOfWeek);
                }
                else
                {
                    log.Warn("Assign responder rule enabled, however no responder has been scheduled for " + dayOfWeek);
                }                        
            }
        }
    }

    class EmailNotification : Rule
    {
        private DetectionModel model;

        public EmailNotification(DetectionModel _model)
        {
            model = _model;
        }

        public override bool IsEnabled()
        {
            return Convert.ToBoolean(config.RULE_NOTIFICATION);
        }

        public override void Execute()
        {
            if (model.Data.Severity >= Convert.ToInt32(config.RULE_NOTIFICATION_THRESHOLD) && model.Data.StatusId != 7)
            {
                log.Debug("Notification rule is enabled and severity of " + model.Data.SeverityName + " is above threshold, attempting to send email");
                try
                {
                    SendEmail();
                }
                catch(Exception e)
                {
                    log.Fatal("An error occured while sending an email notification", e);
                    Environment.Exit(1);
                }
            }
        }

        private string TemplateReplace(string template)
        {

            //If account metadata does not exist create an empty account model
            if (model.Data.AccountModel == null)
                model.Data.AccountModel = new AccountModel();

            StringBuilder sb = new StringBuilder(template);
            return sb.Replace("{{Severity}}", model.Data.SeverityName)
                     .Replace("{{DetectionDescription}}", model.Data.DetectDescription)
                     .Replace("{{DetectionName}}", model.Data.DetectName)
                     .Replace("{{Hostname}}", model.Data.ComputerName)
                     .Replace("{{IPAddress}}", model.Data.IPAddress ?? string.Empty)
                     .Replace("{{Username}}", model.Data.UserName)
                     .Replace("{{ProcessStartTime}}", model.Data.FormattedProcessStartTime.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                     .Replace("{{ProcessEndTime}}", model.Data.FormattedProcessEndTime.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                     .Replace("{{FileName}}", model.Data.FileName)
                     .Replace("{{DetectionName}}", model.Data.DetectName)
                     .Replace("{{FalconOrchestratorLink}}", config.FALCON_ORCHESTRATOR_URL + "/detection/edit/" + DatabaseHelpers.GetNextDetectionID() ?? string.Empty)
                     .Replace("{{FalconHostLink}}", model.Data.FalconHostLink)
                     .Replace("{{FilePath}}", model.Data.FilePath)
                     .Replace("{{FirstName}}", model.Data.AccountModel.FirstName ?? string.Empty)
                     .Replace("{{LastName}}", model.Data.AccountModel.LastName ?? string.Empty)
                     .Replace("{{Department}}", model.Data.AccountModel.Department ?? string.Empty)
                     .Replace("{{JobTitle}}", model.Data.AccountModel.JobTitle ?? string.Empty)
                     .Replace("{{EmailAddress}}", model.Data.AccountModel.EmailAddress ?? string.Empty)
                     .Replace("{{Manager}}", model.Data.AccountModel.Manager ?? string.Empty)
                     .Replace("{{Country}}", model.Data.AccountModel.Country ?? string.Empty)
                     .Replace("{{City}}", model.Data.AccountModel.City ?? string.Empty)
                     .Replace("{{PhoneNumber}}", model.Data.AccountModel.PhoneNumber ?? string.Empty)
                     .Replace("{{StateProvince}}", model.Data.AccountModel.StateProvince ?? string.Empty)
                     .Replace("{{StreetAddress}}", model.Data.AccountModel.StreetAddress ?? string.Empty)
                     .Replace("{{FalconOrchestratorLink}}", config.FALCON_ORCHESTRATOR_URL != null ? config.FALCON_ORCHESTRATOR_URL + "/detection/edit/" + DatabaseHelpers.GetNextDetectionID() : string.Empty)
                     .Replace("{{FalconHostLink}}", model.Data.FalconHostLink).ToString();
        }

        private void SendEmail()
        {
            MailAddress from = new MailAddress(config.EMAIL_SENDER);
            MailAddress to = new MailAddress(DatabaseHelpers.GetResponderEmailAddress(model.Data.ResponderId, config));
            string password = config.EMAIL_PASSWORD;
            string subject = TemplateReplace(config.EMAIL_ALERT_SUBJECT);
            string template = File.ReadAllText(config.EMAIL_ALERT_TEMPLATE_PATH);
            string body = TemplateReplace(template);

            SmtpClient smtp = new SmtpClient
            {
                Host = config.EMAIL_SERVER,
                Port = Convert.ToInt32(config.EMAIL_PORT),
                EnableSsl = Convert.ToBoolean(config.EMAIL_SSL),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(from.Address, password)
            };
            using (MailMessage message = new MailMessage(from, to))
            {
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;

                if (model.Data.SeverityName.Equals("Critical"))
                {
                    message.Priority = MailPriority.High;
                }

                if (!String.IsNullOrEmpty(config.EMAIL_CC))
                {
                    foreach (string line in config.EMAIL_CC.Split(','))
                    {
                        message.CC.Add(new MailAddress(line));
                    }
                }
                smtp.Send(message);
            }
        }

    }

    class ADLookup : Rule
    {
        private DetectionModel model;

        public ADLookup(DetectionModel _model)
        {
            model = _model;
        }

        public override bool IsEnabled()
        {
            return Convert.ToBoolean(config.RULE_AD_LOOKUP);
        }

        public override void Execute()
        {
            if (!String.IsNullOrEmpty(model.Data.UserName) && !model.Data.UserName.ToUpper().Contains("LOCAL SYSTEM")
                && !model.Data.UserName.ToUpper().Equals("ADMINISTRATOR")
                && !model.Data.UserName.ToUpper().Contains(model.Data.ComputerName))
            {
                if (DatabaseHelpers.AccountExists(model.Data.UserName))
                {
                    int daysValid = Convert.ToInt32(config.LDAP_DAYS_VALID);
                    DateTime accountTimestamp = DatabaseHelpers.GetAccountTimestamp(model.Data.UserName);
                    DateTime validUntilDate = accountTimestamp.AddDays(daysValid);

                    if (DateTime.UtcNow.Date > validUntilDate)
                    {
                        log.Debug("AD Lookup rule enabled, account " + model.Data.UserName + " already exists however the days valid threshold has been exceeded, performing LDAP query for new metadata");
                        LdapQuery();
                    }                               
                }
                else
                {
                    log.Debug("AD Lookup rule enabled, account " + model.Data.UserName + " does not exist in LDAP database");
                    LdapQuery();

                }
            }
        }

        private void LdapQuery()
        {
            try
            {
                LdapUtil util = new LdapUtil(config.LDAP_SERVER, config.LDAP_USERNAME, config.LDAP_PASSWORD);
                UserManager mgr = new UserManager(util, model.Data.UserName);
                UserMetaData user = mgr.GetMetaData();
                model.Data.AccountModel = new AccountModel();
                model.Data.AccountModel.City = user.City;
                model.Data.AccountModel.Country = user.Country;
                model.Data.AccountModel.Department = user.Department;
                model.Data.AccountModel.EmailAddress = user.EmailAddress;
                model.Data.AccountModel.FirstName = user.FirstName;
                model.Data.AccountModel.JobTitle = user.JobTitle;
                model.Data.AccountModel.LastLogon = user.LastLogon;
                model.Data.AccountModel.LastName = user.LastName;
                model.Data.AccountModel.Manager = user.Manager;
                model.Data.AccountModel.PhoneNumber = user.PhoneNumber;
                model.Data.AccountModel.StateProvince = user.StateProvince;
                model.Data.AccountModel.StreetAddress = user.StreetAddress;
                model.Data.AccountModel.OrganizationalUnit = user.OrganizationalUnit;
                model.Data.AccountModel.Groups = user.Groups;
            }
            catch (System.DirectoryServices.ActiveDirectory.ActiveDirectoryObjectNotFoundException e)
            {
                log.Warn("Account " + model.Data.UserName + " not found", e);
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException e)
            {
                log.Fatal("An error occured while performing a lookup against Active Directory", e);
                Environment.Exit(1);
            }
        }



    }

    class DNSLookup : Rule
    {
        private DetectionModel model;

        public DNSLookup(DetectionModel _model)
        {
            model = _model;
        }

        public override bool IsEnabled()
        {
            return Convert.ToBoolean(config.RULE_DNS_LOOKUP);
        }

        public override void Execute()
        {
            try
            {
                List<string> resolvedIps = new List<string>();
                List<IPAddress> ips = Dns.GetHostAddresses(model.Data.ComputerName).ToList();
                ips.ForEach(x => resolvedIps.Add(x.ToString()));

                if (resolvedIps.Count > 1)
                {
                    log.Warn("[" + model.Metadata.Offset + "] " + model.Data.ComputerName + " resolved to multiple IP addresses, only the first one will be recorded");
                }

                log.Debug("Resolved IP address of " + resolvedIps[0] + " for host " + model.Data.ComputerName); 
                model.Data.IPAddress = resolvedIps[0];
            }

            catch (ArgumentNullException)
            {
                log.Warn("[" + model.Metadata.Offset + "] Provided hostname is empty, skipping DNS lookup");
            }

            catch (System.Net.Sockets.SocketException)
            {
                log.Warn("[" + model.Metadata.Offset + "] Unable to resolve IP address for host " + model.Data.ComputerName);
            }
        }

    }

    class Taxonomize : Rule
    {
        DetectionModel model;

        public Taxonomize(DetectionModel _model)
        {
            model = _model;
        }

        public override bool IsEnabled()
        {
            return Convert.ToBoolean(config.RULE_TAXONOMIZE);
        }

        public override void Execute()
        {
            CheckTaxonomy("Username", model.Data.UserName);
            CheckTaxonomy("Hostname", model.Data.ComputerName);
            if (model.Data.AccountModel != null)
            {
                CheckTaxonomy("Active Directory OU", model.Data.AccountModel.OrganizationalUnit);

                if (model.Data.AccountModel.Groups != null)
                {
                    CheckTaxonomy("Active Directory Group", model.Data.AccountModel.Groups);
                }
            }
        }

        protected void CheckTaxonomy(string taxonomyTypeString, string modelProperty)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<Taxonomy> patterns = db.Taxonomies.Where(x => x.TaxonomyType.Type.Equals(taxonomyTypeString)).ToList();
                foreach (Taxonomy p in patterns)
                {
                    if (Regex.IsMatch(modelProperty, p.Value))
                    {
                        log.Debug("[" + model.Metadata.Offset + "]" + " detection matched a taxonomy rule " + p.Description);
                        if (p.Critical.Equals(true))
                        {
                            log.Debug("[" + model.Metadata.Offset + "]" + " detection matched a critical taxonomy rule " + p.Description);
                            model.Data.Severity = 5;
                            model.Data.SeverityName = "Critical";
                        };
                        model.Data.TaxonomyIds = new List<int>();
                        model.Data.TaxonomyIds.Add(p.TaxonomyId);
                    }
                }
            }
        }

        protected void CheckTaxonomy(string taxonomyTypeString, List<string> modelProperty)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                List<Taxonomy> patterns = db.Taxonomies.Where(x => x.TaxonomyType.Type.Equals(taxonomyTypeString)).ToList();
                model.Data.TaxonomyIds = new List<int>();
                foreach (Taxonomy p in patterns)
                {
                    foreach (string line in modelProperty)
                    {
                        if (Regex.IsMatch(line, p.Value))
                        {
                            log.Debug("[" + model.Metadata.Offset + "]" + " detection matched a taxonomy rule " + p.Description);
                            if (p.Critical.Equals(true))
                            {
                                log.Debug("[" + model.Metadata.Offset + "]" + " detection matched a critical taxonomy rule " + p.Description);
                                model.Data.Severity = 5;
                                model.Data.SeverityName = "Critical";
                            };
                            model.Data.TaxonomyIds.Add(p.TaxonomyId);
                        }
                    }
                }
            }
        }
    }

    class Whitelisting : Rule
    {
        private DetectionModel model;

        public Whitelisting(DetectionModel _model)
        {
            model = _model;
        }

        public override bool IsEnabled()
        {
            return Convert.ToBoolean(config.RULE_WHITELISTING);
        }

        public override void Execute()
        {

            List<Whitelisting> whitelists = new List<Whitelisting>();
            whitelists.Add(new FileNameWhitelistRule(model));
            whitelists.Add(new FilePathWhitelistRule(model));
            whitelists.Add(new HashWhitelistRule(model));
            whitelists.Add(new CommandLineWhitelistRule(model));
            foreach (Whitelisting list in whitelists)
            {
                if (list.Match())
                {
                    log.Debug("[" + model.Metadata.Offset + "]" + " detection matched a whitelisting rule ");
                    model.Data.StatusId = 7;
                    model.Data.ClosedDate = DateTime.UtcNow;
                }
            }
        }

        public virtual bool Match() { return false; }
    }

    class FileNameWhitelistRule : Whitelisting
    {
        private DetectionModel model;

        public FileNameWhitelistRule(DetectionModel _model)
            : base(_model)
        {
            model = _model;
        }

        public override bool Match()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Whitelists.ToList().Any(x => Regex.IsMatch(model.Data.FileName, Regex.Replace(x.Value, @"\\", @"\\")));
            }
        }

    }

    class FilePathWhitelistRule : Whitelisting
    {
        private DetectionModel model;

        public FilePathWhitelistRule(DetectionModel _model)
            : base(_model)
        {
            model = _model;
        }

        public override bool Match()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Whitelists.ToList().Any(x => Regex.IsMatch(model.Data.FilePath, Regex.Replace(x.Value, @"\\", @"\\")));
            }
        }

    }

    class HashWhitelistRule : Whitelisting
    {
        private DetectionModel model;

        public HashWhitelistRule(DetectionModel _model)
            : base(_model)
        {
            model = _model;
        }

        public override bool Match()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Whitelists.ToList().Any(x => Regex.IsMatch(model.Data.SHA256String, Regex.Replace(x.Value, @"\\", @"\\")));
            }
        }
    }

    class CommandLineWhitelistRule : Whitelisting
    {
        private DetectionModel model;

        public CommandLineWhitelistRule(DetectionModel _model)
            : base(_model)
        {
            model = _model;
        }
        public override bool Match()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Whitelists.ToList().Any(x => Regex.IsMatch(model.Data.CommandLine, Regex.Replace(x.Value, @"\\", @"\\")));
            }
        }

    }

}
