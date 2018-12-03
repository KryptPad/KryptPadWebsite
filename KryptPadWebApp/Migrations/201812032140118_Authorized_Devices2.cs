namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Authorized_Devices2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AuthorizedDevices", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AuthorizedDevices", new[] { "User_Id" });
            AlterColumn("dbo.AuthorizedDevices", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AuthorizedDevices", "User_Id");
            AddForeignKey("dbo.AuthorizedDevices", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthorizedDevices", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AuthorizedDevices", new[] { "User_Id" });
            AlterColumn("dbo.AuthorizedDevices", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.AuthorizedDevices", "User_Id");
            AddForeignKey("dbo.AuthorizedDevices", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
