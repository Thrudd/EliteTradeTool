using EliteTrading.Models.ViewModels;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminStationCommodities {
        public List<StationCommodityViewModel> StationCommodities { get; set; }
        public string StationName { get; set; }
        public int StationId { get; set; }
        public RepResult RepResult { get; set; }
    }
}