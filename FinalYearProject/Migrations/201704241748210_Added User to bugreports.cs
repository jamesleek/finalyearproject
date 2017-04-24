namespace FinalYearProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUsertobugreports : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BugReports", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BugReports", "UserName");
        }
    }
}
