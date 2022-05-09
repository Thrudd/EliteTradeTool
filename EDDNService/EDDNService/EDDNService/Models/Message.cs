using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EDDNService.Models {
    class message {
        [JsonProperty("commodities")]
        public List<commodity> Commodities { get; set; }
        //[JsonProperty("modules")]
        //public List<module> Modules { get; set; }
        //[JsonProperty("ships")]
        //public List<string> Ships { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("systemName")]
        public string SystemName { get; set; }
        [JsonProperty("stationName")]
        public string StationName { get; set; }
    }
}

