using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class HighestTradeCommodity {
        public string CommodityName { get; set; }
        public string Source { get; set; }
        public int SourceStationId { get; set; }
        public double SourceStationDistance { get; set; }
        public string Destination { get; set; }
        public int DestinationStationId { get; set; }
        public double DestinationStationDistance {get;set;}
        public int Profit { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public string BuyLastUpdate { get; set; }
        public string SellLastUpdate { get; set; }
    }
}