namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_field_sort_order : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fields", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fields", "SortOrder");
        }
    }
}
