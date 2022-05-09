namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStatusTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastStarCheck = c.DateTime(nullable: false),
                        ShowHolding = c.Boolean(nullable: false),
                        HoldingMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Status");
        }
    }
}
