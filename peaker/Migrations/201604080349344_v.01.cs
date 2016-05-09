namespace peaker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Athletes", "StravaId", c => c.Int(nullable: false));
            AddColumn("dbo.IndexedActivities", "Athlete_Id", c => c.Int());
            CreateIndex("dbo.IndexedActivities", "Athlete_Id");
            AddForeignKey("dbo.IndexedActivities", "Athlete_Id", "dbo.Athletes", "Id");
            DropColumn("dbo.IndexedActivities", "StravaAthleteId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IndexedActivities", "StravaAthleteId", c => c.Int(nullable: false));
            DropForeignKey("dbo.IndexedActivities", "Athlete_Id", "dbo.Athletes");
            DropIndex("dbo.IndexedActivities", new[] { "Athlete_Id" });
            DropColumn("dbo.IndexedActivities", "Athlete_Id");
            DropColumn("dbo.Athletes", "StravaId");
        }
    }
}
