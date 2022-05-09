using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminCategoryCommodityList {
        public string Text { get; set; }
        public int Value { get; set; }
        public List<AdminCommodityList> Commodities { get; set; }

        public AdminCategoryCommodityList (){
            Commodities = new List<AdminCommodityList>();
        }
    }
}