using EliteTrading.Entities;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class CalculatorByProfitViewModel {
        public string Location { get; set; }
        public int StartingSystemId { get; set; }
        public string StartingSystemName { get; set; }
        public string DestinationSystemName { get; set; }
        public int Cargo { get; set; }
        public int Cash { get; set; }
        public List<CalculatorResult> StationRoutes { get; set; }
        public RepResult RepResult { get; set; }

        public CalculatorByProfitViewModel (){
            StationRoutes = new List<CalculatorResult>();
        }
    }

    public class CalculatorResultViewModel {
        public List<CalculatorResult> Results { get; set; }
        public RepResult RepResult { get; set; }

        public CalculatorResultViewModel() {
            Results = new List<CalculatorResult>();
        }
    }

    public class CommodityRoute{
        public int CommodityId { get; set; }
        public String CommodityName { get; set; }
        public int DestinationId { get; set; }
        public String DestinationName { get; set; }
        public int DestinationSystemId { get; set; }
        public int Buy { get; set; }
        public int Sell { get; set; }
        public int GalacticAveragePrice { get; set; }
        public int Profit { get; set; }
        public int Qty { get; set; }
        public int Total { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy - HH:mm}")]
        public DateTime LastUpdate { get; set; }
        public String UpdatedBy { get; set; }
    }
}