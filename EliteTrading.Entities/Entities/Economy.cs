using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EliteTrading.Entities {
    public class Economy {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(20), Display(Name = "Economy")]
        public string Name { get; set; }
    }
}