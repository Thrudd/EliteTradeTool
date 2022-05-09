namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLoadoutTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bulkheads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ShipId = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Cost = c.Int(nullable: false),
                        C1 = c.Int(nullable: false),
                        C2 = c.Int(nullable: false),
                        C3 = c.Int(nullable: false),
                        C4 = c.Int(nullable: false),
                        C5 = c.Int(nullable: false),
                        C6 = c.Int(nullable: false),
                        C7 = c.Int(nullable: false),
                        C8 = c.Int(nullable: false),
                        Utility = c.Int(nullable: false),
                        Cargo = c.Int(nullable: false),
                        UnladenJumpRange = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Utilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Weight = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Weapons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Class = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Weapons");
            DropTable("dbo.Utilities");
            DropTable("dbo.Ships");
            DropTable("dbo.Bulkheads");
        }
    }
}
