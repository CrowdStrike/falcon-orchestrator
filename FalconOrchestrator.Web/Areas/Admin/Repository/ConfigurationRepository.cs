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
using FalconOrchestratorWeb.Areas.Admin.Models;
using FalconOrchestrator.DAL;

namespace FalconOrchestratorWeb.Areas.Admin.Repository
{
    public class ConfigurationRepository
    {
        private AppConfiguration config = new AppConfiguration(System.Configuration.ConfigurationManager.AppSettings["CryptoKey"]);

        public ConfigurationItemsViewModel MapExisitingValues()
        {
            ConfigurationItemsViewModel model = new ConfigurationItemsViewModel();

            model.LDAP = new LDAPConfigViewModel();
            model.LDAP.Description = config.LDAP_DESCRIPTION;
            model.LDAP.Password = config.LDAP_PASSWORD;
            model.LDAP.Server = config.LDAP_SERVER;
            model.LDAP.Username = config.LDAP_USERNAME;
            model.LDAP.DaysValid = Convert.ToInt32(config.LDAP_DAYS_VALID);

            model.Email = new EmailConfigViewModel();
            model.Email.AlertSubject = config.EMAIL_ALERT_SUBJECT;
            model.Email.AlertTemplatePath = config.EMAIL_ALERT_TEMPLATE_PATH;
            model.Email.Password = config.EMAIL_PASSWORD;
            model.Email.Port = config.EMAIL_PORT;
            model.Email.SenderAddress = config.EMAIL_SENDER;
            model.Email.Server = config.EMAIL_SERVER;
            model.Email.SSL = Convert.ToBoolean(config.EMAIL_SSL);
            model.Email.TeamAddress = config.EMAIL_CC;
            model.Email.TicketSubject = config.EMAIL_TICKET_SUBJECT;
            model.Email.TicketTemplatePath = config.EMAIL_TICKET_TEMPLATE_PATH;
            model.Email.Username = config.EMAIL_USERNAME;
            model.Email.HomeURL = config.FALCON_ORCHESTRATOR_URL;

            model.FalconStream = new FalconStreamingAPIConfigViewModel();
            model.FalconStream.Url = config.FALCON_STREAM_URL;
            model.FalconStream.Uuid = config.FALCON_STREAM_UUID;
            model.FalconStream.Key = config.FALCON_STREAM_KEY;
            model.FalconStream.Offset = AppConfiguration.FALCON_STREAM_LAST_OFFSET;

            model.FalconForensics = new FalconForensicsConfigViewModel();
            model.FalconForensics.Username = config.FALCON_FORENSICS_USERNAME;
            model.FalconForensics.Domain = config.FALCON_FORENSICS_DOMAIN;
            model.FalconForensics.Password = config.FALCON_FORENSICS_PASSWORD;
            model.FalconForensics.ZipPassword = config.FALCON_FORENSICS_ENCRYPTION_PASSWORD;

            model.FalconQuery = new FalconQueryAPIConfigViewModel();
            model.FalconQuery.Url = config.FALCON_QUERY_URL;
            model.FalconQuery.Username = config.FALCON_QUERY_USERNAME;
            model.FalconQuery.Password = config.FALCON_QUERY_PASSWORD;

            model.ETLRules = new ETLRulesViewModel();
            model.ETLRules.ADLookup = Convert.ToBoolean(config.RULE_AD_LOOKUP);
            model.ETLRules.AssignResponder = Convert.ToBoolean(config.RULE_ASSIGN_RESPONDER);
            model.ETLRules.Taxonomize = Convert.ToBoolean(config.RULE_TAXONOMIZE);
            model.ETLRules.Whitelisting = Convert.ToBoolean(config.RULE_WHITELISTING);
            model.ETLRules.DNSLookup = Convert.ToBoolean(config.RULE_DNS_LOOKUP);
            model.ETLRules.Notification = Convert.ToBoolean(config.RULE_NOTIFICATION);
            model.ETLRules.NotificationThreshold = config.RULE_NOTIFICATION_THRESHOLD;

            return model;
        }


        public void UpdateLDAPSettings(LDAPConfigViewModel model)
        {
            config.LDAP_USERNAME = model.Username;
            config.LDAP_SERVER = model.Server;
            config.LDAP_PASSWORD = model.Password;
            config.LDAP_DESCRIPTION = model.Description;
            config.LDAP_DAYS_VALID = model.DaysValid.ToString();
        }

        public void UpdateETLRules(ETLRulesViewModel model)
        {
            config.RULE_AD_LOOKUP = model.ADLookup.ToString();
            config.RULE_ASSIGN_RESPONDER = model.AssignResponder.ToString();
            config.RULE_DNS_LOOKUP = model.DNSLookup.ToString();
            config.RULE_NOTIFICATION = model.Notification.ToString();
            config.RULE_WHITELISTING = model.Whitelisting.ToString();
            config.RULE_TAXONOMIZE = model.Taxonomize.ToString();
            config.RULE_NOTIFICATION_THRESHOLD = model.NotificationThreshold;
        }

        public void UpdateEmailSettings(EmailConfigViewModel model)
        {
            config.EMAIL_ALERT_SUBJECT = model.AlertSubject;
            config.EMAIL_ALERT_TEMPLATE_PATH = model.AlertTemplatePath;
            config.EMAIL_CC = model.TeamAddress;
            config.EMAIL_PASSWORD = model.Password;
            config.EMAIL_PORT = model.Port;
            config.EMAIL_SENDER = model.SenderAddress;
            config.EMAIL_SERVER = model.Server;
            config.EMAIL_SSL = model.SSL.ToString();
            config.EMAIL_TICKET_SUBJECT = model.TicketSubject;
            config.EMAIL_TICKET_TEMPLATE_PATH = model.TicketTemplatePath;
            config.EMAIL_USERNAME = model.Username;
            config.FALCON_ORCHESTRATOR_URL = model.HomeURL;
        }

        public void UpdateFalconStreamingAPISettings(FalconStreamingAPIConfigViewModel model)
        {
            config.FALCON_STREAM_URL = model.Url;
            config.FALCON_STREAM_UUID = model.Uuid;
            config.FALCON_STREAM_KEY = model.Key;
            AppConfiguration.FALCON_STREAM_LAST_OFFSET = model.Offset;

        }

        public void UpdateFalconForensicsSettings(FalconForensicsConfigViewModel model)
        {
            config.FALCON_FORENSICS_USERNAME = model.Username;
            config.FALCON_FORENSICS_PASSWORD = model.Password;
            config.FALCON_FORENSICS_DOMAIN = model.Domain;
            config.FALCON_FORENSICS_ENCRYPTION_PASSWORD = model.ZipPassword;

        }

        public void UpdateFalconQueryAPISettings(FalconQueryAPIConfigViewModel model)
        {
            config.FALCON_QUERY_URL = model.Url;
            config.FALCON_QUERY_USERNAME = model.Username;
            config.FALCON_QUERY_PASSWORD = model.Password;
        }

    }
}