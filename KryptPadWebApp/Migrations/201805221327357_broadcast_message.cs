namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class broadcast_message : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppSettings", "BroadcastMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppSettings", "BroadcastMessage");
        }
    }
}
