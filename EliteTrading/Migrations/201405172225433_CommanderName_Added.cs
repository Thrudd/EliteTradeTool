namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommanderName_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CommanderName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CommanderName");
        }
    }
}
