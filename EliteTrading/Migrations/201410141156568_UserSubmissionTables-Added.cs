namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserSubmissionTablesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MissingIncorrectStations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        SystemId = c.Int(nullable: false),
                        HasBlackmarket = c.Boolean(nullable: false),
                        HasMarket = c.Boolean(nullable: false),
                        HasOutfitting = c.Boolean(nullable: false),
                        HasShipyard = c.Boolean(nullable: false),
                        PadSmall = c.Boolean(nullable: false),
                        PadMedium = c.Boolean(nullable: false),
                        PadLarge = c.Boolean(nullable: false),
                        SubmittedDate = c.DateTime(),
                        SubmittedBy = c.String(),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Systems", t => t.SystemId, cascadeDelete: true)
                .Index(t => t.SystemId);
            
            CreateTable(
                "dbo.MissingSystems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        EconomyId = c.Int(nullable: false),
                        GovernmentId = c.Int(nullable: false),
                        AllegianceId = c.Int(nullable: false),
                        LocX = c.Double(nullable: false),
                        LocY = c.Double(nullable: false),
                        LocZ = c.Double(nullable: false),
                        SubmittedDate = c.DateTime(),
                        SubmittedBy = c.String(),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Allegiances", t => t.AllegianceId, cascadeDelete: true)
                .ForeignKey("dbo.Economies", t => t.EconomyId, cascadeDelete: true)
                .ForeignKey("dbo.Governments", t => t.GovernmentId, cascadeDelete: true)
                .Index(t => t.EconomyId)
                .Index(t => t.GovernmentId)
                .Index(t => t.AllegianceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MissingSystems", "GovernmentId", "dbo.Governments");
            DropForeignKey("dbo.MissingSystems", "EconomyId", "dbo.Economies");
            DropForeignKey("dbo.MissingSystems", "AllegianceId", "dbo.Allegiances");
            DropForeignKey("dbo.MissingIncorrectStations", "SystemId", "dbo.Systems");
            DropIndex("dbo.MissingSystems", new[] { "AllegianceId" });
            DropIndex("dbo.MissingSystems", new[] { "GovernmentId" });
            DropIndex("dbo.MissingSystems", new[] { "EconomyId" });
            DropIndex("dbo.MissingIncorrectStations", new[] { "SystemId" });
            DropTable("dbo.MissingSystems");
            DropTable("dbo.MissingIncorrectStations");
        }
    }
}
