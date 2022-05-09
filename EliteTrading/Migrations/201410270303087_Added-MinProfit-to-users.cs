namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMinProfittousers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MinProfit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MinProfit");
        }
    }
}
