namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedShipTable2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ships", "C2", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "C4", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "C6", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "C8", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ships", "C8", c => c.Int());
            AlterColumn("dbo.Ships", "C6", c => c.Int());
            AlterColumn("dbo.Ships", "C4", c => c.Int());
            AlterColumn("dbo.Ships", "C2", c => c.Int());
        }
    }
}
