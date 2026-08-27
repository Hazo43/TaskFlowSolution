
using Domain.Entites;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presistence.Data.DataSeed;
using Presistence.Data.DbContexts;
using Services.Abstraction.Interfaces;
using Services.ImplementaionService;

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
            builder.Services.AddSwaggerGen();

            // TaskFlowDbContext
            builder.Services.AddDbContext<TaskFlowDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            // Identity
            builder.Services.AddIdentityCore<User>()
                    .AddRoles<IdentityRole<int>>()
                    .AddEntityFrameworkStores<TaskFlowDbContext>();

            // IAuthenticationService 
            builder.Services.AddScoped(typeof(IAuthenticationService), typeof(AuthenticationService));
            // Data Seeding
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
