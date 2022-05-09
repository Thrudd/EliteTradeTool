using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminImportData {
        public int Id { get; set; }
        public string StationName { get; set; }
        public string SystemName { get; set; }
        public List<AdminImportStationCommodity> Commodities { get; set; }
    }
}