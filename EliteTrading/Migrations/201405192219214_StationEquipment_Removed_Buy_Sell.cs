namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StationEquipment_Removed_Buy_Sell : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StationEquipments", "Buy");
            DropColumn("dbo.StationEquipments", "Sell");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StationEquipments", "Sell", c => c.Int(nullable: false));
            AddColumn("dbo.StationEquipments", "Buy", c => c.Int(nullable: false));
        }
    }
}
