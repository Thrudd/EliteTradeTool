namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TweakedLoadout3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Loadouts", "TotalCost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loadouts", "TotalCost", c => c.Int(nullable: false));
        }
    }
}
