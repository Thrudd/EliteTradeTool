namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Azureinitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Allegiances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Commodities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 40),
                        GalacticAveragePrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Economies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Governments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShowFrom = c.DateTime(nullable: false),
                        ShowTo = c.DateTime(nullable: false),
                        Message = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Rank = c.Int(),
                        RepRequired = c.Int(),
                        Badge = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.StationCommodities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationId = c.Int(nullable: false),
                        CommodityId = c.Int(nullable: false),
                        Buy = c.Int(nullable: false),
                        Sell = c.Int(nullable: false),
                        DemandSupplyId = c.Int(),
                        LastUpdate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Commodities", t => t.CommodityId, cascadeDelete: true)
                .ForeignKey("dbo.DemandSupplies", t => t.DemandSupplyId)
                .ForeignKey("dbo.Stations", t => t.StationId, cascadeDelete: true)
                .Index(t => t.StationId)
                .Index(t => t.CommodityId)
                .Index(t => t.DemandSupplyId);
            
            CreateTable(
                "dbo.DemandSupplies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        SystemId = c.Int(nullable: false),
                        HasBlackmarket = c.Boolean(nullable: false),
                        HasMarket = c.Boolean(nullable: false),
                        HasOutfitting = c.Boolean(nullable: false),
                        HasShipyard = c.Boolean(nullable: false),
                        HasRepairs = c.Boolean(nullable: false),
                        DistanceFromJumpIn = c.Double(nullable: false),
                        Version = c.Double(nullable: false),
                        StationTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StationTypes", t => t.StationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Systems", t => t.SystemId, cascadeDelete: true)
                .Index(t => t.SystemId)
                .Index(t => t.StationTypeId);
            
            CreateTable(
                "dbo.StationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        PrimaryType = c.Int(nullable: false),
                        PadSmall = c.Boolean(nullable: false),
                        PadMedium = c.Boolean(nullable: false),
                        PadLarge = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Systems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        EconomyId = c.Int(nullable: false),
                        GovernmentId = c.Int(nullable: false),
                        AllegianceId = c.Int(nullable: false),
                        X = c.Double(nullable: false),
                        Y = c.Double(nullable: false),
                        Z = c.Double(nullable: false),
                        Version = c.Double(nullable: false),
                        DevData = c.Boolean(nullable: false),
                        Checked = c.Boolean(nullable: false),
                        CheckedDate = c.DateTime(),
                        CheckedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Allegiances", t => t.AllegianceId, cascadeDelete: true)
                .ForeignKey("dbo.Economies", t => t.EconomyId, cascadeDelete: true)
                .ForeignKey("dbo.Governments", t => t.GovernmentId, cascadeDelete: true)
                .Index(t => t.EconomyId)
                .Index(t => t.GovernmentId)
                .Index(t => t.AllegianceId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CommanderName = c.String(nullable: false, maxLength: 50),
                        LastStationName = c.String(),
                        Cargo = c.Int(),
                        Cash = c.Int(),
                        JumpRange = c.Double(),
                        MaxJumps = c.Int(nullable: false),
                        MinProfit = c.Int(nullable: false),
                        Reputation = c.Int(nullable: false),
                        ReputationNeeded = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        Title = c.String(),
                        Badge = c.String(),
                        Queries = c.Int(nullable: false),
                        Updates = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stations", "SystemId", "dbo.Systems");
            DropForeignKey("dbo.Systems", "GovernmentId", "dbo.Governments");
            DropForeignKey("dbo.Systems", "EconomyId", "dbo.Economies");
            DropForeignKey("dbo.Systems", "AllegianceId", "dbo.Allegiances");
            DropForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes");
            DropForeignKey("dbo.StationCommodities", "StationId", "dbo.Stations");
            DropForeignKey("dbo.StationCommodities", "DemandSupplyId", "dbo.DemandSupplies");
            DropForeignKey("dbo.StationCommodities", "CommodityId", "dbo.Commodities");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Commodities", "CategoryId", "dbo.Categories");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Systems", new[] { "AllegianceId" });
            DropIndex("dbo.Systems", new[] { "GovernmentId" });
            DropIndex("dbo.Systems", new[] { "EconomyId" });
            DropIndex("dbo.Stations", new[] { "StationTypeId" });
            DropIndex("dbo.Stations", new[] { "SystemId" });
            DropIndex("dbo.StationCommodities", new[] { "DemandSupplyId" });
            DropIndex("dbo.StationCommodities", new[] { "CommodityId" });
            DropIndex("dbo.StationCommodities", new[] { "StationId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Commodities", new[] { "CategoryId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Systems");
            DropTable("dbo.StationTypes");
            DropTable("dbo.Stations");
            DropTable("dbo.DemandSupplies");
            DropTable("dbo.StationCommodities");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Notifications");
            DropTable("dbo.Governments");
            DropTable("dbo.Economies");
            DropTable("dbo.Commodities");
            DropTable("dbo.Categories");
            DropTable("dbo.Allegiances");
        }
    }
}
