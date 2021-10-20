namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtrainertraineetable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseTrainees",
                c => new
                    {
                        CourseId = c.Int(nullable: false),
                        TraineeUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CourseId, t.TraineeUserId })
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Trainees", t => t.TraineeUserId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.TraineeUserId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseTrainees", "TraineeUserId", "dbo.Trainees");
            DropForeignKey("dbo.Trainers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Trainees", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseTrainees", "CourseId", "dbo.Courses");
            DropIndex("dbo.Trainers", new[] { "UserId" });
            DropIndex("dbo.Trainees", new[] { "UserId" });
            DropIndex("dbo.CourseTrainees", new[] { "TraineeUserId" });
            DropIndex("dbo.CourseTrainees", new[] { "CourseId" });
            DropTable("dbo.Trainers");
            DropTable("dbo.Trainees");
            DropTable("dbo.CourseTrainees");
        }
    }
}
