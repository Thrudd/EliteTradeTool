using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.DataLists {
    public class Commodity {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public int AveragePrice { get; set; }
        public string Supply { get; set; }
        public int SupplyAmount { get; set; }
        public string Demand { get; set; }
        public int DemandAmount { get; set; }
        public string LastUpdate { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}