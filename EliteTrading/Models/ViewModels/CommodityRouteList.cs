using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class CommodityRouteList {
        public string Commodity { get; set; }
        public List<CommodityRoute> Routes { get; set; }
    }
}