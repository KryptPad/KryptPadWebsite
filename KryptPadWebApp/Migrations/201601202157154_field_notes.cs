namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class field_notes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "Notes");
        }
    }
}
