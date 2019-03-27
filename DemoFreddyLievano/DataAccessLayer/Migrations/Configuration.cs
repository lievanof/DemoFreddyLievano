using Demo.DataAccessLayer.Context;
using System;
using System.Data.Entity.Migrations;
using Demo.DataAccessLayer.Utils;
using Demo.Model;

namespace Demo.DataAccessLayer.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DemoContext>
    {
        public Configuration()
        {
            //set true just for development
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DemoContext context)
        {
            context.Users.AddOrUpdate(
               u => u.Username,
               new User
               {
                   
                   FirstName = "Admin",
                   LastName = "Admin",
                   Rol = Roles.SuperUser,
                   Username = "admin@latinmail.com",
                   PasswordConfirmation = PasswordGenerator.Encrypt("Admin1"),
                   Password = PasswordGenerator.Encrypt("Admin1"),
                   Enabled = true,
                   Created = DateTime.Now,
                   UserData = new UserData { BirthDate = DateTime.Now.AddYears(-40), Gender = Gender.Unspecified, Email = "admin@latinmail.com" }
                   }
           );

            context.Users.AddOrUpdate(
               u => u.Username,
               new User
               {
                   FirstName = "User",
                   LastName = "User",
                   Rol = Roles.CommonUser,
                   Username = "user@latinmail.com",
                   PasswordConfirmation = PasswordGenerator.Encrypt("User1"),
                   Password = PasswordGenerator.Encrypt("User1"),
                   Enabled = true,
                   Created = DateTime.Now,
                   UserData = new UserData { BirthDate = DateTime.Now.AddYears(-40), Gender = Gender.Male, Email = "user@latinmail.com" }
               }
           );

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            base.Seed(context);
        }
    }
}
