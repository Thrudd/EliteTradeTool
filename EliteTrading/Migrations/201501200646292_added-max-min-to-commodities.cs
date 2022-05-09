namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedmaxmintocommodities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Commodities", "Max", c => c.Int(nullable: false));
            AddColumn("dbo.Commodities", "Min", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Commodities", "Min");
            DropColumn("dbo.Commodities", "Max");
        }
    }
}
