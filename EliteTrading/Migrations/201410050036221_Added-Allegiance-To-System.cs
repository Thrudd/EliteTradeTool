namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAllegianceToSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Systems", "AllegianceId", c => c.Int(nullable: false, defaultValue:3));
            CreateIndex("dbo.Systems", "AllegianceId");
            AddForeignKey("dbo.Systems", "AllegianceId", "dbo.Allegiances", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Systems", "AllegianceId", "dbo.Allegiances");
            DropIndex("dbo.Systems", new[] { "AllegianceId" });
            DropColumn("dbo.Systems", "AllegianceId");
        }
    }
}
