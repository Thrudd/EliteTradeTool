namespace EliteTrading.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedsupplydemandtocommoditiesremovedsomeoldstuff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stations", "LastUpdateDate", c => c.DateTime());
            AddColumn("dbo.Stations", "LastUpdateBy", c => c.String());
            AddColumn("dbo.StationCommodities", "Supply", c => c.String(maxLength: 4));
            AddColumn("dbo.StationCommodities", "SupplyAmount", c => c.Int(nullable: false));
            AddColumn("dbo.StationCommodities", "Demand", c => c.String(maxLength: 4));
            AddColumn("dbo.StationCommodities", "DemandAmount", c => c.Int(nullable: false));
            AddColumn("dbo.Systems", "LastUpdateDate", c => c.DateTime());
            AddColumn("dbo.Systems", "LastUpdateBy", c => c.String());
            DropColumn("dbo.Systems", "Checked");
            DropColumn("dbo.Systems", "CheckedDate");
            DropColumn("dbo.Systems", "CheckedBy");
            DropColumn("dbo.AspNetUsers", "LastStationName");
            DropColumn("dbo.AspNetUsers", "Cargo");
            DropColumn("dbo.AspNetUsers", "Cash");
            DropColumn("dbo.AspNetUsers", "JumpRange");
            DropColumn("dbo.AspNetUsers", "MaxJumps");
            DropColumn("dbo.AspNetUsers", "MinProfit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "MinProfit", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "MaxJumps", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "JumpRange", c => c.Double());
            AddColumn("dbo.AspNetUsers", "Cash", c => c.Int());
            AddColumn("dbo.AspNetUsers", "Cargo", c => c.Int());
            AddColumn("dbo.AspNetUsers", "LastStationName", c => c.String());
            AddColumn("dbo.Systems", "CheckedBy", c => c.String());
            AddColumn("dbo.Systems", "CheckedDate", c => c.DateTime());
            AddColumn("dbo.Systems", "Checked", c => c.Boolean(nullable: false));
            DropColumn("dbo.Systems", "LastUpdateBy");
            DropColumn("dbo.Systems", "LastUpdateDate");
            DropColumn("dbo.StationCommodities", "DemandAmount");
            DropColumn("dbo.StationCommodities", "Demand");
            DropColumn("dbo.StationCommodities", "SupplyAmount");
            DropColumn("dbo.StationCommodities", "Supply");
            DropColumn("dbo.Stations", "LastUpdateBy");
            DropColumn("dbo.Stations", "LastUpdateDate");
        }
    }
}
