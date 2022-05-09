using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EliteTrading.Models.Entities {
    public class Category {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, MaxLength(50), Display(Name = "Category Name")]
        public string Name { get; set; }

        public ICollection<Commodity> Commodities { get; set; }
    }
}