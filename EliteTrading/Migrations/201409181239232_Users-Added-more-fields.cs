namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersAddedmorefields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Title", c => c.String());
            AddColumn("dbo.AspNetUsers", "Badge", c => c.String());
            AddColumn("dbo.AspNetUsers", "Updates", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetRoles", "Badge", c => c.String());
            DropColumn("dbo.AspNetRoles", "Logo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetRoles", "Logo", c => c.String());
            DropColumn("dbo.AspNetRoles", "Badge");
            DropColumn("dbo.AspNetUsers", "Updates");
            DropColumn("dbo.AspNetUsers", "Badge");
            DropColumn("dbo.AspNetUsers", "Title");
        }
    }
}
