using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Entities {
    public class ApplicationRole : IdentityRole {
        public int Rank { get; set; }
        public int RepRequired { get; set; }
        public string Badge { get; set; }

        public ApplicationRole(){}

        public ApplicationRole(string roleName)
            : base(roleName) {
        }
    }
}