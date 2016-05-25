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
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;

namespace FalconOrchestrator.LDAP
{
    public class LdapUtil
    {
        private string server;
        private string username;
        private string password;

        public DirectoryEntry Connection => GetDirectoryEntry();

        public LdapUtil(string _server, string _username, string _password)
        {
            server = _server;
            username = _username;
            password = _password;
        }

        private DirectoryEntry GetDirectoryEntry()
        {
            return new DirectoryEntry("LDAP://" + server, username, password, AuthenticationTypes.Secure);
        }

        public List<string> GetListOfOrganizationalUnits()
        {
            List<string> ActiveOUs = new List<string>();

            using (DirectoryEntry conn = this.Connection)
            {
                using (DirectorySearcher searcher = new DirectorySearcher(conn))
                {
                    searcher.SearchScope = SearchScope.Subtree;
                    searcher.PropertiesToLoad.Add("ou");
                    searcher.PropertiesToLoad.Add("distinguishedName");
                    searcher.Filter = "(objectClass=organizationalUnit)";

                    foreach (SearchResult result in searcher.FindAll())
                    {
                        string dsName = result.Properties["distinguishedName"][0].ToString();
                        ActiveOUs.Add(dsName);
                    }
                }
            }
            return ActiveOUs;
        }

        public List<string> GetMembersOfOU(string ouPath)
        {
            List<string> accounts = new List<string>();
            using (DirectoryEntry conn = this.Connection)
            {
                conn.Path += "/" + ouPath;
                using (DirectorySearcher searcher = new DirectorySearcher(conn))
                {
                    searcher.SearchScope = SearchScope.OneLevel;
                    searcher.PropertiesToLoad.Add("userPrincipalName");
                    searcher.Filter = "(&(objectCategory=Person)(objectClass=user))";

                    foreach (SearchResult result in searcher.FindAll())
                    {
                        string userName = result.Properties["userPrincipalName"][0].ToString();
                        accounts.Add(userName.Split('@')[0]);
                    }
                }

            }
            return accounts;

        }

        public string GetEmailAddressOfDN(string dn)
        {
            using (DirectoryEntry conn = this.Connection)
            {
                conn.Path += "/" + dn;
                if (conn.Properties["mail"].Count > 0)
                {
                    return conn.Properties["mail"][0].ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public Boolean CheckIfDNExists(string dn)
        {
            using (DirectoryEntry conn = this.Connection)
            {
                conn.Path += "/" + dn;
                try
                {
                    Guid guid = conn.Guid;
                    return true;
                }
                catch (DirectoryServicesCOMException)
                {
                    return false;
                }
            }

        }

    }

    public class UserManager
    {
        private SearchResult userObject;
        private LdapUtil util;

        public UserManager(LdapUtil _util, string accountName)
        {
            util = _util;

            using (DirectoryEntry conn = util.Connection)
            {
                using (DirectorySearcher searcher = new DirectorySearcher(conn))
                {
                    userObject = GetUserObject(searcher, accountName);
                }
            }
        }

        public UserMetaData GetMetaData()
        {
            UserMetaData model = new UserMetaData();

            if (userObject.Properties["manager"].Count > 0)
            {
                string managerDn = userObject.Properties["manager"][0].ToString();
                model.Manager = util.GetEmailAddressOfDN(managerDn);
            }

            if (userObject.Properties["userPrincipalName"].Count > 0)
            {
                model.AccountName = userObject.Properties["userPrincipalName"][0].ToString();
            }

            if (userObject.Properties["sn"].Count > 0)
            {
                model.LastName = userObject.Properties["sn"][0].ToString();
            }

            if (userObject.Properties["givenName"].Count > 0)
            {
                model.FirstName = userObject.Properties["givenName"][0].ToString();
            }

            if (userObject.Properties["department"].Count > 0)
            {
                model.Department = userObject.Properties["department"][0].ToString();
            }

            if (userObject.Properties["title"].Count > 0)
            {
                model.JobTitle = userObject.Properties["title"][0].ToString();
            }

            if (userObject.Properties["mail"].Count > 0)
            {
                model.EmailAddress = userObject.Properties["mail"][0].ToString();
            }

            if (userObject.Properties["telephoneNumber"].Count > 0)
            {
                model.PhoneNumber = userObject.Properties["telephoneNumber"][0].ToString();
            }

            if (userObject.Properties["streetAddress"].Count > 0)
            {
                model.StreetAddress = userObject.Properties["streetAddress"][0].ToString();
            }

            if (userObject.Properties["distinguishedName"].Count > 0)
            {
                string dn = userObject.Properties["distinguishedName"][0].ToString();
                model.OrganizationalUnit = dn.Substring(dn.IndexOf("OU="));
            }

            if (userObject.Properties["l"].Count > 0)
            {
                model.City = userObject.Properties["l"][0].ToString();
            }
            if (userObject.Properties["st"].Count > 0)
            {
                model.StateProvince = userObject.Properties["st"][0].ToString();
            }

            if (userObject.Properties["co"].Count > 0)
            {
                model.Country = userObject.Properties["co"][0].ToString();
            }

            if (userObject.Properties["lastLogonTimestamp"].Count > 0)
            {
                long timestamp = Convert.ToInt64(userObject.Properties["lastLogonTimestamp"][0].ToString());
                model.LastLogon = DateTime.FromFileTime(timestamp);

            }

            if (userObject.Properties["memberOf"].Count > 0)
            {
                model.Groups = new List<string>();
                foreach (string line in userObject.Properties["memberOf"])
                {
                    model.Groups.Add(line);
                }
            }
            return model;
        }

        public void DisableAccount(string description)
        {
            using (DirectoryEntry userAccount = userObject.GetDirectoryEntry())
            {
                int val = (int)userAccount.Properties["userAccountControl"].Value;
                userAccount.Properties["userAccountControl"].Value = val | (int)ActiveDs.ADS_USER_FLAG.ADS_UF_ACCOUNTDISABLE;
                userAccount.Properties["description"].Value += Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + " - " + description;
                userAccount.CommitChanges();
            }
        }

        public void EnforcePasswordReset()
        {
            using (DirectoryEntry userAccount = userObject.GetDirectoryEntry())
            {
                userAccount.Properties["pwdLastSet"].Value = 0;
                userAccount.CommitChanges();
            }
        }

        private SearchResult GetUserObject(DirectorySearcher searcher, string accountName)
        {
            searcher.SearchScope = SearchScope.Subtree;
            searcher.Filter = string.Format("(&(objectCategory=Person)(objectClass=user)(samaccountname={0}))", accountName);
            searcher.PropertiesToLoad.AddRange(new[] {
                "sn",
                "givenName",
                "manager",
                "streetAddress",
                "pwdLastSet",
                "l",
                "st",
                "co",
                "distinguishedName",
                "userPrincipalName",
                "department",
                "displayName",
                "telephoneNumber",
                "mail",
                "lastLogonTimestamp",
                "memberOf",
                "title"
            });

            SearchResult result = searcher.FindOne();
            if (result == null)
            {
                throw new ActiveDirectoryObjectNotFoundException("Username " + accountName + " was not found in active directory database");
            }
            return result;
        }
    }


    public class UserMetaData
    {
        public string AccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string Manager { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string StateProvince { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public DateTime LastLogon { get; set; }
        public string OrganizationalUnit { get; set; }
        public List<string> Groups { get; set; }
    }
}
