using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Search {
    public class SearchQuery {
        public string CurrentLocation { get; set; }
        public int CommodityId { get; set; }
        public int EconomyId { get; set; }
        public int GovernmentId { get; set; }
        public int AllegianceId { get; set; }
        public string SearchType { get; set; }
        public string FactionName { get; set; }
        public bool Commodity { get; set; }
        public bool Blackmarket { get; set; }
        public bool Outfitting { get; set; }
        public bool Shipyard { get; set; }
        public bool Repairs { get; set; }
        public bool Refuel { get; set; }
        public bool Rearm { get; set; }
        public bool Economy { get; set; }
        public bool Government { get; set; }
        public bool Allegiance { get; set; }
        public double SearchRange { get; set; }
        public bool ExcludeOutposts { get; set; }
        public bool ExcludePlanets { get; set; }
    }
}