using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EliteTrading.Models.ViewModels {
    public class EditUserViewModel {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string CommanderName { get; set; }

        public int Rank { get; set; }
        public int Reputation { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}