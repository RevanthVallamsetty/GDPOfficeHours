namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addstatustable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FacultyStatus",
                c => new
                    {
                        Status = c.String(nullable: false, maxLength: 128, unicode: false),
                    })
                .PrimaryKey(t => t.Status);
            
            AddColumn("dbo.Faculties", "Status", c => c.String(maxLength: 128, unicode: false));
            CreateIndex("dbo.Faculties", "Status");
            AddForeignKey("dbo.Faculties", "Status", "dbo.FacultyStatus", "Status");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Faculties", "Status", "dbo.FacultyStatus");
            DropIndex("dbo.Faculties", new[] { "Status" });
            DropColumn("dbo.Faculties", "Status");
            DropTable("dbo.FacultyStatus");
        }
    }
}
