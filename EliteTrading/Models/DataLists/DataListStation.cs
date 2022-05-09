using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.DataLists {
    public class Station {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FactionName { get; set; }
        public string Allegiance { get; set; }
        public string Economy { get; set; }
        public string Government { get; set; }
        public string StationType { get; set; }
        public string StationTypeIcon { get; set; }
        public string Services { get; set; }
        public double DistanceFromJumpIn { get; set; }
        public string Pads { get; set; }
        public List<CommodityCategory> CommodityCategories { get; set; }
    }
}