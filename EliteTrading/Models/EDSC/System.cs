using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.EDSC {
    public class System {
        public int cr { get; set; }
        public DateTime date { get; set; }
        public string name { get; set; }
        public double[] coord { get; set; }
    }
}