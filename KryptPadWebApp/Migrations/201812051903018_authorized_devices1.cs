namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class authorized_devices1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AuthorizedDevices", "AccessedFromIPAddress", c => c.String());
            AddColumn("dbo.AuthorizedDevices", "DateAuthorized", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AuthorizedDevices", "DateAuthorized");
            DropColumn("dbo.AuthorizedDevices", "AccessedFromIPAddress");
        }
    }
}
