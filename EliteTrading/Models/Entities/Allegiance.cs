using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EliteTrading.Models.Entities {
    public class Allegiance {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(20), Display(Name = "Allegiance")]
        public string Name { get; set; }
    }
}