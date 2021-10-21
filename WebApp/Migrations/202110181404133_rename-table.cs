namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renametable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TraineeProfiles", newName: "Trainees");
            RenameTable(name: "dbo.TrainerProfiles", newName: "Trainers");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Trainers", newName: "TrainerProfiles");
            RenameTable(name: "dbo.Trainees", newName: "TraineeProfiles");
        }
    }
}
