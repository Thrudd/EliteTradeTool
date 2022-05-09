namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedShipSupport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ships", "Support", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ships", "Support");
        }
    }
}
