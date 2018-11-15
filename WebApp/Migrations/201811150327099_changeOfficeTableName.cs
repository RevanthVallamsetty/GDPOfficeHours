namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeOfficeTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.OfficeHours", newName: "OfficeSchedules");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.OfficeSchedules", newName: "OfficeHours");
        }
    }
}
