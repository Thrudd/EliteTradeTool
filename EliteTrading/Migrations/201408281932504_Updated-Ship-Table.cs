namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedShipTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ships", "C2", c => c.Int());
            AlterColumn("dbo.Ships", "C4", c => c.Int());
            AlterColumn("dbo.Ships", "C6", c => c.Int());
            AlterColumn("dbo.Ships", "C8", c => c.Int());
            DropColumn("dbo.Ships", "C1");
            DropColumn("dbo.Ships", "C3");
            DropColumn("dbo.Ships", "C5");
            DropColumn("dbo.Ships", "C7");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Ships", "C7", c => c.Int(nullable: false));
            AddColumn("dbo.Ships", "C5", c => c.Int(nullable: false));
            AddColumn("dbo.Ships", "C3", c => c.Int(nullable: false));
            AddColumn("dbo.Ships", "C1", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "C8", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "C6", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "C4", c => c.Int(nullable: false));
            AlterColumn("dbo.Ships", "C2", c => c.Int(nullable: false));
        }
    }
}
