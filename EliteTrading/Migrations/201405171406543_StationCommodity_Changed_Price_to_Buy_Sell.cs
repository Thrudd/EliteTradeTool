namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StationCommodity_Changed_Price_to_Buy_Sell : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StationCommodities", "Buy", c => c.Int(nullable: false));
            AddColumn("dbo.StationCommodities", "Sell", c => c.Int(nullable: false));
            DropColumn("dbo.StationCommodities", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StationCommodities", "Price", c => c.Int(nullable: false));
            DropColumn("dbo.StationCommodities", "Sell");
            DropColumn("dbo.StationCommodities", "Buy");
        }
    }
}
