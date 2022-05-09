namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDemandSupplytoStationCommodities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DemandSupplies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.StationCommodities", "DemandSupplyId", c => c.Int(nullable: true));
            CreateIndex("dbo.StationCommodities", "DemandSupplyId");
            AddForeignKey("dbo.StationCommodities", "DemandSupplyId", "dbo.DemandSupplies", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StationCommodities", "DemandSupplyId", "dbo.DemandSupplies");
            DropIndex("dbo.StationCommodities", new[] { "DemandSupplyId" });
            DropColumn("dbo.StationCommodities", "DemandSupplyId");
            DropTable("dbo.DemandSupplies");
        }
    }
}
