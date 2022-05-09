using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    [NotMapped]
    public class AdminStation {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SystemId { get; set; }
        public bool HasBlackmarket { get; set; }
        public bool HasMarket { get; set; }
        public bool HasOutfitting { get; set; }
        public bool HasShipyard { get; set; }
        public bool HasRepairs { get; set; }
        public bool HasRefuel { get; set; }
        public bool HasRearm { get; set; }
        public string FactionName { get; set; }
        public int AllegianceId { get; set; }
        public string Allegiance { get; set; }
        public int EconomyId { get; set; }
        public int? SecondaryEconomyId { get; set; }
        public string Economy { get; set; }
        public int GovernmentId { get; set; }
        public string Government { get; set; }
        public double DistanceFromJumpIn { get; set; }
        public int StationTypeId { get; set; }
        public string StationTypeName { get; set; }
        public int CopyMarketFromStationId { get; set; }
    }
}