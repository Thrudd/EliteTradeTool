namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedWeapon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Weapons", "Mount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Weapons", "Mount");
        }
    }
}
