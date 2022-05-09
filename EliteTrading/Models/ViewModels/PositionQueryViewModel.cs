using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class PositionQueryViewModel {
        [Required, Display(Name = "Distance")]
        public double Point1Distance { get; set; }
        [Required, Display(Name = "Distance")]
        public double Point2Distance { get; set; }
        [Required, Display(Name = "Distance")]
        public double Point3Distance { get; set; }
        [Required, Display(Name = "Distance")]
        public double Point4Distance { get; set; }

        [Required, Display(Name = "System 2")]
        public int Point2Id { get; set; }
        [Required, Display(Name = "System 3")]
        public int Point3Id { get; set; }
        [Required, Display(Name = "System 4")]
        public int Point4Id { get; set; }
    }
}