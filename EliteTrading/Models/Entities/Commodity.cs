using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EliteTrading.Models.Entities {
    public class Commodity {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Required, MaxLength(40), Display(Name = "Commodity Name")]
        public string Name { get; set; }
        [Required, Display(Name="Galactic Average Price")]
        public int GalacticAveragePrice { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
    }
}