namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedUtility : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Utilities", "Weight", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Utilities", "Weight", c => c.Int(nullable: false));
        }
    }
}
