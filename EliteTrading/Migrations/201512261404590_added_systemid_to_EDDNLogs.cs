namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_systemid_to_EDDNLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EDDNLogs", "SystemId", c => c.Int(defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EDDNLogs", "SystemId");
        }
    }
}
