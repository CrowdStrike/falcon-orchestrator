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


using System.Reflection;

namespace FalconOrchestrator.DAL
{
    public class AppConfiguration
    {
        private string cryptoKey;

        public AppConfiguration(string key)
        {
            cryptoKey = key;
        }

        public string LDAP_SERVER {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string LDAP_USERNAME
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string LDAP_PASSWORD
        {
            get
            {
                string encryptedValue = DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
                return Crypto.DecryptText(encryptedValue, cryptoKey);
            }
            set
            {
                string encrypted = Crypto.EncryptText(value, cryptoKey);
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), encrypted);
            }
        }
        public string LDAP_DESCRIPTION
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }

        public string LDAP_DAYS_VALID
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }


        public string EMAIL_SERVER
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_PORT
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_USERNAME
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_PASSWORD
        {
            get
            {
                string encryptedValue = DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
                return Crypto.DecryptText(encryptedValue, cryptoKey);
            }
            set
            {
                string encrypted = Crypto.EncryptText(value, cryptoKey);
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), encrypted);
            }
        }
        public string EMAIL_SENDER
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_CC
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_SSL
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_ALERT_SUBJECT
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_TICKET_SUBJECT
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_ALERT_TEMPLATE_PATH
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string EMAIL_TICKET_TEMPLATE_PATH
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }


        public string FALCON_STREAM_URL
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string FALCON_STREAM_UUID
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string FALCON_STREAM_KEY
        {
            get
            {
                string encryptedValue = DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
                return Crypto.DecryptText(encryptedValue, cryptoKey);
            }
            set
            {
                string encrypted = Crypto.EncryptText(value, cryptoKey);
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), encrypted);
            }
        }
        public static string FALCON_STREAM_LAST_OFFSET
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }


        public string FALCON_FORENSICS_USERNAME
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string FALCON_FORENSICS_DOMAIN
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string FALCON_FORENSICS_PASSWORD
        {
            get
            {
                string encryptedValue = DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
                return Crypto.DecryptText(encryptedValue, cryptoKey);
            }
            set
            {
                string encrypted = Crypto.EncryptText(value, cryptoKey);
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), encrypted);
            }
        }

        public string FALCON_FORENSICS_ENCRYPTION_PASSWORD
        {
            get
            {
                string encryptedValue = DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
                return Crypto.DecryptText(encryptedValue, cryptoKey);
            }
            set
            {
                string encrypted = Crypto.EncryptText(value, cryptoKey);
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), encrypted);
            }
        }


        public string FALCON_QUERY_URL
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string FALCON_QUERY_USERNAME
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string FALCON_QUERY_PASSWORD
        {
            get
            {
                string encryptedValue = DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
                return Crypto.DecryptText(encryptedValue, cryptoKey);
            }
            set
            {
                string encrypted = Crypto.EncryptText(value, cryptoKey);
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), encrypted);
            }
        }


        public string RULE_AD_LOOKUP
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string RULE_WHITELISTING
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string RULE_TAXONOMIZE
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string RULE_DNS_LOOKUP
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string RULE_ASSIGN_RESPONDER
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string RULE_NOTIFICATION
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string RULE_NOTIFICATION_THRESHOLD
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
        public string FALCON_ORCHESTRATOR_URL
        {
            get
            {
                return DatabaseHelpers.GetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
            set
            {
                DatabaseHelpers.SetConfigValue(MethodBase.GetCurrentMethod().Name.Substring(4), value);
            }
        }
    }
}
