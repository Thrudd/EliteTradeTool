namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondaryEconomyAddedToStations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "SecondaryEconomyId", c => c.Int());
            CreateIndex("dbo.Stations", "SecondaryEconomyId");
            AddForeignKey("dbo.Stations", "SecondaryEconomyId", "dbo.Economies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stations", "SecondaryEconomyId", "dbo.Economies");
            DropIndex("dbo.Stations", new[] { "SecondaryEconomyId" });
            DropColumn("dbo.Stations", "SecondaryEconomyId");
        }
    }
}
