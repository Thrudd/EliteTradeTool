namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class readdedLoadout : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Loadouts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ShipId = c.Int(nullable: false),
                        LoadoutWeight = c.Double(nullable: false),
                        EmptyWeight = c.Double(nullable: false),
                        LoadoutValue = c.Int(nullable: false),
                        TotalValue = c.Int(nullable: false),
                        TotalCost = c.Int(nullable: false),
                        InsuranceCost = c.Int(nullable: false),
                        EmptyDistance = c.Double(nullable: false),
                        FullDistance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ships", t => t.ShipId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ShipId);
            
            CreateTable(
                "dbo.ShipFittings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadoutId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        SlotType = c.Int(nullable: false),
                        SlotId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Loadouts", t => t.LoadoutId, cascadeDelete: true)
                .Index(t => t.LoadoutId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Loadouts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShipFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.Loadouts", "ShipId", "dbo.Ships");
            DropIndex("dbo.ShipFittings", new[] { "LoadoutId" });
            DropIndex("dbo.Loadouts", new[] { "ShipId" });
            DropIndex("dbo.Loadouts", new[] { "UserId" });
            DropTable("dbo.ShipFittings");
            DropTable("dbo.Loadouts");
        }
    }
}
