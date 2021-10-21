namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtrainertraineeprofiletables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TraineeProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        BirthDate = c.String(),
                        Education = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TrainerProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Specialty = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainerProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TraineeProfiles", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.TrainerProfiles", new[] { "UserId" });
            DropIndex("dbo.TraineeProfiles", new[] { "UserId" });
            DropTable("dbo.TrainerProfiles");
            DropTable("dbo.TraineeProfiles");
        }
    }
}
