using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminImportStationCommodity {
        public int StationId { get; set; }
        public int CommodityId { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public string Supply { get; set; }
        public int? SupplyAmount { get; set; }
        public string Demand { get; set; }
        public int? DemandAmount { get; set; }
        public DateTime Date { get; set; }
    }
}