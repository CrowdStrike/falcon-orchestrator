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
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using Newtonsoft.Json;
using log4net;
using FalconOrchestrator.DAL;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace FalconOrchestrator.Client
{
    public partial class FalconOrchestratorService : ServiceBase
    {
        private Thread thread;

        public FalconOrchestratorService()
        {
            InitializeComponent();
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(FalconOrchestratorService));
        private static readonly ILog apiLog = LogManager.GetLogger("API");

        protected override void OnStart(string[] args)
        {

            if (DatabaseHelpers.TestConnection())
            {
                log.Debug("Connection to database is successful, starting service");
                thread = new Thread(Invoke);
                thread.IsBackground = true;
                thread.Start();
            }

            else
            {
                log.Fatal("Connection to database failed");
                this.ExitCode = 8409;
            }
        }

        protected override void OnStop()
        {
            log.Fatal("Client service has been stopped");
            thread.Abort();
            this.Stop();
            this.ExitCode = 0;

        }

        public static void Invoke()
        {
            try
            {
                AppConfiguration appConfig = new AppConfiguration(ConfigurationManager.AppSettings["CryptoKey"]);
                Authentication config = new Authentication(
                    String.Concat(appConfig.FALCON_STREAM_URL, "?appId=falcon_orchestrator"),
                    appConfig.FALCON_STREAM_UUID,
                    appConfig.FALCON_STREAM_KEY);

                Connection connection = new Connection(config);
                ProcessStream(connection.GetStream());
            }
            catch (Exception e)
            {
                log.Fatal("An unhandled error occured",e);
                Environment.Exit(1);
            }
        }

        private static void ProcessStream(Stream firehose)
        {
            using (StreamReader reader = new StreamReader(firehose))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        apiLog.Debug(line);
                        EventModel data = JsonConvert.DeserializeObject<EventModel>(line);

                        try
                        {
                            EventModel model = Factory.EntityMapping(data.Metadata.EventType, line);
                            if (model.Exists())
                            {
                                log.Debug("[" + data.Metadata.Offset + "] Event already stored in database");
                                continue;
                            }

                            model.Save();
                        }

                        catch (System.Data.Entity.Validation.DbEntityValidationException)
                        {
                            log.Fatal("Error saving detection event to database");
                            log.Fatal(JsonConvert.SerializeObject(data, Formatting.Indented));
                            Environment.Exit(1);
                        }

                        catch (NotSupportedException)
                        {
                            log.Warn("[" + data.Metadata.Offset + "]" + "Unhandled mapping for event type " + data.Metadata.EventType);
                        }
                    }
                }
            }
        }
    }
}
