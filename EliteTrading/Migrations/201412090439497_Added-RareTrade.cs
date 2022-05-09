namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRareTrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RareTrades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 40),
                        Buy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stations", t => t.StationId, cascadeDelete: true)
                .Index(t => t.StationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RareTrades", "StationId", "dbo.Stations");
            DropIndex("dbo.RareTrades", new[] { "StationId" });
            DropTable("dbo.RareTrades");
        }
    }
}
