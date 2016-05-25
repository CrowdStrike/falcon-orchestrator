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


using System.Net;
using System.IO;
using FalconOrchestrator.DAL;
using log4net;

namespace FalconOrchestrator.Client
{
    class Connection
    {
        private HttpWebRequest request;
        private string token;
        private string dataFeedUrl;
        private static readonly ILog log = LogManager.GetLogger(typeof(Connection));

        public Connection(Authentication setup)
        {
            token = setup.Token;
            dataFeedUrl = setup.DataFeed + "&offset=" + AppConfiguration.FALCON_STREAM_LAST_OFFSET;
        }

        private void AuthenticateRequest()
        {
            request = (HttpWebRequest)HttpWebRequest.Create(dataFeedUrl);
            request.Headers.Add("Authorization", "Token " + token);
            request.KeepAlive = true;
        }

        public Stream GetStream()
        {
            try
            {
                AuthenticateRequest();
                WebResponse response = request.GetResponse();
                Stream feed = response.GetResponseStream();
                return feed;
            }
            catch (WebException e)
            {
                log.Fatal("Error while connecting to API stream ",e);
                System.Environment.Exit(1);
                throw;
            }
        }
    }
}
