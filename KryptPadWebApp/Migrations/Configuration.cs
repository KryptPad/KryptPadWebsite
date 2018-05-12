namespace KryptPadWebApp.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KryptPadWebApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "KryptPadWebApp.Models.ApplicationDbContext";
        }

        protected override void Seed(KryptPadWebApp.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            // Seed the field types table
            context.FieldTypes.AddOrUpdate(
                ft => ft.Id,
                new FieldType() { Id = 1, Name = "Password" },
                new FieldType() { Id = 2, Name = "Username" },
                new FieldType() { Id = 3, Name = "Email" },
                new FieldType() { Id = 4, Name = "Account Number" },
                new FieldType() { Id = 5, Name = "Credit Card Number" },
                new FieldType() { Id = 6, Name = "Numeric" },
                new FieldType() { Id = 7, Name = "Text" }
            );
        }
    }
}
