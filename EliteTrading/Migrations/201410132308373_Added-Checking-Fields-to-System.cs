namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCheckingFieldstoSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Systems", "DevData", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Systems", "Checked", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Systems", "CheckedDate", c => c.DateTime());
            AddColumn("dbo.Systems", "CheckedBy", c => c.String());
            DropTable("dbo.SystemChecklistItems");
        }
        
        public override void Down()
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
            
            DropColumn("dbo.Systems", "CheckedBy");
            DropColumn("dbo.Systems", "CheckedDate");
            DropColumn("dbo.Systems", "Checked");
            DropColumn("dbo.Systems", "DevData");
        }
    }
}
