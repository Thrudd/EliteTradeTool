namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedApplicationUser2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Cash", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Cash");
        }
    }
}
