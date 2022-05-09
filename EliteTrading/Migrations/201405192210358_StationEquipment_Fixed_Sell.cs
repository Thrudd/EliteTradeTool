namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StationEquipment_Fixed_Sell : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StationEquipments", "Sell_Id", "dbo.Stations");
            DropIndex("dbo.StationEquipments", new[] { "Sell_Id" });
            AddColumn("dbo.StationEquipments", "Sell", c => c.Int(nullable: false));
            DropColumn("dbo.StationEquipments", "Sell_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StationEquipments", "Sell_Id", c => c.Int());
            DropColumn("dbo.StationEquipments", "Sell");
            CreateIndex("dbo.StationEquipments", "Sell_Id");
            AddForeignKey("dbo.StationEquipments", "Sell_Id", "dbo.Stations", "Id");
        }
    }
}
