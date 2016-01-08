namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class category_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Profile_Id", c => c.Int());
            CreateIndex("dbo.Categories", "Profile_Id");
            AddForeignKey("dbo.Categories", "Profile_Id", "dbo.Profiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Categories", "Profile_Id", "dbo.Profiles");
            DropIndex("dbo.Categories", new[] { "Profile_Id" });
            DropColumn("dbo.Categories", "Profile_Id");
        }
    }
}
