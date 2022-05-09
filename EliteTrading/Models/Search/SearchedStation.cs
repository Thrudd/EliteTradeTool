using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Search {
    public class SearchedStation {
        public string SystemName { get; set; }
        public int Buy { get; set; }
        public string Supply { get; set; }
        public int SupplyAmount { get; set; }
        public int Sell { get; set; }
        public string Demand { get; set; }
        public int DemandAmount { get; set; }
        public string LastUpdate { get; set; }
        public Station Station { get; set; }
        public EliteTrading.Entities.System System { get; set; }
        public double Distance { get; set; }
    }
}