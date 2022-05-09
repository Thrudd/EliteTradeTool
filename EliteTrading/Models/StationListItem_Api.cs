using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class StationListItem_Api {
        public int StationId { get; set; }
        public string Station { get; set; }
        public string System { get; set; }
        public int SystemId { get; set; }
    }
}