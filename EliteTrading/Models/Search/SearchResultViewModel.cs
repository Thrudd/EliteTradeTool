using EliteTrading.Entities;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Search {
    public class SearchResultViewModel {
        public EliteTrading.Entities.System StartingSystem { get; set; }
        public SearchQuery Query { get; set; }
        public List<SearchedStation> Results { get; set; }
        public RepResult RepResult { get; set; }
    }
}