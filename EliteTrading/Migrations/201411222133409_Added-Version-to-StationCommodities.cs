namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVersiontoStationCommodities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StationCommodities", "Version", c => c.Double(nullable: false, defaultValue:0.3));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StationCommodities", "Version");
        }
    }
}
