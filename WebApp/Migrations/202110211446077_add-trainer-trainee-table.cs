namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtrainertraineetable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trainees",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        BirthDate = c.DateTime(),
                        Education = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Trainers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Specialty = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TraineeCourses",
                c => new
                    {
                        Trainee_UserId = c.String(nullable: false, maxLength: 128),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Trainee_UserId, t.Course_Id })
                .ForeignKey("dbo.Trainees", t => t.Trainee_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)
                .Index(t => t.Trainee_UserId)
                .Index(t => t.Course_Id);
            
            CreateTable(
                "dbo.TrainerCourses",
                c => new
                    {
                        Trainer_UserId = c.String(nullable: false, maxLength: 128),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Trainer_UserId, t.Course_Id })
                .ForeignKey("dbo.Trainers", t => t.Trainer_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)
                .Index(t => t.Trainer_UserId)
                .Index(t => t.Course_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trainers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TrainerCourses", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.TrainerCourses", "Trainer_UserId", "dbo.Trainers");
            DropForeignKey("dbo.Trainees", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TraineeCourses", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.TraineeCourses", "Trainee_UserId", "dbo.Trainees");
            DropIndex("dbo.TrainerCourses", new[] { "Course_Id" });
            DropIndex("dbo.TrainerCourses", new[] { "Trainer_UserId" });
            DropIndex("dbo.TraineeCourses", new[] { "Course_Id" });
            DropIndex("dbo.TraineeCourses", new[] { "Trainee_UserId" });
            DropIndex("dbo.Trainers", new[] { "UserId" });
            DropIndex("dbo.Trainees", new[] { "UserId" });
            DropTable("dbo.TrainerCourses");
            DropTable("dbo.TraineeCourses");
            DropTable("dbo.Trainers");
            DropTable("dbo.Trainees");
        }
    }
}
