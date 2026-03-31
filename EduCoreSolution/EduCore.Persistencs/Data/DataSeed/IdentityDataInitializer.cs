using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.DataSeed
{
    public class IdentityDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public IdentityDataInitializer(UserManager<User> userManager,
                                        RoleManager<IdentityRole> roleManager) {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task InitializeAsync()
        {
            try
            {
                if (!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("Student"));
                    await roleManager.CreateAsync(new IdentityRole("Teacher"));
                }
                if (!userManager.Users.Any())
                {
                    var user01 = new User
                    {
                        Name = "Hala",
                        UserName = "HalaMedhat",
                        Email = "halamedhat2486@gmail.com",
                        PhoneNumber = "01100775305"


                    };
                    var user02 = new User
                    {
                        Name = "Menna",
                        UserName = "Menna012",
                        Email = "Menna@gmail.com",
                        PhoneNumber = "0113245878"


                    };
                    await userManager.CreateAsync(user01);
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}
