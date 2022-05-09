namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedstationtypeslocationinfoandversioning2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        PadSmall = c.Boolean(nullable: false),
                        PadMedium = c.Boolean(nullable: false),
                        PadLarge = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Systems", "X", c => c.Double(nullable: false));
            AddColumn("dbo.Systems", "Y", c => c.Double(nullable: false));
            AddColumn("dbo.Systems", "Z", c => c.Double(nullable: false));
            AddColumn("dbo.Systems", "Version", c => c.Double(nullable: false, defaultValue: 0));
            AddColumn("dbo.Stations", "HasRepairs", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Stations", "DistanceFromJumpIn", c => c.Double(nullable: false, defaultValue: 0));
            AddColumn("dbo.Stations", "Version", c => c.Double(nullable: false, defaultValue: 0));
            AddColumn("dbo.Stations", "StationType_Id", c => c.Int());
            AddColumn("dbo.StationCommodities", "Supply", c => c.Int(nullable: false, defaultValue:0));
            AddColumn("dbo.StationCommodities", "Demand", c => c.Int(nullable: false, defaultValue: 0));
            CreateIndex("dbo.Stations", "StationType_Id");
            AddForeignKey("dbo.Stations", "StationType_Id", "dbo.StationTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stations", "StationType_Id", "dbo.StationTypes");
            DropIndex("dbo.Stations", new[] { "StationType_Id" });
            DropColumn("dbo.StationCommodities", "Demand");
            DropColumn("dbo.StationCommodities", "Supply");
            DropColumn("dbo.Stations", "StationType_Id");
            DropColumn("dbo.Stations", "Version");
            DropColumn("dbo.Stations", "DistanceFromJumpIn");
            DropColumn("dbo.Stations", "HasRepairs");
            DropColumn("dbo.Systems", "Version");
            DropColumn("dbo.Systems", "Z");
            DropColumn("dbo.Systems", "Y");
            DropColumn("dbo.Systems", "X");
            DropTable("dbo.StationTypes");
        }
    }
}
