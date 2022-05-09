using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.DataLists {
    public class CommodityCategory {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Commodity> Commodities { get; set; }
    }
}