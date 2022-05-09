namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shipremovednulls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "HasBlackmarket", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Ships", "EmptyWeight", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "OptimisedMass", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "FuelTank", c => c.Double(nullable: false));
            AlterColumn("dbo.Ships", "Reservoir", c => c.Double(nullable: false));
            AlterColumn("dbo.Ships", "FSDMaxFuel", c => c.Double(nullable: false));
            AlterColumn("dbo.Ships", "FuelPower", c => c.Double(nullable: false));
            AlterColumn("dbo.Ships", "FuelCoefficient", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ships", "FuelCoefficient", c => c.Double());
            AlterColumn("dbo.Ships", "FuelPower", c => c.Double());
            AlterColumn("dbo.Ships", "FSDMaxFuel", c => c.Double());
            AlterColumn("dbo.Ships", "Reservoir", c => c.Double());
            AlterColumn("dbo.Ships", "FuelTank", c => c.Double());
            AlterColumn("dbo.Ships", "OptimisedMass", c => c.Int());
            AlterColumn("dbo.Ships", "EmptyWeight", c => c.Int());
            DropColumn("dbo.Stations", "HasBlackmarket");
        }
    }
}
