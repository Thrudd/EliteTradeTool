namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EDDNLog_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EDDNLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.DateTime(nullable: false),
                        Action = c.String(nullable: false, maxLength: 20),
                        Message = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EDDNLogs");
        }
    }
}
