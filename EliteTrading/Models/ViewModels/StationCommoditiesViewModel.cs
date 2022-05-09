using EliteTrading.Entities;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class StationCommoditiesViewModel {
        public List<StationCommodityViewModel> StationCommodities { get; set; }
        public RepResult RepResult { get; set; }
    }
}