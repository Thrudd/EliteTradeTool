using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Entities {
    public class StationType {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(60), Display(Name = "Type Name")]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Icon { get; set; }

        [NotMapped]
        public string Pads {
            get {
                if (Id == 2) {
                    return "Small, Medium";
                } else {
                    return "Small, Medium, Large";
                }
            }
        }


        [NotMapped]
        public string TypeName {
            get {
                switch (Id) {
                    case 1:
                        return "Station";
                    case 2:
                        return "Outpost";
                    case 3:
                        return "Surface Port";
                    default:
                        return String.Empty;
                }
            }
        }
    }
}