using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Entities {
    public class EDDNLog {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required, MaxLength(20)]
        public string Action { get; set; }

        [Required, MaxLength(100)]
        public string Message { get; set; }
    }
}