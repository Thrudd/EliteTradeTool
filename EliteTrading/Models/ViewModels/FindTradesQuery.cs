using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class FindTradesQuery {
        public string SystemName { get; set; }
        public double SearchRange { get; set; }
        //public string PadSize { get; set; }
        public bool ExcludeOutposts { get; set; }
        public bool ExcludePlanets { get; set; }
        //public int MinProfitPerTon { get; set; }
        public double MaxDistanceFromJumpIn { get; set; }
        public double MaxDistanceBetweenSystems { get; set; }
        //public bool BiDirectional { get; set; }
    }
}