namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StationCommoditySupplyanddemandnullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.StationCommodities", "Supply", c => c.Int());
            AlterColumn("dbo.StationCommodities", "Demand", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StationCommodities", "Demand", c => c.Int(nullable: false));
            AlterColumn("dbo.StationCommodities", "Supply", c => c.Int(nullable: false));
        }
    }
}
