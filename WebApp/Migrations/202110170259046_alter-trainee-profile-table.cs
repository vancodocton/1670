namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class altertraineeprofiletable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TraineeProfiles", "BirthDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TraineeProfiles", "BirthDate", c => c.String());
        }
    }
}
