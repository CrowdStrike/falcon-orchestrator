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
using Newtonsoft.Json;


namespace FalconOrchestrator.IOC
{
    public class IndicatorsAPI
    {
        private ApiUtil api;

        public IndicatorsAPI(ApiUtil util)
        {
            api = util;
        }

        public enum Types
        {
            DOMAIN,
            IPV4,
            IPV6,
            MD5,
            SHA1,
            SHA256
        }

        public enum Policies
        {
            DETECT,
            NONE
        }

        public enum ShareLevels
        {
            RED
        }

        public List<Indicator> List()
        {
            string endpoint = string.Concat(api.baseUrl,"/indicators/queries/iocs/v1?");
            string json = api.Get(endpoint + "limit=1");
            ApiResponseItem response = JsonConvert.DeserializeObject<ApiResponseItem>(json);
            int limit = response.meta.pagination.total;
            json = api.Get(endpoint + "limit=" + limit);
            response = JsonConvert.DeserializeObject<ApiResponseItem>(json);
            return Get(response.resources).ToList();
        }

        public Indicator[] Get(List<string> ids)
        {
            string endpoint = string.Concat(api.baseUrl, "/indicators/entities/iocs/v1?ids=");
            int count = 1;
            foreach(string id in ids)
            {
                endpoint += id;
                if(count < ids.Count)
                    endpoint += "&ids=";
                count++;
            }
            ApiResponseIndicators response = JsonConvert.DeserializeObject<ApiResponseIndicators>(api.Get(endpoint));
            return response.resources;
        }

        public Indicator Get(string id)
        {
            string endpoint = string.Concat(api.baseUrl, "/indicators/entities/iocs/v1?ids=",id);
            ApiResponseIndicators response = JsonConvert.DeserializeObject<ApiResponseIndicators>(api.Get(endpoint));
            List<string> errors = api.ErrorHandler(response);
            if (errors != null)
                throw new Exception(String.Join(String.Empty, errors));
            return response.resources[0];
        }


        public void Upload(List<Indicator> ioc)       
        {
            string endpoint = string.Concat(api.baseUrl, "/indicators/entities/iocs/v1?");
            string json = JsonConvert.SerializeObject(ioc, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            ApiResponseItem response = JsonConvert.DeserializeObject<ApiResponseItem>(api.Post(endpoint, json));
            List<string> errors = api.ErrorHandler(response);
            if (errors != null)
                throw new Exception(String.Join(String.Empty, errors));
        }

        public void Delete(string id)
        {
            string endpoint = string.Concat(api.baseUrl, "/indicators/entities/iocs/v1?ids=", id);
            ApiResponseItem response = JsonConvert.DeserializeObject<ApiResponseItem>(api.Delete(endpoint));
            List<string> errors = api.ErrorHandler(response);
            if (errors != null)
                throw new Exception(String.Join(String.Empty, errors));
            
        }

        public ApiResponseItem Update(Indicator ioc, string ids)
        {
            string endpoint = string.Concat(api.baseUrl, "/indicators/entities/iocs/v1?ids=", ids);
            string json = JsonConvert.SerializeObject(ioc, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            ApiResponseItem response = JsonConvert.DeserializeObject<ApiResponseItem>(api.Update(endpoint, json));
            api.ErrorHandler(response);
            return response;
        }
    }

    public class ApiResponse
    {
        [JsonProperty("meta")]
        public Meta meta { get; set; }

        [JsonProperty("errors")]
        public Errors[] errors { get; set; }

    }

    public class ApiResponseItem : ApiResponse
    {
        [JsonProperty("resources")]
        public List<String> resources { get; set; }

    }

    public class ApiResponseIndicators : ApiResponse
    {
        [JsonProperty("resources")]
        public Indicator[] resources { get; set; }
    }


    public class Meta
    {
        [JsonProperty("query_time")]
        public float query_time { get; set; }

        [JsonProperty("pagination")]
        public Pagination pagination { get; set; }

        [JsonProperty("trace_id")]
        public String trace_id { get; set; }

        [JsonProperty("entity")]
        public String entity { get; set; }
    }

    public class Pagination
    {
        [JsonProperty("offset")]
        public int offset { get; set; }

        [JsonProperty("limit")]
        public int limit { get; set; }

        [JsonProperty("total")]
        public int total { get; set; }
    }

    public class Errors
    {
        [JsonProperty("code")]
        public int code { get; set; }

        [JsonProperty("message")]
        public String message { get; set; }

    }


    public class Indicator
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("value")]
        public String Value { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("share_level")]
        public String ShareLevel { get; set; }

        [JsonProperty("source")]
        public String Source { get; set; }

        [JsonProperty("policy")]
        public String Policy { get; set; }

        [JsonProperty("expiration_timestamp")]
        public DateTime? ExpirationTimestamp { get; set; }

        [JsonProperty("expiration_days")]
        public int ExpirationDays { get; set; }

        [JsonProperty("created_timestamp")]
        public DateTime? CreatedTimestamp { get; set; }

        [JsonProperty("created_by")]
        public String CreatedBy { get; set; }

        [JsonProperty("modified_timestamp")]
        public DateTime? ModifiedTimestamp { get; set; }

        [JsonProperty("modified_by")]
        public String ModifiedBy { get; set; }
    }

}
