using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Entities {
    public class RareTrade {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int StationId { get; set; }
        public Station Station { get; set; }
        [Required, MaxLength(40)]
        public string Name { get; set; }
        public int Buy { get; set; }
    }
}