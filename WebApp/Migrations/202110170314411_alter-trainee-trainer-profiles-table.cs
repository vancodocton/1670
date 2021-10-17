namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class altertraineetrainerprofilestable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TraineeProfiles", "BirthDate", c => c.DateTime());
            AlterColumn("dbo.TraineeProfiles", "Education", c => c.String(maxLength: 255));
            AlterColumn("dbo.TrainerProfiles", "Specialty", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrainerProfiles", "Specialty", c => c.String());
            AlterColumn("dbo.TraineeProfiles", "Education", c => c.String());
            AlterColumn("dbo.TraineeProfiles", "BirthDate", c => c.String());
        }
    }
}
