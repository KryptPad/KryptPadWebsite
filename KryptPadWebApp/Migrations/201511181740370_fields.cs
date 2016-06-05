namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Name", c => c.String());
            DropColumn("dbo.Items", "ItemName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "ItemName", c => c.String());
            DropColumn("dbo.Items", "Name");
        }
    }
}
