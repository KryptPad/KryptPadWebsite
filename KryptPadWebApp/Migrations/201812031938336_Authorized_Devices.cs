namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Authorized_Devices : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthorizedDevices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthorizedDevices", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AuthorizedDevices", new[] { "User_Id" });
            DropTable("dbo.AuthorizedDevices");
        }
    }
}
