namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renamednotificationtonews : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Notifications", newName: "News");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.News", newName: "Notifications");
        }
    }
}
