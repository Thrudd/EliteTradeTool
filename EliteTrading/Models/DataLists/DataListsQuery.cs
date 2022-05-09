using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.DataLists {
    public class DataListsQuery {
        public string SystemName { get; set; }
        public DataListsQueryType QueryType { get; set; }
        public int CommodityId { get; set; }
        public bool ExcludeOutposts { get; set; }
        public bool ExcludePlanets { get; set; }
        public double? SearchRange { get; set; }
    }

    public enum DataListsQueryType {
        System,
        StationBuys,
        StationSells
    }
}