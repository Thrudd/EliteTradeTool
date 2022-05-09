using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.DataLists {
    public class DataListsResult {
        public DataListsQueryType QueryType { get; set; }
        public EliteTrading.Models.DataLists.System System { get; set; }
        public List<CommodityListItem> CommodityList { get; set; }
    }
}