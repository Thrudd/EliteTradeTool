using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class FindHighestTradeCommodityQuery {
        public int SourceStationId { get; set; }
        public int DestinationStationId { get; set; }
    }
}