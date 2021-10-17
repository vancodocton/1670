namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcoursetraineetable : DbMigration
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
                .ForeignKey("dbo.TraineeProfiles", t => t.TraineeUserId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.TraineeUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseTrainees", "TraineeUserId", "dbo.TraineeProfiles");
            DropForeignKey("dbo.CourseTrainees", "CourseId", "dbo.Courses");
            DropIndex("dbo.CourseTrainees", new[] { "TraineeUserId" });
            DropIndex("dbo.CourseTrainees", new[] { "CourseId" });
            DropTable("dbo.CourseTrainees");
        }
    }
}
