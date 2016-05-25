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
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Collections.ObjectModel;

namespace FalconOrchestrator.Forensics
{
    public class PSRemoting
    {
        private string computerName;
        private string userName;
        private string password;
        private string domain;

        public PSRemoting(string _computerName, string _userName, string _password, string _domain = null)
        {
            computerName = _computerName;
            userName = _userName;
            password = _password;

            if(String.IsNullOrEmpty(_domain))
            {
                domain = computerName;
            }
            else
            {
                domain = _domain;
            }
        }

        private WSManConnectionInfo ConnectionInfo()
        {
            const string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";

            SecureString securePassword = new SecureString();
            password.ToList().ForEach(x => securePassword.AppendChar(x));

            PSCredential remoteCreds = new PSCredential(domain + "\\" + userName ,securePassword);
            WSManConnectionInfo connInfo = new WSManConnectionInfo(false,computerName,5985,"/wsman",shellUri,remoteCreds);
            connInfo.OpenTimeout = 1 * 60 * 1000;
            connInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            return connInfo;
        }

        public Boolean TestConnection()
        {
            try
            {
                Runspace runSpace = RunspaceFactory.CreateRunspace(ConnectionInfo());
                runSpace.Open();
                runSpace.Close();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public Collection<PSObject> ExecuteCommand(string command)
        {
            Runspace runSpace = RunspaceFactory.CreateRunspace(ConnectionInfo());
            runSpace.Open();

            Collection<PSObject> results = null;
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Runspace = runSpace;
                ps.AddScript(command);
                results = ps.Invoke();

                if (ps.Streams.Error.Count > 0)
                {
                    string errorOutput = null;
                    ps.Streams.Error.ToList().ForEach(x => errorOutput += x.ToString());
                    throw new Exception(errorOutput);
                }

            }          

            runSpace.Close();
            return results;
        }

        public static string CommandMapping(string command, Dictionary<string, string> mapping)
        {
            var output = new StringBuilder(command);

            foreach (var line in mapping)
                output.Replace(line.Key, line.Value);

            return output.ToString();
        }
    }

}
