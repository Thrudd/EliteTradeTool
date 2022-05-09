using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.DataLists {
    public class CommodityListItem {
        public int Id { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public int GalacticAveragePrice { get; set; }
        public string StationTypeName { get; set; }
        public string StationTypeIcon { get; set; }
        public string StationAllegiance { get; set; }
        public string Supply { get; set; }
        public int SupplyAmount { get; set; }
        public string Demand { get; set; }
        public int DemandAmount { get; set; }
        public string LastUpdate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Location { get; set; }
        public double DistanceFromJumpIn { get; set; }
        public double Distance { get; set; }
        public bool PermitRequired { get; set; }
    }
}