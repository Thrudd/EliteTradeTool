using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Calculator {
    public class CalcStationCommodity {
        public int CommodityId { get; set; }
        public int StationId { get; set; }
        public string CommodityName { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public int GalacticAveragePrice { get; set; }
        public int Profit { get; set; }
        public DateTime LastUpdate { get; set; }
        public string UpdatedBy { get; set; }
    }
}