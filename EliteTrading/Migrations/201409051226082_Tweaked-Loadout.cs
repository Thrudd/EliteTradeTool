namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TweakedLoadout : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loadouts", "LoadoutValue", c => c.Int(nullable: false));
            DropColumn("dbo.Loadouts", "Cost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Loadouts", "Cost", c => c.Int(nullable: false));
            DropColumn("dbo.Loadouts", "LoadoutValue");
        }
    }
}
