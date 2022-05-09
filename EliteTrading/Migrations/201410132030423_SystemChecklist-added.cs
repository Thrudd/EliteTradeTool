namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemChecklistadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SystemChecklistItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemId = c.Int(nullable: false),
                        CheckedOn = c.DateTime(nullable: false),
                        CommanderName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SystemChecklistItems");
        }
    }
}
