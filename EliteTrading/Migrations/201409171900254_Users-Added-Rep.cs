namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersAddedRep : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Reputation", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Queries", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Queries");
            DropColumn("dbo.AspNetUsers", "Reputation");
        }
    }
}
