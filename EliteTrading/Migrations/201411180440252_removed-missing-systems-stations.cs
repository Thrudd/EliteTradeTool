namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedmissingsystemsstations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MissingIncorrectStations", "SystemId", "dbo.Systems");
            DropForeignKey("dbo.MissingSystems", "AllegianceId", "dbo.Allegiances");
            DropForeignKey("dbo.MissingSystems", "EconomyId", "dbo.Economies");
            DropForeignKey("dbo.MissingSystems", "GovernmentId", "dbo.Governments");
            DropIndex("dbo.MissingIncorrectStations", new[] { "SystemId" });
            DropIndex("dbo.MissingSystems", new[] { "EconomyId" });
            DropIndex("dbo.MissingSystems", new[] { "GovernmentId" });
            DropIndex("dbo.MissingSystems", new[] { "AllegianceId" });
            DropTable("dbo.MissingIncorrectStations");
            DropTable("dbo.MissingSystems");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.MissingSystems", "AllegianceId");
            CreateIndex("dbo.MissingSystems", "GovernmentId");
            CreateIndex("dbo.MissingSystems", "EconomyId");
            CreateIndex("dbo.MissingIncorrectStations", "SystemId");
            AddForeignKey("dbo.MissingSystems", "GovernmentId", "dbo.Governments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MissingSystems", "EconomyId", "dbo.Economies", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MissingSystems", "AllegianceId", "dbo.Allegiances", "Id", cascadeDelete: true);
            AddForeignKey("dbo.MissingIncorrectStations", "SystemId", "dbo.Systems", "Id", cascadeDelete: true);
        }
    }
}
