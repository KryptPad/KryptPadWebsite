namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class profile_remove_haspassphrase : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Profiles", "HasPassword");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Profiles", "HasPassword", c => c.Boolean(nullable: false));
        }
    }
}
