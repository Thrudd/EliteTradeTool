namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stationlastupdatelengthadded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stations", "LastUpdateBy", c => c.String(maxLength: 60));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stations", "LastUpdateBy", c => c.String());
        }
    }
}
