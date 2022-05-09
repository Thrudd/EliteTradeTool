namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedSystemRoutes : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.SystemRoutes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SystemRoutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartSystemId = c.Int(nullable: false),
                        EndSystemId = c.Int(nullable: false),
                        AvoidAnarchy = c.Boolean(nullable: false),
                        Reachable = c.Boolean(nullable: false),
                        JumpRange = c.Double(nullable: false),
                        Jumps = c.Int(nullable: false),
                        Route = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
