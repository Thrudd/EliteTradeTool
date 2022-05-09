using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class WhatToBuyStationByCommodityViewModel {
        public Station Station { get; set; }
        public List<Tuple<string,List<CommodityRoute>>> CommodityRoutes { get; set; }
    }
}