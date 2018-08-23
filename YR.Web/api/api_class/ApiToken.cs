using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YR.Web.api.api_class
{
    public class ApiToken
    {
        public string access_token { get; set; }

        public long expires_in { get; set; }

        public string refresh_token { get; set; }

        [JsonIgnore]
        public DateTime create_time { get; set; }

        [JsonIgnore]
        public DateTime update_time { get; set; }

        [JsonIgnore]
        public string client_id { get; set; }
    }
}