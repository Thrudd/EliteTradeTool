namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStationTypeIdtomodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stations", "StationType_Id", "dbo.StationTypes");
            DropIndex("dbo.Stations", new[] { "StationType_Id" });
            RenameColumn(table: "dbo.Stations", name: "StationType_Id", newName: "StationTypeId");
            DropColumn("dbo.Stations", "StationTypeId");
            AddColumn("dbo.Stations", "StationTypeId", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.Stations", "StationTypeId");
            AddForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes");
            DropIndex("dbo.Stations", new[] { "StationTypeId" });
            AlterColumn("dbo.Stations", "StationTypeId", c => c.Int());
            RenameColumn(table: "dbo.Stations", name: "StationTypeId", newName: "StationType_Id");
            CreateIndex("dbo.Stations", "StationType_Id");
            AddForeignKey("dbo.Stations", "StationType_Id", "dbo.StationTypes", "Id");
        }
    }
}
