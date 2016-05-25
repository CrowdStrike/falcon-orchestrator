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
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using log4net;


namespace FalconOrchestrator.Client
{
    class Authentication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Authentication));
        private string response;
        private string url;
        private string id;
        private string key;

        public Authentication(string _url, string _id, string _key)
        {
            url = _url;
            id = _id;
            key = _key;
            response = AuthenticateAndGetResponse();
        }

        public string Token
        {
            get
            {
                JObject json = JObject.Parse(response);
                return (string)json.SelectToken("resources[0].sessionToken.token");
            }
        }

        public string DataFeed
        {
            get
            {
                JObject json = JObject.Parse(response);
                return (string)json.SelectToken("resources[0].dataFeedURL");
            }
        }

        private string AuthenticateAndGetResponse()
        {
            try
            {
                //Setup initial request to the API
                WebRequest request = WebRequest.Create(url);

                //Calculate the content MD5 of the body, this will always be empty since we are not using POST and only GET
                string md5 = CalculateContentMD5(String.Empty);

                //Calculate current date time and format so its RFC7231 compliant
                string date = DateTime.UtcNow.ToUniversalTime().ToString("r");


                //build the request string with the most (will always be GET), Content MD5 (will be empty), Date (as set in previous step)
                //the request URI host portion (firehose.crowdstrike.com), the path portion (/sensors/entities/datafeed/v1 and the query string (appId=test&offset=x)            
                string requestString = request.Method + "\n" + md5 + "\n" + date + "\n" +
                    request.RequestUri.Host + request.RequestUri.AbsolutePath +
                    "\n" + CanonicalQueryString(request.RequestUri.Query);


                //calculate signature using the api key and request string, see function below for details
                string signature = CalculateHMAC(key, requestString);

                //set the date header
                request.Headers["X-CS-Date"] = date;

                //set the authorization header using the api uuid and the signature calculated in the previous setp
                request.Headers["Authorization"] = "cs-hmac " + id + ":" + signature + ":customers";

                //Initiate request to API
                WebResponse response = request.GetResponse();

                //Get response as a stream and return stream to be processed further
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                return reader.ReadToEnd();
            }

            catch (WebException e)
            {
                log.Fatal("Error while authenticating to API",e);
                throw;
            }
        }

        //This function is not currently used since we only execute GET requests and not POST
        private string CalculateContentMD5(string body)
        {
            //if the body is empty, which it will be, just return an empty string
            if (String.IsNullOrEmpty(body))
                return String.Empty;

            //If its not then compute the MD5 hash of the body string
            using (MD5 md5 = MD5.Create())
            {
                byte[] input = System.Text.Encoding.ASCII.GetBytes(body);
                byte[] hash = md5.ComputeHash(input);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }

                byte[] hashBytes = Encoding.ASCII.GetBytes(builder.ToString());

                //Get the md5 and return it as a base64 encoded value
                return Convert.ToBase64String(hashBytes);
            }

        }

        private string CanonicalQueryString(string query)
        {
            //if there is no query string return an empty value
            if (String.IsNullOrEmpty(query))
                return String.Empty;

            //Otherwise, split on the ? (i.e. firehose.crowdstrike.com/sensors/entities/datafeed/v1?appId=test&offset=123) would return appId=test&offset=123
            //NOTE: this may need to vary depeneding on the library being used, however for the most part the logic is simple since all we are passing in the query string
            //is an appId and optionally the offset, there are no special characters/spaces, etc. so I left out handling all that logic
            return query.Split('?')[1];
        }

        //This takes the API key and Request string and computes the HMAC SHA256 hash
        private string CalculateHMAC(string apiKey, string message)
        {
            byte[] key = Encoding.ASCII.GetBytes(apiKey);
            byte[] req = Encoding.ASCII.GetBytes(message);

            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] hmacBytes = hmac.ComputeHash(req);
                //get the hash and return it as a base 64 encoded value
                return Convert.ToBase64String(hmacBytes);
            }

        }


    }
}
