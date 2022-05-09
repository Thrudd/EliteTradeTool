using EliteTrading.Entities;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Search {
    public class StationDetailViewModel {
        public int StationId { get; set; }
        public Station Station { get; set; }
        public CommodityList StationCommodities { get; set; }
        public SearchQueryViewModel Query { get; set; }
        public RepResult RepResult { get; set; }
    }
}