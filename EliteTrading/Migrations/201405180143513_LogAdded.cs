namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommanderName = c.String(nullable: false, maxLength: 50),
                        StationCommodityId = c.Int(nullable: false),
                        ChangeTime = c.DateTime(nullable: false),
                        OriginalBuy = c.Int(nullable: false),
                        OriginalSell = c.Int(nullable: false),
                        NewBuy = c.Int(nullable: false),
                        NewSell = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StationCommodities", t => t.StationCommodityId, cascadeDelete: true)
                .Index(t => t.StationCommodityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogEntries", "StationCommodityId", "dbo.StationCommodities");
            DropIndex("dbo.LogEntries", new[] { "StationCommodityId" });
            DropTable("dbo.LogEntries");
        }
    }
}
