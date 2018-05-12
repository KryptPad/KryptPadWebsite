namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Item1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Categories", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.Items", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Categories", new[] { "Profile_Id" });
            DropIndex("dbo.Items", new[] { "Category_Id" });
            AlterColumn("dbo.Categories", "Profile_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Items", "Category_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Categories", "Profile_Id");
            CreateIndex("dbo.Items", "Category_Id");
            AddForeignKey("dbo.Categories", "Profile_Id", "dbo.Profiles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Items", "Category_Id", "dbo.Categories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Categories", "Profile_Id", "dbo.Profiles");
            DropIndex("dbo.Items", new[] { "Category_Id" });
            DropIndex("dbo.Categories", new[] { "Profile_Id" });
            AlterColumn("dbo.Items", "Category_Id", c => c.Int());
            AlterColumn("dbo.Categories", "Profile_Id", c => c.Int());
            CreateIndex("dbo.Items", "Category_Id");
            CreateIndex("dbo.Categories", "Profile_Id");
            AddForeignKey("dbo.Items", "Category_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.Categories", "Profile_Id", "dbo.Profiles", "Id");
        }
    }
}
