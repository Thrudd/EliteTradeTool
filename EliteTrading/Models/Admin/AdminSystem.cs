using EliteTrading.Entities;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    [NotMapped]
    public class AdminSystem {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Economy { get; set; }

        public int GovernmentId { get; set; }
        public string GovernmentName { get; set; }

        public int AllegianceId { get; set; }
        public string AllegianceName { get; set; }

        public bool PermitRequired { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public RepResult RepResult { get; set; }

        // public List<AsteroidBelt> AsteroidBelts { get; set; }
    }
}