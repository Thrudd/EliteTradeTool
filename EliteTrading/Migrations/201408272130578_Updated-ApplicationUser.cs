namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LastStationId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Cargo", c => c.Int());
            AddColumn("dbo.AspNetUsers", "JumpRange", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "JumpRange");
            DropColumn("dbo.AspNetUsers", "Cargo");
            DropColumn("dbo.AspNetUsers", "LastStationId");
        }
    }
}
