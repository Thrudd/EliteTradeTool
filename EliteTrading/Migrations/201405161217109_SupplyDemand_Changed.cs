namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupplyDemand_Changed : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Demands", newName: "SupplyDemands");
            AddColumn("dbo.StationCommodities", "SupplyId", c => c.Int(nullable: false));
            CreateIndex("dbo.StationCommodities", "SupplyId");
            AddForeignKey("dbo.StationCommodities", "SupplyId", "dbo.SupplyDemands", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StationCommodities", "SupplyId", "dbo.SupplyDemands");
            DropIndex("dbo.StationCommodities", new[] { "SupplyId" });
            DropColumn("dbo.StationCommodities", "SupplyId");
            RenameTable(name: "dbo.SupplyDemands", newName: "Demands");
        }
    }
}
