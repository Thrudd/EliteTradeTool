using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class RoleViewModel {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }

        public int Rank { get; set; }

        [Display(Name = "Rep Required")]
        public int RepRequired { get; set; }

        public string Badge { get; set; }
    }
}