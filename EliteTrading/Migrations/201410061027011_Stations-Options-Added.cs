namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StationsOptionsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "HasMarket", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Stations", "HasOutfitting", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Stations", "HasShipyard", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stations", "HasShipyard");
            DropColumn("dbo.Stations", "HasOutfitting");
            DropColumn("dbo.Stations", "HasMarket");
        }
    }
}
