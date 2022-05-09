namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Station_PrimaryType_Removed : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StationTypes", "PrimaryType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StationTypes", "PrimaryType", c => c.Int(nullable: false));
        }
    }
}
