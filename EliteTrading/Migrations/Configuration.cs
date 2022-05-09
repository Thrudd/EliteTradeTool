namespace EliteTrading.Migrations {
    using EliteTrading.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EliteTrading.Entities.ApplicationDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EliteTrading.Entities.ApplicationDbContext context) {
            //  This method will be called after migrating to the latest version.

            //context.Categories.AddOrUpdate(
            //    new Category { Name = "Chemicals"},
            //    new Category { Name = "Consumer Items"},
            //    new Category { Name = "Drugs"},
            //    new Category { Name = "Foods"},
            //    new Category { Name = "Industrial Materials"},
            //    new Category { Name = "Machinery" },
            //    new Category { Name = "Medicines"},
            //    new Category { Name = "Metals"},
            //    new Category { Name = "Minerals"},
            //    new Category { Name = "Technology"},
            //    new Category { Name = "Textiles" },
            //    new Category { Name = "Weapons"},
            //    new Category { Name = "Waste"}
            //    );

            //context.Demands.AddOrUpdate(
            //    new SupplyDemand { Name="None" },
            //    new SupplyDemand { Name="Low" },
            //    new SupplyDemand { Name="Med" },
            //    new SupplyDemand { Name="High" }
            //    );

            //context.Economy.AddOrUpdate(
            //    new Economy { Name = "Agricultural" },
            //    new Economy { Name = "Extraction" },
            //    new Economy { Name = "High Tech" },
            //    new Economy { Name = "Industrial" },
            //    new Economy { Name = "Refinery" }
            //    );

            //context.Governments.AddOrUpdate(
            //    new Government { Name="Anarchy" },
            //    new Government { Name = "Colony" },
            //    new Government { Name="Communism" },
            //    new Government { Name="Corporate" },
            //    new Government { Name = "Democracy" }
            //    );

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
