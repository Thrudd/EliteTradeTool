namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedShipFuel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ships", "EmptyWeight", c => c.Int());
            AddColumn("dbo.Ships", "FuelTank", c => c.Double());
            AddColumn("dbo.Ships", "Reservoir", c => c.Double());
            AddColumn("dbo.Ships", "FuelPower", c => c.Double());
            AddColumn("dbo.Ships", "FuelCoefficient", c => c.Double());
            DropColumn("dbo.Ships", "UnladenJumpRange");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Ships", "UnladenJumpRange", c => c.Double(nullable: false));
            DropColumn("dbo.Ships", "FuelCoefficient");
            DropColumn("dbo.Ships", "FuelPower");
            DropColumn("dbo.Ships", "Reservoir");
            DropColumn("dbo.Ships", "FuelTank");
            DropColumn("dbo.Ships", "EmptyWeight");
        }
    }
}
