using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Admin {
    public class AdminImportStationOutOfRangeCommodity {
        public string Station { get; set; }
        public string Commodity { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public string Range { get; set; }
    }
}