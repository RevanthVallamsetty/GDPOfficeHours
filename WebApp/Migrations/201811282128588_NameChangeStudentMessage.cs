namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NameChangeStudentMessage : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Messages", newName: "StudentMessages");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.StudentMessages", newName: "Messages");
        }
    }
}
