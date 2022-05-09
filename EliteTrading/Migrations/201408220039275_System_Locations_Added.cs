namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class System_Locations_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Systems", "LocX", c => c.Double(nullable: false));
            AddColumn("dbo.Systems", "LocY", c => c.Double(nullable: false));
            AddColumn("dbo.Systems", "LocZ", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Systems", "LocZ");
            DropColumn("dbo.Systems", "LocY");
            DropColumn("dbo.Systems", "LocX");
        }
    }
}
