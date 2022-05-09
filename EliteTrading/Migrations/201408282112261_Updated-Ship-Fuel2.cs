namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedShipFuel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ships", "FSDMaxFuel", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ships", "FSDMaxFuel");
        }
    }
}
