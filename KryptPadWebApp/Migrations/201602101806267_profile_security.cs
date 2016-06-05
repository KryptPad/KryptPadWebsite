namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class profile_security : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "Key1", c => c.String());
            AddColumn("dbo.Profiles", "Key2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "Key2");
            DropColumn("dbo.Profiles", "Key1");
        }
    }
}
