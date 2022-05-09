using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminImportStationCommodities {
        public int StationId { get; set; }
        public List<StationCommodity> StationCommodities { get; set; }
    }
}