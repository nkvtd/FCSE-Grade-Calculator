using FCSE_Grade_Calculator.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FCSE_Grade_Calculator.Startup))]
namespace FCSE_Grade_Calculator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateDefaultRolesAndUsers();
        }

        private void CreateDefaultRolesAndUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                if (!roleManager.RoleExists("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    roleManager.Create(role);

                    var adminUser = new ApplicationUser
                    {
                        UserName = "admin@finki.ukim.mk",
                        Email = "admin@finki.ukim.mk",
                        FirstName = "Admin",
                        LastName = "Admin"
                    };

                    string adminPassword = "Admin@123";
                    var chkUser = userManager.Create(adminUser, adminPassword);

                    if (chkUser.Succeeded)
                    {
                        userManager.AddToRole(adminUser.Id, "Admin");
                    }
                }

                if (!roleManager.RoleExists("Student"))
                {
                    var role = new IdentityRole("Student");
                    roleManager.Create(role);
                }

                if (!roleManager.RoleExists("Teacher"))
                {
                    var role = new IdentityRole("Teacher");
                    roleManager.Create(role);
                }

                context.SaveChanges();
            }
        }
    }
}
