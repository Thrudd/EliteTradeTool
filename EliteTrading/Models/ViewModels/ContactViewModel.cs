using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class ContactViewModel {
        [Required, Display(Name = "Name"), StringLength(50), DataType(DataType.Text)]
        public string Name { get; set; }

        [Required, Display(Name = "Email"), StringLength(100), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, Display(Name = "Message/Suggestion"), StringLength(1000), DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}