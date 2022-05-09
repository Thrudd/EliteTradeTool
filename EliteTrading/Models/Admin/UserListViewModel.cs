using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.ViewModels {
    public class UserListViewModel {
        public List<ApplicationUser> Users { get; set; }

        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}