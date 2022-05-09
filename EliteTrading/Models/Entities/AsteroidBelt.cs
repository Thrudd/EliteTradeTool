using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Entities {
    public class AsteroidBelt {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SystemId { get; set; }
        public AsteroidBeltType AsteroidBeltType { get; set; }
    }

    public class AsteroidBeltType {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get;set; }
    }
}