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
using System.Text;
using System.Net;
using System.IO;


namespace FalconOrchestrator.IOC
{
    public class ApiUtil
    {
        private string userName;
        private string password;
        public string baseUrl { get; set; }

        public ApiUtil(string _username, string _password, string baseUrl)
        {
            userName = _username;
            password = _password;
            this.baseUrl = baseUrl;
            
        }

        public enum Method
        {
            GET,
            POST,
            PATCH,
            DELETE
        }

        private HttpWebRequest Request(string endpoint, Method method)
        {
            //Get credenttials and format for basic authentication header
            string authInfo = userName + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            //Make HTTP Request and add API keys as headers
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Headers["Authorization"] = "Basic " + authInfo;
            request.ContentType = "application/json";
            request.Method = method.ToString();
            return request;
        }

        private String Response(HttpWebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();        
        }

        public String Get(string endpoint)
        {

            HttpWebRequest request = Request(endpoint,Method.GET);
            return Response(request);

        }

        public String Post(string endpoint, string json)
        {
            HttpWebRequest request = Request(endpoint, Method.POST);
            
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }
            return Response(request);
          
        }

        public String Delete(string endpoint)
        {
            HttpWebRequest request = Request(endpoint, Method.DELETE);
            return Response(request);
        }

        public String Update(string endpoint, string json)
        {
            HttpWebRequest request = Request(endpoint, Method.PATCH);
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(json);
            }
            return Response(request);
        }

        public List<string> ErrorHandler(ApiResponse response)
        {
            if (response.errors.Length > 0)
            {
                List<string> errors = new List<String>();
                foreach (Errors error in response.errors)
                {
                    errors.Add("[" + error.code + "] " + error.message + "\n");
                }
                return errors;
            }
            else
            {
                return null;
            }
            
        }

    }
}
