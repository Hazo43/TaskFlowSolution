
using Domain.Entites;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presistence.Data.DataSeed;
using Presistence.Data.DbContexts;
using Presistence.Data.Repositories;
using Presistence.Data.Unitofwork;
using Services;
using Services.Abstraction.Interfaces;
using Services.ImplementaionService;
using System.Text;

namespace TaskFlow
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Add services to the container



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // TaskFlowDbContext
            builder.Services.AddDbContext<TaskFlowDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            // Identity
            builder.Services.AddIdentityCore<User>()
                    .AddRoles<IdentityRole<int>>()
                    .AddEntityFrameworkStores<TaskFlowDbContext>();

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"] ?? string.Empty)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero

                };
            });

            // Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                // في الواجهه Authorize عشان يظهر زراز ال Bearer اسمه Auth ان فيه نوع  Swagger دا بيعمل اي ؟ بيعرف ال 
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    // 
                    In = ParameterLocation.Header, // Http Header في ال passing بتاعي لازم يحصلها Token معناها ان ال
                    Description = "Please Enter a Valid Token", // Token دي المسدج اللي هتبقي ظاهره وانا بدخل ال
                    Name = "Authorization", // JwtToken اللي انا بستخدمو عشان اباصي ال Http Header بتاع ال Name دا ال
                    Type = SecuritySchemeType.Http, // Http ان انا شغال Swagger بفهم ال
                    BearerFormat = "JWT", // JWT بقولو ان انا شغال بالشكل بتاع ال
                    Scheme = "Bearer"
                });
                // Request يتبعت تلقائي في ال Token ال Authorize يعني لما اضغط APIs يطبق النظام دا علي ال Swagger بيخلي ال
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        // بالتعريف اللي عملناه فوق requirement بيربط ال
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string []{ }
                    }
                });
            });

            // IAuthenticationService 
            builder.Services.AddScoped(typeof(IAuthenticationService), typeof(AuthenticationService));
          
            // token من ال User بتاع ال id عشان نجيب ال
            builder.Services.AddHttpContextAccessor();
          
            // اللي عندو UserId  مين المستخدم الحالي من خلال الservices بيعرف ال class ال
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            // IProjectService
            builder.Services.AddScoped<IProjectService, ProjectService>();
            // IUnit Of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Data Seeding
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            //IProjectMemberRepository
            builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
            // Auto Mapper
            builder.Services.AddAutoMapper(cfg => { }, typeof(AssembleyReference).Assembly);
            // ITask Service
            builder.Services.AddScoped<ITaskService, TaskService>();

            #endregion


            var app = builder.Build();

            // Add DataSeeding 
            using var scope = app.Services.CreateScope();
            var objectOfDataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await objectOfDataSeeding.SeedIdentityDataAsync(); // Identity DataSeeding
            await objectOfDataSeeding.DataSeedAsync(); // Seeding Data

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            // JWT validation on a Token 
            app.UseAuthentication();// Step  1
            app.UseAuthorization(); // step  2 

            app.MapControllers();

            app.Run();
        }
    }
}
