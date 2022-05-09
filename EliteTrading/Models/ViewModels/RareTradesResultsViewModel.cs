using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class RareTradesResultsViewModel {
        public int Id { get; set; }
        public string RareTrade { get; set; }
        public string Location { get; set; }
        public string Allegiance { get; set; }
        public int Price { get; set; }
        public double Distance { get; set; }
        public double DistanceFromJumpIn { get; set; }
    }
}