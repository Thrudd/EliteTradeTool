using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminStationCommodity {
        public int Id {get;set;}
        public int StationId {get;set;}
        public int CommodityId {get;set;}
        public int Buy{get;set;}
        public int Sell {get;set;}
        public int CategoryId {get;set;}
    }
}