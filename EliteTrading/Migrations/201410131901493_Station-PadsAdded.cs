namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StationPadsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "PadSmall", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stations", "PadMedium", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stations", "PadLarge", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Stations", "Name", c => c.String(nullable: false, maxLength: 60));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stations", "Name", c => c.String(nullable: false, maxLength: 30));
            DropColumn("dbo.Stations", "PadLarge");
            DropColumn("dbo.Stations", "PadMedium");
            DropColumn("dbo.Stations", "PadSmall");
        }
    }
}
