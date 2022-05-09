namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StationChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "EconomyId", c => c.Int(nullable: false, defaultValue:7));
            AddColumn("dbo.Stations", "GovernmentId", c => c.Int(nullable: false, defaultValue:6));
            AddColumn("dbo.Stations", "AllegianceId", c => c.Int(nullable: false, defaultValue:5));
            CreateIndex("dbo.Stations", "EconomyId");
            CreateIndex("dbo.Stations", "GovernmentId");
            CreateIndex("dbo.Stations", "AllegianceId");
            AddForeignKey("dbo.Stations", "AllegianceId", "dbo.Allegiances", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Stations", "EconomyId", "dbo.Economies", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Stations", "GovernmentId", "dbo.Governments", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stations", "GovernmentId", "dbo.Governments");
            DropForeignKey("dbo.Stations", "EconomyId", "dbo.Economies");
            DropForeignKey("dbo.Stations", "AllegianceId", "dbo.Allegiances");
            DropIndex("dbo.Stations", new[] { "AllegianceId" });
            DropIndex("dbo.Stations", new[] { "GovernmentId" });
            DropIndex("dbo.Stations", new[] { "EconomyId" });
            DropColumn("dbo.Stations", "AllegianceId");
            DropColumn("dbo.Stations", "GovernmentId");
            DropColumn("dbo.Stations", "EconomyId");
        }
    }
}
