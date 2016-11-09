namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class item_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "BackgroundColor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "BackgroundColor");
        }
    }
}
