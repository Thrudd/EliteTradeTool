using Postal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EliteTrading.Models {
    public class ContactEmail : Email {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }

        [Required, Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email (optional)")]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}