namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterCourseCourseCategoryTablesColumns : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.CourseCategories", "Name", unique: true);
            CreateIndex("dbo.Courses", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Courses", new[] { "Name" });
            DropIndex("dbo.CourseCategories", new[] { "Name" });
        }
    }
}
