using Auth_with_JWT.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Cryptography;
using Auth_with_JWT.Entities;
using Pomelo.EntityFrameworkCore.MySql;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Identity;


namespace Auth_with_JWT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========== Add services ==========

            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
                ));


            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

           

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("auth", new OpenApiInfo { Title = "Auth APIs", Version = "v1" });
                options.DocInclusionPredicate((groupName, apiDesc) => true);
            });

            // ========== Build App ==========
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                dbContext.Database.Migrate(); // hoặc dbContext.Database.EnsureCreated(); nếu không dùng migration
            }
            // ========== Configure Middleware ==========
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth APIs");
                    // Uncomment if using other endpoints
                    // options.SwaggerEndpoint("/swagger/weather/swagger.json", "Weather APIs");
                    options.RoutePrefix = "";
                });
            }

            app.UseHttpsRedirection();

         

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
