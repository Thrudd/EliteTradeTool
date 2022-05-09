using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace EliteTrading.Models.Entities {
    public class ApplicationUser : IdentityUser {
        [Required, MaxLength(50), Display(Name = "Commander Name")]
        public string CommanderName { get; set; }
        public int Reputation { get; set; }
        public int ReputationNeeded { get; set; }
        public int Rank { get;set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(100)]
        public string Badge { get; set; }
        public int Queries { get; set; }
        public int Updates { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}