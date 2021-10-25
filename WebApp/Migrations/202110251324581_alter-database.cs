namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterdatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        CourseCategoryId = c.Int(nullable: false),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseCategories", t => t.CourseCategoryId, cascadeDelete: true)
                .Index(t => t.CourseCategoryId);
            
            CreateTable(
                "dbo.TraineeCourses",
                c => new
                    {
                        Trainee_Id = c.String(nullable: false, maxLength: 128),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Trainee_Id, t.Course_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.Trainee_Id, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)
                .Index(t => t.Trainee_Id)
                .Index(t => t.Course_Id);
            
            CreateTable(
                "dbo.TrainerCourses",
                c => new
                    {
                        Trainer_Id = c.String(nullable: false, maxLength: 128),
                        Course_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Trainer_Id, t.Course_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.Trainer_Id, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_Id, cascadeDelete: true)
                .Index(t => t.Trainer_Id)
                .Index(t => t.Course_Id);
            
            AddColumn("dbo.AspNetUsers", "FullName", c => c.String());
            AddColumn("dbo.AspNetUsers", "Age", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Address", c => c.String());
            AddColumn("dbo.AspNetUsers", "BirthDate", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "Education", c => c.String(maxLength: 255));
            AddColumn("dbo.AspNetUsers", "Specialty", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainerCourses", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.TrainerCourses", "Trainer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TraineeCourses", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.TraineeCourses", "Trainee_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courses", "CourseCategoryId", "dbo.CourseCategories");
            DropIndex("dbo.TrainerCourses", new[] { "Course_Id" });
            DropIndex("dbo.TrainerCourses", new[] { "Trainer_Id" });
            DropIndex("dbo.TraineeCourses", new[] { "Course_Id" });
            DropIndex("dbo.TraineeCourses", new[] { "Trainee_Id" });
            DropIndex("dbo.Courses", new[] { "CourseCategoryId" });
            DropColumn("dbo.AspNetUsers", "Discriminator");
            DropColumn("dbo.AspNetUsers", "Specialty");
            DropColumn("dbo.AspNetUsers", "Education");
            DropColumn("dbo.AspNetUsers", "BirthDate");
            DropColumn("dbo.AspNetUsers", "Address");
            DropColumn("dbo.AspNetUsers", "Age");
            DropColumn("dbo.AspNetUsers", "FullName");
            DropTable("dbo.TrainerCourses");
            DropTable("dbo.TraineeCourses");
            DropTable("dbo.Courses");
            DropTable("dbo.CourseCategories");
        }
    }
}
