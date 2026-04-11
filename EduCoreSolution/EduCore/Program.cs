
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
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
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

            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<EduCoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<ICenterService, CenterService>();
            builder.Services.AddScoped<IQuizService, QuizService>(); 
            builder.Services.AddScoped<IQuizRepository, QuizRepository>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ILessonService, LessonService>();
            builder.Services.AddScoped<IVideoLessonService, VideoLessonService>();
            builder.Services.AddScoped<ILiveSessionService, LiveSessionService>();
            builder.Services.AddValidatorsFromAssembly(typeof(EduCore.Shared.DTOs.Content.Validators.CreateLessonRequestValidator).Assembly);

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
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
                    ValidAudience = builder.Configuration["JWTOptions:Audience"],
                    IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]))
                };
            });
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http, 
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            //Samir from 67 to 77
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IImageService, ImageService>();
    //        builder.Services.AddControllers()
    //.AddApplicationPart(
    //    typeof(EduCore.Presentation.Controllers.AdminCoursesController).Assembly);
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddControllers().AddApplicationPart(typeof(EduCore.Presentation.Controllers.AdminCoursesController).Assembly).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });
            #region JWT Configuration
            // JWT — بيخلي الـ [Authorize] يقرأ الـ Token من الـ Header
            //        builder.Services.AddAuthentication(options =>
            //        {
            //            options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            //            options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            //        })
            //        .AddJwtBearer(options =>
            //        {
            //            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //            {
            //                ValidateIssuer = true,
            //                ValidateAudience = true,
            //                ValidateLifetime = true,
            //                ValidateIssuerSigningKey = true,
            //                ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
            //                ValidAudience = builder.Configuration["JWTOptions:Audience"],
            //                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            //                    System.Text.Encoding.UTF8.GetBytes(
            //                        builder.Configuration["JWTOptions:SecretKey"]!))
            //            };
            //        });
            //        builder.Services.AddSwaggerGen(c =>
            //        {
            //            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            //            {
            //                Name = "Authorization",
            //                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            //                Scheme = "Bearer",
            //                BearerFormat = "JWT",
            //                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            //                Description = "Enter: Bearer {your token}"
            //            });

            //            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            //{
            //    {
            //        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            //        {
            //            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            //            {
            //                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            //                Id   = "Bearer"
            //            }
            //        },
            //        []
            //    }
            //});
            //        });
            #endregion
            // السماح للـ Angular بالتواصل مع الـ API
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });



            // Tawfik from 78 to 88
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IAnswerOptionService, AnswerOptionService>();
            builder.Services.AddScoped<IstudentQuizService, StudentQuizService>();
            builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });
            //builder.Services.AddControllers()
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.Converters
            //        .Add(new JsonStringEnumConverter());
            //});










            // Abdelbadea from 89 to 99










            // Menna from 100 to 110
            

            builder.Services.AddScoped<IReviewService, ReviewService>();







            // Badr from 111 to 121
            










            // End

            // ── Forum ─────────────────────────────────────
            builder.Services.AddScoped<IForumService, ForumService>();
            builder.Services.AddScoped<IForumRepository, ForumRepository>();

            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();
            //using var scope = app.Services.CreateScope();

            //var context = scope.ServiceProvider.GetRequiredService<EduCoreDbContext>();
            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            //await  QuizDataSeed.SeedAsync(context, userManager);
            //ahmed samir 137-147
            //using (var seederScope = app.Services.CreateScope())
            //{
            //    var seederUserManager = seederScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            //    var seederRoleManager = seederScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //    var seederContext = seederScope.ServiceProvider.GetRequiredService<EduCoreDbContext>();

            //    await DataSeeder.SeedAsync(seederUserManager, seederRoleManager, seederContext);
            //}
            using (var initScope = app.Services.CreateScope())
            {
                var identityInit = initScope.ServiceProvider
                    .GetRequiredKeyedService<IDataInitializer>("Identity");
                await identityInit.InitializeAsync();
            }

            // ── Forum Data Seed (for testing — remove before merge) ──
            using (var forumScope = app.Services.CreateScope())
            {
                var forumContext = forumScope.ServiceProvider.GetRequiredService<EduCoreDbContext>();
                var forumUserManager = forumScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await ForumDataSeed.SeedAsync(forumContext, forumUserManager);
            }















            //using var scope = app.Services.CreateScope();
            //var IdentityDataInitializerService = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Identity");
            //IdentityDataInitializerService.InitializeAsync().Wait();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAngular");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
