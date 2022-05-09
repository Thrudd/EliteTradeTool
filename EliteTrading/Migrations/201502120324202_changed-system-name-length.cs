namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedsystemnamelength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Systems", "Name", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Systems", "Name", c => c.String(nullable: false, maxLength: 30));
        }
    }
}
