namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DroppedLoadout : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Loadouts", "ShipId", "dbo.Ships");
            DropForeignKey("dbo.ShipFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.Loadouts", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Loadouts", new[] { "UserId" });
            DropIndex("dbo.Loadouts", new[] { "ShipId" });
            DropIndex("dbo.ShipFittings", new[] { "LoadoutId" });
            DropTable("dbo.Loadouts");
            DropTable("dbo.ShipFittings");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Loadouts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ShipId = c.Int(nullable: false),
                        LoadoutWeight = c.Int(nullable: false),
                        Mass = c.Int(nullable: false),
                        LoadoutValue = c.Int(nullable: false),
                        TotalCost = c.Int(nullable: false),
                        InsuranceCost = c.Int(nullable: false),
                        EmptyDistance = c.Double(nullable: false),
                        FullDistance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.ShipFittings", "LoadoutId");
            CreateIndex("dbo.Loadouts", "ShipId");
            CreateIndex("dbo.Loadouts", "UserId");
            AddForeignKey("dbo.Loadouts", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ShipFittings", "LoadoutId", "dbo.Loadouts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Loadouts", "ShipId", "dbo.Ships", "Id", cascadeDelete: true);
        }
    }
}
