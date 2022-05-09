using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Search {
    public class CommodityList {
        public List<CommodityCategory> Category { get; set; }

        public CommodityList() {
            Category = new List<CommodityCategory>();
        }
    }

    public class CommodityCategory {
        public string Name { get; set; }
        public List<StationCommodity> StationCommodity { get; set; }

        public CommodityCategory() {
            StationCommodity = new List<StationCommodity>();
        }
    }
}