namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rolechanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "Rank", c => c.Int());
            AddColumn("dbo.AspNetRoles", "RepRequired", c => c.Int());
            AddColumn("dbo.AspNetRoles", "Logo", c => c.String());
            AddColumn("dbo.AspNetRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "Discriminator");
            DropColumn("dbo.AspNetRoles", "Logo");
            DropColumn("dbo.AspNetRoles", "RepRequired");
            DropColumn("dbo.AspNetRoles", "Rank");
        }
    }
}
