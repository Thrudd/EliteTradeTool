using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    //public class CalculatorResult {
    //    public int StartingStationId { get; set; }
    //    public string StartingStationName { get; set; }
    //    public string StartingSystemName { get; set; }
    //    public int DestinationStationId { get; set; }
    //    public String DestinationStationName { get; set; }
    //    public string DestinationSystemName { get; set; }
    //    public double Distance { get; set; }
    //    public int CommodityId { get; set; }
    //    public String CommodityName { get; set; }
    //    public int Buy { get; set; }
    //    public int Sell { get; set; }
    //    public int GalacticAveragePrice { get; set; }
    //    public int Profit { get; set; }
    //    public int Qty { get; set; }
    //    public int Total { get; set; }
    //    [DisplayFormat(DataFormatString = "{0:MMM dd - HH:mm}")]
    //    public String LastUpdate { get; set; }
    //    public String UpdatedBy { get; set; }
    //    public double DistanceFromJumpIn { get; set; }
    //}

    public class CalculatorResult {
        public String CommodityName { get; set; }
        public int Buy { get; set; }
        public string Supply { get; set; }
        public int SupplyAmount { get; set; }
        public int Sell { get; set; }
        public string Demand { get; set; }
        public int DemandAmount { get; set; }
        public int GalacticAveragePrice { get; set; }
        public String SellLastUpdate { get; set; }
        public String SellUpdatedBy { get; set; }
        public String BuyLastUpdate { get; set; }
        public String BuyUpdatedBy { get; set; }
        public int Profit { get; set; }
        public string Source { get; set; }
        public string SourceSystemName { get; set; }
        public int SourceStationId { get; set; }
        public string SourceStationTypeName { get; set; }
        public string SourceStationTypeIcon { get; set; }
        public string SourceStationAllegiance { get; set; }
        public int SourceSystemId { get; set; }
        public double SourceStationDistance { get; set; }
        public string Destination { get; set; }
        public string DestinationSystemName { get; set; }
        public int DestinationStationId { get; set; }
        public string DestinationStationTypeName { get; set; }
        public string DestinationStationTypeIcon { get; set; }
        public string DestinationStationAllegiance { get; set; }
        public int DestinationSystemId { get; set; }
        public double DestinationStationDistance { get; set; }
        public double Distance { get; set; }
        public int Qty { get; set; }
        public int TotalProfit { get; set; }
        public bool SourcePermitRequired { get; set; }
        public bool DestinationPermitRequired { get; set; }
    }
}