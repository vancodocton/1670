namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createcoursetable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        CourseCategoryId = c.Int(nullable: false),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseCategories", t => t.CourseCategoryId, cascadeDelete: true)
                .Index(t => t.CourseCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "CourseCategoryId", "dbo.CourseCategories");
            DropIndex("dbo.Courses", new[] { "CourseCategoryId" });
            DropTable("dbo.Courses");
        }
    }
}
