namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class field_types : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FieldTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Field_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fields", t => t.Field_Id)
                .Index(t => t.Field_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FieldTypes", "Field_Id", "dbo.Fields");
            DropIndex("dbo.FieldTypes", new[] { "Field_Id" });
            DropTable("dbo.FieldTypes");
        }
    }
}
