
using EduCore.Domain.Contracts;
using EduCore.Domain.Contracts.Repositories;
using EduCore.Domain.Entities.AuthModel;
using EduCore.Middlewares;
using EduCore.Persistencs.Data.DataSeed;
using EduCore.Persistencs.Data.DbContexts;
using EduCore.Persistencs.Repositories;
using EduCore.Services;
using EduCore.Services.MappingProfiles;
using EduCore.Services_Abstraction;
using EduCore.Shared.Settings;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EduCore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<EduCoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<ICenterService, CenterService>();
            builder.Services.AddScoped<IQuizService, QuizService>(); 
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //builder.Services.AddAutoMapper(typeof(CenterMappingProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(ServicesAssemblyReference).Assembly);
            builder.Services.AddTransient<CenterLogoUrlResolver>();

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // User settings
                options.User.RequireUniqueEmail = true;

            })
                .AddEntityFrameworkStores<EduCoreDbContext>()
                .AddDefaultTokenProviders();

            //Hala from 56 to 66
           
            builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();






            //Samir from 67 to 77
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddControllers()
    .AddApplicationPart(
        typeof(EduCore.Presentation.Controllers.AdminCoursesController).Assembly);
            builder.Services.AddScoped<ICategoryService, CategoryService>();






            // Tawfik from 78 to 88
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IAnswerOptionService, AnswerOptionService>();
            builder.Services.AddScoped<IstudentQuizService, StudentQuizService>();
            builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });









            // Abdelbadea from 89 to 99










            // Menna from 100 to 110










            // Badr from 111 to 121











            // End

            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();
            using var scope = app.Services.CreateScope();
            
                //var context = scope.ServiceProvider.GetRequiredService<EduCoreDbContext>();
                //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                //await  QuizDataSeed.SeedAsync(context, userManager);
            //ahmed samir 137-147
            using (var seederScope = app.Services.CreateScope())
            {
                var seederUserManager = seederScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var seederRoleManager = seederScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var seederContext = seederScope.ServiceProvider.GetRequiredService<EduCoreDbContext>();

                await DataSeeder.SeedAsync(seederUserManager, seederRoleManager, seederContext);
            }
















            //using var scope = app.Services.CreateScope();
            var IdentityDataInitializerService = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Identity");
            IdentityDataInitializerService.InitializeAsync().Wait();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
