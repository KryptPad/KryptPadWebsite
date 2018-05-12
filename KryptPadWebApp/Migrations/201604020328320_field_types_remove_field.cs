namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class field_types_remove_field : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FieldTypes", "Field_Id", "dbo.Fields");
            DropIndex("dbo.FieldTypes", new[] { "Field_Id" });
            DropColumn("dbo.FieldTypes", "Field_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FieldTypes", "Field_Id", c => c.Int());
            CreateIndex("dbo.FieldTypes", "Field_Id");
            AddForeignKey("dbo.FieldTypes", "Field_Id", "dbo.Fields", "Id");
        }
    }
}
