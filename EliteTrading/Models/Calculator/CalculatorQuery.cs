using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EliteTrading.Models.ViewModels {
    //public class CalculatorQuery {
    //    public int StartStationId { get; set; }
    //    public int? EndStationId { get; set; }
    //    [Required, Range(1,9999)]
    //    public int Cargo { get; set; }
    //    [Required, Range(1, 999999999)]
    //    public int Cash { get; set; }
    //    public int MinProfit { get; set; }
    //    public double SearchRange { get; set; }
    //    public double MaxDistanceFromJumpIn { get; set; }
    //    public string PadSize { get; set; }
    //}

    public class CalculatorQuery {
        public string StartSystem { get; set; }
        public string EndSystem { get; set; }
        public int? StartStationId { get; set; }
        public int? EndStationId { get; set; }
        [Required, Range(1, 9999)]
        public int Cargo { get; set; }
        [Required, Range(1, 999999999)]
        public int Cash { get; set; }
        public int MinProfit { get; set; }
        public double SearchRange { get; set; }
        public double MaxDistanceFromJumpIn { get; set; }
        //public string PadSize { get; set; }
        public bool ExcludeOutposts {get;set;}
        public bool ExcludePlanets { get; set; }
    }
}