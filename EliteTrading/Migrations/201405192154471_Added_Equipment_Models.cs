namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Equipment_Models : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EquipmentTypeId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EquipmentTypes", t => t.EquipmentTypeId, cascadeDelete: true)
                .Index(t => t.EquipmentTypeId);
            
            CreateTable(
                "dbo.EquipmentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StationEquipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationId = c.Int(nullable: false),
                        EquipmentId = c.Int(nullable: false),
                        Buy = c.Int(nullable: false),
                        Sell_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Equipments", t => t.EquipmentId, cascadeDelete: true)
                .ForeignKey("dbo.Stations", t => t.Sell_Id)
                .ForeignKey("dbo.Stations", t => t.StationId, cascadeDelete: true)
                .Index(t => t.StationId)
                .Index(t => t.EquipmentId)
                .Index(t => t.Sell_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StationEquipments", "StationId", "dbo.Stations");
            DropForeignKey("dbo.StationEquipments", "Sell_Id", "dbo.Stations");
            DropForeignKey("dbo.StationEquipments", "EquipmentId", "dbo.Equipments");
            DropForeignKey("dbo.Equipments", "EquipmentTypeId", "dbo.EquipmentTypes");
            DropIndex("dbo.StationEquipments", new[] { "Sell_Id" });
            DropIndex("dbo.StationEquipments", new[] { "EquipmentId" });
            DropIndex("dbo.StationEquipments", new[] { "StationId" });
            DropIndex("dbo.Equipments", new[] { "EquipmentTypeId" });
            DropTable("dbo.StationEquipments");
            DropTable("dbo.EquipmentTypes");
            DropTable("dbo.Equipments");
        }
    }
}
