using Hospital.Utilities.Interfaces;
using Microsoft.AspNetCore.Identity;
using Hospital.Databases;
using Microsoft.EntityFrameworkCore;
using Hospital.Models;

namespace Hospital.Utilities
{
    public class DbInitializer : DbInitializerInterface
    {
        private UserManager<IdentityUser> _userManager;
        
        private RoleManager<IdentityRole> _roleManager;
        
        private ApplicationDbContext _DbContext;
            
        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext DbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _DbContext = DbContext;
        }
        
        public void Initialize()
        {
            // * migrate the unapplied migrations
            try
            {
                if (_DbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _DbContext.Database.Migrate();
                }
            }
            catch (System.Exception)
            {
                
                throw;
            }

            // Determine if there are any roles in database
            // If there are no roles, create the roles
            if (!_roleManager.RoleExistsAsync(ApplicationRoles.Administrator).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Administrator)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Doctor)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(ApplicationRoles.Patient)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Developer",
                    Email = "developer@mail.com",
                }, "developer").GetAwaiter().GetResult();

                ApplicationUser user = _DbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "developer@mail.com");

                if (user != null) {
                    _userManager.AddToRoleAsync(user, ApplicationRoles.Administrator);
                }
            }
        }
    }
}