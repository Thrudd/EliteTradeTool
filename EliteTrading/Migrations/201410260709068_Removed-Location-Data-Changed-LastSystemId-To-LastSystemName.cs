namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedLocationDataChangedLastSystemIdToLastSystemName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastStationName", c => c.String());
            DropColumn("dbo.AspNetUsers", "LastStationId");
            DropColumn("dbo.Systems", "LocX");
            DropColumn("dbo.Systems", "LocY");
            DropColumn("dbo.Systems", "LocZ");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Systems", "LocZ", c => c.Double(nullable: false));
            AddColumn("dbo.Systems", "LocY", c => c.Double(nullable: false));
            AddColumn("dbo.Systems", "LocX", c => c.Double(nullable: false));
            AddColumn("dbo.AspNetUsers", "LastStationId", c => c.Int());
            DropColumn("dbo.AspNetUsers", "LastStationName");
        }
    }
}
