namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIconToStationTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StationTypes", "Icon", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StationTypes", "Icon");
        }
    }
}
