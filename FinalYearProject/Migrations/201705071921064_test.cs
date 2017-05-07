namespace FinalYearProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BugReports", "BugDescription", c => c.String(maxLength: 150));
            AlterColumn("dbo.Categories", "CategoryName", c => c.String(maxLength: 15));
            AlterColumn("dbo.Categories", "Keywords", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Categories", "Keywords", c => c.String());
            AlterColumn("dbo.Categories", "CategoryName", c => c.String());
            AlterColumn("dbo.BugReports", "BugDescription", c => c.String());
        }
    }
}
