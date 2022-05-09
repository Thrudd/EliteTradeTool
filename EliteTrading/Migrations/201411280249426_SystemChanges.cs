namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Systems", "EconomyId", "dbo.Economies");
            DropIndex("dbo.Systems", new[] { "EconomyId" });
            DropColumn("dbo.Systems", "EconomyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Systems", "EconomyId", c => c.Int(nullable: false));
            CreateIndex("dbo.Systems", "EconomyId");
            AddForeignKey("dbo.Systems", "EconomyId", "dbo.Economies", "Id", cascadeDelete: true);
        }
    }
}
