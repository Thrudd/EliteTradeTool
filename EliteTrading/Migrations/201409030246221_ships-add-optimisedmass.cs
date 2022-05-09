namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shipsaddoptimisedmass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ships", "OptimisedMass", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ships", "OptimisedMass");
        }
    }
}
