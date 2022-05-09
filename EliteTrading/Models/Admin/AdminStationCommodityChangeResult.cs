using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminStationCommodityChangeResult {
        public int StationCommodityId { get; set; }
        public string Action { get; set; }
        public int Value { get; set; }
        public string LastUpdate { get; set; }
        public string UpdatedBy { get; set; }
        public RepResult RepResult { get; set; }
    }
}