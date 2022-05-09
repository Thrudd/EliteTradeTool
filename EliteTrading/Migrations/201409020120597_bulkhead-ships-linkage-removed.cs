namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bulkheadshipslinkageremoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bulkheads", "ShipId", "dbo.Ships");
            DropIndex("dbo.Bulkheads", new[] { "ShipId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Bulkheads", "ShipId");
            AddForeignKey("dbo.Bulkheads", "ShipId", "dbo.Ships", "Id", cascadeDelete: true);
        }
    }
}
