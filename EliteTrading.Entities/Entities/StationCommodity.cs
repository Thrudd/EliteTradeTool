using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EliteTrading.Entities {
    public class StationCommodity {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Station")]
        public int StationId { get; set; }
        public Station Station { get; set; }

        [Display(Name = "Commodity")]
        public int CommodityId { get; set; }
        public Commodity Commodity { get; set; }

        [Required]
        public int Buy { get; set; }

        [Required]
        public int Sell { get; set; }

        [StringLength(4)]
        public string Supply { get; set; }
        public int SupplyAmount { get; set; }

        [StringLength(4)]
        public string Demand { get; set; }
        public int DemandAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd - HH:mm}")]
        public DateTime LastUpdate { get; set; }
        [MaxLength(50)]
        public string UpdatedBy { get; set; }
        public double Version { get; set; }
    }
}