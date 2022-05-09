using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Entities {
    public class Status {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime LastStarCheck { get; set; }
        public bool ShowHolding { get; set; }
        [MaxLength(2000)]
        public string HoldingMessage { get; set; }
    }
}