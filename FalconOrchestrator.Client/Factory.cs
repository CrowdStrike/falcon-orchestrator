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
using Newtonsoft.Json;

namespace FalconOrchestrator.Client
{
    class Factory
    {

        public static EventModel EntityMapping(string eventType, string json)
        {
            switch (eventType)
            {
                case "DetectionSummaryEvent":
                    return JsonConvert.DeserializeObject<DetectionModel>(json);

                case "AuthActivityAuditEvent":
                    return JsonConvert.DeserializeObject<AuthActivityAuditModel>(json);

                case "UserActivityAuditEvent":
                    return JsonConvert.DeserializeObject<UserActivityAuditModel>(json);

                default:
                    throw new NotSupportedException();
            }

        }
    }
}
