namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbupdates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Profiles", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Profiles", new[] { "User_Id" });
            AlterColumn("dbo.Categories", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Fields", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Profiles", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Profiles", "Key1", c => c.String(nullable: false));
            AlterColumn("dbo.Profiles", "Key2", c => c.String(nullable: false));
            AlterColumn("dbo.Profiles", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Profiles", "User_Id");
            AddForeignKey("dbo.Profiles", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Profiles", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Profiles", new[] { "User_Id" });
            AlterColumn("dbo.Profiles", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Profiles", "Key2", c => c.String());
            AlterColumn("dbo.Profiles", "Key1", c => c.String());
            AlterColumn("dbo.Profiles", "Name", c => c.String());
            AlterColumn("dbo.Fields", "Name", c => c.String());
            AlterColumn("dbo.Categories", "Name", c => c.String());
            CreateIndex("dbo.Profiles", "User_Id");
            AddForeignKey("dbo.Profiles", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
