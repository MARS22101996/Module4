using System;
using System.Data.Entity;
using Auth.Host.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Auth.Host.Initializer
{
    class AuthDatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            // Create roles for system.
            var roles = new[]
            {
                new IdentityRole("Admin"),
                new IdentityRole("Support"),
                new IdentityRole("User"),
            };
            foreach (var r in roles)
            {
                context.Roles.Add(r);
            }
            context.SaveChanges();
            // Create users.
            var users = new[]
            {
                new ApplicationUser
                {
                    Email = "admin@periodicals.com",
                    UserName = "admin@periodicals.com",
                },
                new ApplicationUser
                {
                    Email = "support1@periodicals.com",
                    UserName = "support1@periodicals.com",
                },
                 new ApplicationUser
                {
                    Email = "user1@periodicals.com",
                    UserName = "user1@periodicals.com",
                }
            };
            foreach (var user in users)
            {
                CheckResult(userManager.Create(user));
                CheckResult(userManager.AddPassword(user.Id, "password"));

            }
            context.SaveChanges();

            // Set roles to main users.
            CheckResult(userManager.AddToRole(users[0].Id, "Admin"));
            CheckResult(userManager.AddToRole(users[1].Id, "Support"));
            CheckResult(userManager.AddToRole(users[2].Id, "User"));

            context.SaveChanges();

            base.Seed(context);
        }
        private void CheckResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new Exception("False");
            }
        }
    }
}