namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Factionsandserviesaddedtostations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "FactionName", c => c.String(maxLength: 100, defaultValue:""));
            AddColumn("dbo.Stations", "HasRefuel", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stations", "HasRearm", c => c.Boolean(nullable: false));
            AlterColumn("dbo.News", "Message", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.StationCommodities", "UpdatedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.Systems", "LastUpdateBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.Status", "HoldingMessage", c => c.String(maxLength: 2000));
            AlterColumn("dbo.AspNetUsers", "Title", c => c.String(maxLength: 50));
            AlterColumn("dbo.AspNetUsers", "Badge", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Badge", c => c.String());
            AlterColumn("dbo.AspNetUsers", "Title", c => c.String());
            AlterColumn("dbo.Status", "HoldingMessage", c => c.String());
            AlterColumn("dbo.Systems", "LastUpdateBy", c => c.String());
            AlterColumn("dbo.StationCommodities", "UpdatedBy", c => c.String());
            AlterColumn("dbo.News", "Message", c => c.String(nullable: false));
            DropColumn("dbo.Stations", "HasRearm");
            DropColumn("dbo.Stations", "HasRefuel");
            DropColumn("dbo.Stations", "FactionName");
        }
    }
}
