namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddcaptureNotemodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CaptureNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentName = c.String(),
                        NoteLink = c.String(),
                        CapturedDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 128, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculties", t => t.Email)
                .Index(t => t.Email);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CaptureNotes", "Email", "dbo.Faculties");
            DropIndex("dbo.CaptureNotes", new[] { "Email" });
            DropTable("dbo.CaptureNotes");
        }
    }
}
