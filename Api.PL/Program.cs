
using RepositoryProject;
using Microsoft.EntityFrameworkCore;
using CoreProject.Repository;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using CoreProject.Enteties;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Core.Repository;
using Repository.Repository;
using EducationalPlatform.Helper;
using BookingSystem.Helper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Core.Entities;
using RepositoryProject.Repository;
using System.Text.Json.Serialization;
using Core.ServiceContract;
using Service;
using Api.PL.Helper;
using Fawaterak.Services;
using Service.ServiceContract;


namespace EducationalPlatform
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
          
           
            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BookingSystem API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'",
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });
            #endregion
             
            #region ConnectionString
            builder.Services.AddDbContext<EducationalPlatformDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.Configure<FawaterkOptions>(builder.Configuration.GetSection("Fawaterk"));

            #endregion
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            #region ServicesDi
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
          
            #endregion
            // Options binding
           
            builder.Services.AddScoped<IFawaterkPaymentService, FawaterakPaymentService>();
            // builder.Services.AddHttpClient<IFawaterkPaymentService,FawaterakPaymentService>();
            #region Identity
            //builder.Services.AddMemoryCache();
            // Register GenericRepository for all entities

            builder.Services.AddTransient<Core.ServiceContract.IEmailSender, Service.EmailSender>();
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {

            })
           .AddEntityFrameworkStores<EducationalPlatformDbContext>()
           .AddDefaultTokenProviders();
          


            // Register TokenServices
            builder.Services.AddScoped<ITokenServices, TokenServices>();
           

            

            // Add AutoMapper with MappingProfile
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var keyString = builder.Configuration["Jwt:key"] ?? throw new InvalidOperationException("JWT key is not configured");
                var key = Encoding.UTF8.GetBytes(keyString);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole(UserRole.Admin.ToString()));
                options.AddPolicy("Student", policy => policy.RequireRole(UserRole.Student.ToString()));
                options.AddPolicy("Center", policy => policy.RequireRole(UserRole.Center.ToString()));


            });


            #endregion

            builder.Services.AddControllers()
              .AddJsonOptions(options =>
              {
                  // ÌÕÊ· «·‹ enum „‰ int ≈·Ï string ›Ì «·‹ JSON
                  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
              });




            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            #region Update Database And Seeding
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var dbContext = services.GetRequiredService<EducationalPlatformDbContext>();
                await dbContext.Database.MigrateAsync();

                // SEED ROLES
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "Admin", "Student", "Center" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // SEED ADMIN USER
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var phonenumber = "01033927215";
                var adminUser = await userManager.FindByNameAsync(phonenumber);
                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = phonenumber,

                        PhoneNumber = phonenumber,
                        FullName = "Nader Rahoumah",
                        Role = UserRole.Admin
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogInformation("Admin user created successfully");
                    }
                    else
                    {
                        var logger = loggerFactory.CreateLogger<Program>();
                        logger.LogError(string.Join("; ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    // Ensure admin is in Admin role
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error During Migration");
            }
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

