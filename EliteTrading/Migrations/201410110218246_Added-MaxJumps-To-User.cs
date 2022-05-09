namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMaxJumpsToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MaxJumps", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MaxJumps");
        }
    }
}
