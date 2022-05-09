namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Increased_News : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.News", "Message", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.News", "Message", c => c.String(nullable: false, maxLength: 2000));
        }
    }
}
