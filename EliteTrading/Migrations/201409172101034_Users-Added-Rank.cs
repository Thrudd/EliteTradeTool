namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersAddedRank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ReputationNeeded", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Rank", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Reputation", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Queries", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Queries", c => c.Int());
            AlterColumn("dbo.AspNetUsers", "Reputation", c => c.Int());
            DropColumn("dbo.AspNetUsers", "Rank");
            DropColumn("dbo.AspNetUsers", "ReputationNeeded");
        }
    }
}
