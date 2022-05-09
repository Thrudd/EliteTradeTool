using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EliteTrading.Extensions;
using EliteTrading.Services;
using Newtonsoft.Json;

namespace EliteTrading.Models.ViewModels {
    public class FindTradeResult {
        //public List<FindTradeItem> UniList { get; set; }
        public List<FindBiTradeItem> List { get; set; }
        public RepResult RepResult { get; set; }

        public FindTradeResult() {
            //UniList = new List<FindTradeItem>();
            List = new List<FindBiTradeItem>();
        }
    }

    public class FindBiTradeItem {
        public string OutgoingCommodityName { get; set; }
        public string ReturningCommodityName { get; set; }

        public int OutgoingBuy { get; set; }
        public int OutgoingSell { get; set; }

        public string OutgoingBuyLastUpdate { get; set; }
        public string OutgoingSellLastUpdate { get; set; }

        public int ReturningBuy { get; set; }
        public int ReturningSell { get; set; }

        public string ReturningBuyLastUpdate { get; set; }
        public string ReturningSellLastUpdate { get; set; }

        public int OutgoingProfit { get; set; }
        public int ReturningProfit { get; set; }
        public int TotalProfit { get; set; }

        public string Source { get; set; }
        public long SourceStationId { get; set; }
        public long SourceSystemId { get; set; }
        public double SourceStationDistance { get; set; }

        public string Destination { get; set; }
        public long DestinationStationId { get; set; }
        public long DestinationSystemId { get; set; }
        public double DestinationStationDistance { get; set; }

        public double Distance { get; set; }
    }

    public class FindTradeItem {
        public string CommodityName { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public string BuyLastUpdate { get; set; }
        public string SellLastUpdate { get; set; }
        public int Profit { get; set; }
        public string Source { get; set; }
        public long SourceStationId { get; set; }
        public long SourceSystemId { get; set; }
        public double SourceStationDistance { get; set; }
        public string Destination { get; set; }
        public long DestinationStationId { get; set; }
        public long DestinationSystemId { get; set; }
        public double DestinationStationDistance { get; set; }
        public double Distance { get; set; }
    }
}