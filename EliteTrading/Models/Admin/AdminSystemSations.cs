using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminSystemSations {
        public int SystemId { get; set; }
        public int MarketStationId { get; set; }
        public List<AdminStation> Stations { get; set; }
    }
}