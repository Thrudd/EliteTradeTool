using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Calculator {
    public class CalcStation {
        public int StationId { get; set; }
        public int SystemId { get; set; }
        public string StationName { get; set; }
        public string SystemName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Distance { get; set; }
        public double DistanceFromJumpIn { get; set; }
    }
}
