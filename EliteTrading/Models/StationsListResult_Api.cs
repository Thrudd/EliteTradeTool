using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class StationsListResult_Api {
        public List<StationListItem_Api> Stations { get; set; }
        public bool Success { get; set; }
        public string Message {get;set;}
    }
}