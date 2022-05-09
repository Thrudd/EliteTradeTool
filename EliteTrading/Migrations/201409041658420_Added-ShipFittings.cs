namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedShipFittings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Loadouts", "BulkheadId", "dbo.Bulkheads");
            DropForeignKey("dbo.SupportFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.SupportFittings", "SupportId", "dbo.Supports");
            DropForeignKey("dbo.UtilityFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.UtilityFittings", "UtilityId", "dbo.Utilities");
            DropForeignKey("dbo.WeaponFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.WeaponFittings", "WeaponId", "dbo.Weapons");
            DropIndex("dbo.Loadouts", new[] { "BulkheadId" });
            DropIndex("dbo.SupportFittings", new[] { "LoadoutId" });
            DropIndex("dbo.SupportFittings", new[] { "SupportId" });
            DropIndex("dbo.UtilityFittings", new[] { "LoadoutId" });
            DropIndex("dbo.UtilityFittings", new[] { "UtilityId" });
            DropIndex("dbo.WeaponFittings", new[] { "LoadoutId" });
            DropIndex("dbo.WeaponFittings", new[] { "WeaponId" });
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
            
            AddColumn("dbo.Loadouts", "LoadoutWeight", c => c.Int(nullable: false));
            AddColumn("dbo.Loadouts", "Mass", c => c.Int(nullable: false));
            AddColumn("dbo.Loadouts", "Cost", c => c.Int(nullable: false));
            AddColumn("dbo.Loadouts", "TotalCost", c => c.Int(nullable: false));
            AddColumn("dbo.Loadouts", "InsuranceCost", c => c.Int(nullable: false));
            AddColumn("dbo.Loadouts", "EmptyDistance", c => c.Double(nullable: false));
            AddColumn("dbo.Loadouts", "FullDistance", c => c.Double(nullable: false));
            DropColumn("dbo.Loadouts", "BulkheadId");
            DropTable("dbo.SupportFittings");
            DropTable("dbo.UtilityFittings");
            DropTable("dbo.WeaponFittings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WeaponFittings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadoutId = c.Int(nullable: false),
                        Class = c.Int(nullable: false),
                        WeaponId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UtilityFittings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadoutId = c.Int(nullable: false),
                        UtilityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SupportFittings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadoutId = c.Int(nullable: false),
                        SupportId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Loadouts", "BulkheadId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ShipFittings", "LoadoutId", "dbo.Loadouts");
            DropIndex("dbo.ShipFittings", new[] { "LoadoutId" });
            DropColumn("dbo.Loadouts", "FullDistance");
            DropColumn("dbo.Loadouts", "EmptyDistance");
            DropColumn("dbo.Loadouts", "InsuranceCost");
            DropColumn("dbo.Loadouts", "TotalCost");
            DropColumn("dbo.Loadouts", "Cost");
            DropColumn("dbo.Loadouts", "Mass");
            DropColumn("dbo.Loadouts", "LoadoutWeight");
            DropTable("dbo.ShipFittings");
            CreateIndex("dbo.WeaponFittings", "WeaponId");
            CreateIndex("dbo.WeaponFittings", "LoadoutId");
            CreateIndex("dbo.UtilityFittings", "UtilityId");
            CreateIndex("dbo.UtilityFittings", "LoadoutId");
            CreateIndex("dbo.SupportFittings", "SupportId");
            CreateIndex("dbo.SupportFittings", "LoadoutId");
            CreateIndex("dbo.Loadouts", "BulkheadId");
            AddForeignKey("dbo.WeaponFittings", "WeaponId", "dbo.Weapons", "Id", cascadeDelete: true);
            AddForeignKey("dbo.WeaponFittings", "LoadoutId", "dbo.Loadouts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UtilityFittings", "UtilityId", "dbo.Utilities", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UtilityFittings", "LoadoutId", "dbo.Loadouts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SupportFittings", "SupportId", "dbo.Supports", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SupportFittings", "LoadoutId", "dbo.Loadouts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Loadouts", "BulkheadId", "dbo.Bulkheads", "Id", cascadeDelete: true);
        }
    }
}
