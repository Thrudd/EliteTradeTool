using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EliteTrading.Entities {
    public class Station {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(60), Display(Name = "Station Name")]
        public string Name { get; set; }

        [Display(Name = "System")]
        public int SystemId { get; set; }

        [Display(Name = "Primary Economy")]
        public int EconomyId { get; set; }
        public Economy Economy { get; set; }

        [Display(Name = "Secondary Economy")]
        public int? SecondaryEconomyId { get; set; }
        public Economy SecondaryEconomy { get; set; }

        [Display(Name = "Government")]
        public int GovernmentId { get; set; }
        public Government Government { get; set; }

        [Display(Name = "Allegiance")]
        public int AllegianceId { get; set; }
        public Allegiance Allegiance { get; set; }

        [MaxLength(100)]
        public string FactionName { get; set; }

        [JsonIgnore]
        public System System { get; set; }
        public bool HasBlackmarket { get; set; }
        public bool HasMarket { get; set; }
        public bool HasOutfitting { get; set; }
        public bool HasShipyard { get; set; }
        public bool HasRepairs { get; set; }
        public bool HasRefuel { get; set; }
        public bool HasRearm { get; set; }

        [Required, Display(Name = "Distance From Jump In")]
        public double DistanceFromJumpIn { get; set; }

        public double Version { get; set; }

        public int StationTypeId { get; set; }
        public StationType StationType { get; set; }

        public DateTime? LastUpdateDate { get; set; }
        [MaxLength(60)]
        public string LastUpdateBy { get; set; }

        public ICollection<StationCommodity> StationCommodities { get; set; }
        
        [NotMapped]
        public string Services {
            get {
                List<string> serviceList = new List<string>();
                if (HasBlackmarket)
                    serviceList.Add("Blackmarket");
                if (HasMarket)
                    serviceList.Add("Commodities Market");
                if (HasOutfitting)
                    serviceList.Add("Outfitting");
                if (HasShipyard)
                    serviceList.Add("Shipyard");
                if (HasRepairs)
                    serviceList.Add("Repairs");
                if (HasRefuel)
                    serviceList.Add("Refuel");
                if (HasRearm)
                    serviceList.Add("Rearm");
                if (serviceList.Count > 0) {
                    return string.Join(", ", serviceList);
                } else {
                    return "No services listed in the database.";
                }
            }
        }

        [NotMapped]
        public string EconomyString {
            get {
                return SecondaryEconomyId.HasValue ? Economy.Name + "/" + SecondaryEconomy.Name : Economy.Name;
            }
        }
    }
}