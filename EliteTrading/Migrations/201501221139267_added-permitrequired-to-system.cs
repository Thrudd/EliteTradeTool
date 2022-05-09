namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpermitrequiredtosystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Systems", "PermitRequired", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Systems", "PermitRequired");
        }
    }
}
