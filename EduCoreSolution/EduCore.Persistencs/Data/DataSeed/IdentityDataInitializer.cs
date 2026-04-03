using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.AuthModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IdentityDataInitializer> logger;

        public IdentityDataInitializer(UserManager<User> userManager,
                                        RoleManager<IdentityRole> roleManager,
                                        ILogger<IdentityDataInitializer> logger) {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
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
                        PhoneNumber = "01100775305",
                        CenterId=3,


                    };
                    var user02 = new User
                    {
                        Name = "Menna",
                        UserName = "Menna012",
                        Email = "Menna@gmail.com",
                        PhoneNumber = "0113245878",
                        CenterId = 2,



                    };
                    var user03 = new User
                    {
                        Name = "Mohamed",
                        UserName = "Mohamed012",
                        Email = "Mm@gmail.com",
                        PhoneNumber = "0113245878",
                        CenterId = 2,


                    };
                    await userManager.CreateAsync(user01,"Hh@123");
                    await userManager.CreateAsync(user02,"Mm#123");
                    await userManager.CreateAsync(user03,"Mm@123");
                    //await userManager.AddToRoleAsync(user01, "Admin");
                    //await userManager.AddToRoleAsync(user02, "Student");
                    //await userManager.AddToRoleAsync(user03, "Teatcher");

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding identity data.");
            }
        }
    }
}
