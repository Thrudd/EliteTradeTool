using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EliteTrading.Models.Entities {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public DbSet<ApplicationRole> Roles { get; set; }

        /* Galaxy tables */
        public DbSet<System> Systems { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<StationType> StationTypes { get; set; }
        public DbSet<Government> Governments { get; set; }
        public DbSet<Economy> Economy { get; set; }
        public DbSet<Allegiance> Allegiances { get; set; }
        public DbSet<RareTrade> RareTrades { get; set; }
        //public DbSet<AsteroidBelt> AsteroidBelts { get; set; }
        //public DbSet<AsteroidBeltType> AsteroidBeltTypes { get; set; }
        //public DbSet<ResourceExtractionType> AsteroidBeltTypes { get; set; }

        /* Economy tables */
        public DbSet<Category> Categories { get; set; }
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<StationCommodity> StationCommodities { get; set; }
        
        /* Notifications */
        public DbSet<News> News { get; set; }
        
        
        public DbSet<Status> Status { get; set; }
        public DbSet<EDDNLog> EDDNLogs { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }
}