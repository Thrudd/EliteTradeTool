using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class CommodityListingItem {
        public int Id { get; set; }
        
        public int StationId { get; set; }

        [Display(Name = "Station Name")]
        public string StationName { get; set; }
        
        public int CommodityId { get; set; }

        [Display(Name = "Commodity Name")]
        public string CommodityName { get; set; }

        public int Buy { get; set; }

        public int Sell { get; set; }

        public int AveragePrice { get; set; }

        [Display(Name = "Last Update")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy - HH:mm}")]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        public double Distance { get; set; }
    }
}