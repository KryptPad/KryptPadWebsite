namespace KryptPadWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pending_changes1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FieldType = c.Int(nullable: false),
                        Name = c.String(),
                        Value = c.String(),
                        Item_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.Item_Id, cascadeDelete: true)
                .Index(t => t.Item_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fields", "Item_Id", "dbo.Items");
            DropIndex("dbo.Fields", new[] { "Item_Id" });
            DropTable("dbo.Fields");
        }
    }
}
