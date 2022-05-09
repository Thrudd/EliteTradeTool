using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EliteTrading.Entities {
    public class System {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(50), Display(Name = "System Name")]
        public string Name { get; set; }

        [Display(Name = "Government")]
        public int GovernmentId { get; set; }
        public Government Government { get; set; }

        [Display(Name = "Allegiance")]
        public int AllegianceId { get; set; }
        public Allegiance Allegiance { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Version { get; set; }
        public bool DevData { get; set; }
 
        public DateTime? LastUpdateDate { get; set; }
        [MaxLength(50)]
        public string LastUpdateBy { get; set; }

        public bool PermitRequired { get; set; }

        [JsonIgnore]
        public ICollection<Station> Stations { get;set;}

        [NotMapped]
        public string Economy {
            get {
                List<string> economies = new List<string>();

                if (Stations != null) {
                    foreach (Station station in Stations) {
                        if (!economies.Contains(station.Economy.Name))
                            economies.Add(station.Economy.Name);

                        if (station.SecondaryEconomy != null)
                            if (!economies.Contains(station.SecondaryEconomy.Name))
                                economies.Add(station.SecondaryEconomy.Name);

                    }
                    if (economies.Count > 0) {
                        return string.Join("/", economies);
                    }
                }
                return "None";
                
            }
        }
        //public ICollection<AsteroidBelt> AsteroidBelts { get; set; }

    }
}