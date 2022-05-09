using EliteTrading.Models.Admin;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class AdminImportResult {
        public int Updated { get; set; }
        public int Skipped { get; set; }
        public int Added { get; set; }
        public int Deleted { get; set; }
        public List<AdminImportStationOutOfRangeCommodity> OutOfRange { get; set; }
        public RepResult RepResult { get; set; }

        public AdminImportResult (){
            OutOfRange = new List<AdminImportStationOutOfRangeCommodity>();
        }
    }
}