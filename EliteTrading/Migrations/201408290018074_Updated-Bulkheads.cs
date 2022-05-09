namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedBulkheads : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Bulkheads", "ShipId");
            AddForeignKey("dbo.Bulkheads", "ShipId", "dbo.Ships", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bulkheads", "ShipId", "dbo.Ships");
            DropIndex("dbo.Bulkheads", new[] { "ShipId" });
        }
    }
}
