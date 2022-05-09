namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Putpadsbackintemporarily : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes");
            DropIndex("dbo.Stations", new[] { "StationTypeId" });
            AddColumn("dbo.Stations", "PadSmall", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stations", "PadMedium", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stations", "PadLarge", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Stations", "StationTypeId", c => c.Int());
            CreateIndex("dbo.Stations", "StationTypeId");
            AddForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes");
            DropIndex("dbo.Stations", new[] { "StationTypeId" });
            AlterColumn("dbo.Stations", "StationTypeId", c => c.Int(nullable: false));
            DropColumn("dbo.Stations", "PadLarge");
            DropColumn("dbo.Stations", "PadMedium");
            DropColumn("dbo.Stations", "PadSmall");
            CreateIndex("dbo.Stations", "StationTypeId");
            AddForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes", "Id", cascadeDelete: true);
        }
    }
}
