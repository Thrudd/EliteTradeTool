namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedpadsizes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StationTypes", "PadSmall");
            DropColumn("dbo.StationTypes", "PadMedium");
            DropColumn("dbo.StationTypes", "PadLarge");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StationTypes", "PadLarge", c => c.Boolean(nullable: false));
            AddColumn("dbo.StationTypes", "PadMedium", c => c.Boolean(nullable: false));
            AddColumn("dbo.StationTypes", "PadSmall", c => c.Boolean(nullable: false));
        }
    }
}
