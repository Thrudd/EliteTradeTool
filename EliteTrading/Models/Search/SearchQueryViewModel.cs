using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EliteTrading.Models.Search {
    public class SearchQueryViewModel {
        public List<SelectListItem> Commodities { get; set; }
        public List<SelectListItem> Economies { get; set; }
        public List<SelectListItem> Governments { get; set; }
        public List<SelectListItem> Allegiances { get; set; }

        public List<string> SearchTypes { get; set; }

        [Required, Display(Name="Current System")]
        public string CurrentLocation { get; set; }

        [Display(Name = "Faction Name")]
        public string FactionName { get; set; }

        [Display(Name = "Commodity")]
        public int CommodityId { get; set; }

        [Display(Name = "Economy")]
        public int EconomyId { get; set; }

        [Display(Name = "Government")]
        public int GovernmentId { get; set; }

        [Display(Name = "Allegiance")]
        public int AllegianceId { get; set; }

        [Display(Name = "Jump Range")]
        public double JumpRange { get; set; }

        [Display(Name = "Max Jumps")]
        public int MaxJumps { get; set; }

        [Display(Name = "Search Type")]
        public string SearchType { get; set; }

        public bool Commodity { get; set; }
        public bool Blackmarket { get; set; }
        public bool Outfitting { get; set; }
        public bool Shipyard { get; set; }
        public bool Economy { get; set; }
        public bool Government { get; set; }
        public bool Allegiance { get; set; }
        public bool Repairs { get; set; }
        public bool Refuel { get; set; }
        public bool Rearm { get; set; }
        
        public SearchQueryViewModel() {
            SearchTypes = new List<string>();
            SearchTypes.Add("Station Buying");
            SearchTypes.Add("Station Selling");
        }
    }
}