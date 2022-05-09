namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedstationtypesremovedpadsfromstation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes");
            DropIndex("dbo.Stations", new[] { "StationTypeId" });
            AddColumn("dbo.StationTypes", "PrimaryType", c => c.Int(nullable: false));
            AlterColumn("dbo.Stations", "StationTypeId", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.Stations", "StationTypeId");
            AddForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes", "Id", cascadeDelete: true);
            DropColumn("dbo.Stations", "PadSmall");
            DropColumn("dbo.Stations", "PadMedium");
            DropColumn("dbo.Stations", "PadLarge");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stations", "PadLarge", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stations", "PadMedium", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stations", "PadSmall", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes");
            DropIndex("dbo.Stations", new[] { "StationTypeId" });
            AlterColumn("dbo.Stations", "StationTypeId", c => c.Int());
            DropColumn("dbo.StationTypes", "PrimaryType");
            CreateIndex("dbo.Stations", "StationTypeId");
            AddForeignKey("dbo.Stations", "StationTypeId", "dbo.StationTypes", "Id");
        }
    }
}
