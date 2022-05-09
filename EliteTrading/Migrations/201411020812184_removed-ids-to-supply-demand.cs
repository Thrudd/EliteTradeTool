namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedidstosupplydemand : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StationCommodities", "SupplyId");
            DropColumn("dbo.StationCommodities", "DemandId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StationCommodities", "DemandId", c => c.Int(nullable: false));
            AddColumn("dbo.StationCommodities", "SupplyId", c => c.Int(nullable: false));
        }
    }
}
