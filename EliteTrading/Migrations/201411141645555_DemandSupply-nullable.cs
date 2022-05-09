namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DemandSupplynullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StationCommodities", "DemandSupplyId", "dbo.DemandSupplies");
            DropIndex("dbo.StationCommodities", new[] { "DemandSupplyId" });
            AlterColumn("dbo.StationCommodities", "DemandSupplyId", c => c.Int());
            CreateIndex("dbo.StationCommodities", "DemandSupplyId");
            AddForeignKey("dbo.StationCommodities", "DemandSupplyId", "dbo.DemandSupplies", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StationCommodities", "DemandSupplyId", "dbo.DemandSupplies");
            DropIndex("dbo.StationCommodities", new[] { "DemandSupplyId" });
            AlterColumn("dbo.StationCommodities", "DemandSupplyId", c => c.Int(nullable: false));
            CreateIndex("dbo.StationCommodities", "DemandSupplyId");
            AddForeignKey("dbo.StationCommodities", "DemandSupplyId", "dbo.DemandSupplies", "Id", cascadeDelete: true);
        }
    }
}
