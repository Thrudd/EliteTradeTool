namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoadoutAdded : DbMigration
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
                        BulkheadId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bulkheads", t => t.BulkheadId, cascadeDelete: true)
                .ForeignKey("dbo.Ships", t => t.ShipId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ShipId)
                .Index(t => t.BulkheadId);
            
            CreateTable(
                "dbo.SupportFittings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadoutId = c.Int(nullable: false),
                        SupportId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Loadouts", t => t.LoadoutId, cascadeDelete: true)
                .ForeignKey("dbo.Supports", t => t.SupportId, cascadeDelete: true)
                .Index(t => t.LoadoutId)
                .Index(t => t.SupportId);
            
            CreateTable(
                "dbo.UtilityFittings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadoutId = c.Int(nullable: false),
                        UtilityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Loadouts", t => t.LoadoutId, cascadeDelete: true)
                .ForeignKey("dbo.Utilities", t => t.UtilityId, cascadeDelete: true)
                .Index(t => t.LoadoutId)
                .Index(t => t.UtilityId);
            
            CreateTable(
                "dbo.WeaponFittings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoadoutId = c.Int(nullable: false),
                        Class = c.Int(nullable: false),
                        WeaponId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Loadouts", t => t.LoadoutId, cascadeDelete: true)
                .ForeignKey("dbo.Weapons", t => t.WeaponId, cascadeDelete: true)
                .Index(t => t.LoadoutId)
                .Index(t => t.WeaponId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WeaponFittings", "WeaponId", "dbo.Weapons");
            DropForeignKey("dbo.WeaponFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.UtilityFittings", "UtilityId", "dbo.Utilities");
            DropForeignKey("dbo.UtilityFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.Loadouts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SupportFittings", "SupportId", "dbo.Supports");
            DropForeignKey("dbo.SupportFittings", "LoadoutId", "dbo.Loadouts");
            DropForeignKey("dbo.Loadouts", "ShipId", "dbo.Ships");
            DropForeignKey("dbo.Loadouts", "BulkheadId", "dbo.Bulkheads");
            DropIndex("dbo.WeaponFittings", new[] { "WeaponId" });
            DropIndex("dbo.WeaponFittings", new[] { "LoadoutId" });
            DropIndex("dbo.UtilityFittings", new[] { "UtilityId" });
            DropIndex("dbo.UtilityFittings", new[] { "LoadoutId" });
            DropIndex("dbo.SupportFittings", new[] { "SupportId" });
            DropIndex("dbo.SupportFittings", new[] { "LoadoutId" });
            DropIndex("dbo.Loadouts", new[] { "BulkheadId" });
            DropIndex("dbo.Loadouts", new[] { "ShipId" });
            DropIndex("dbo.Loadouts", new[] { "UserId" });
            DropTable("dbo.WeaponFittings");
            DropTable("dbo.UtilityFittings");
            DropTable("dbo.SupportFittings");
            DropTable("dbo.Loadouts");
        }
    }
}
