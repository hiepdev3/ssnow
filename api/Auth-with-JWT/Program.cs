using Auth_with_JWT.Data;
using Auth_with_JWT.Services;
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

            // Add services to the container.
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
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!))
                };
            });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost3000", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // Allow requests from localhost:3000
                          .AllowAnyHeader() // Allow all headers
                          .AllowAnyMethod(); // Allow all HTTP methods
                });
            });

            // Add Swagger services only ONCE
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("auth", new OpenApiInfo { Title = "Auth APIs", Version = "v1" });
                //options.SwaggerDoc("weather", new OpenApiInfo { Title = "Weather APIs", Version = "v1" });

                options.DocInclusionPredicate((groupName, apiDesc) => true);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth APIs");
                    options.SwaggerEndpoint("/swagger/weather/swagger.json", "Weather APIs");
                    options.RoutePrefix = "";
                });
            }

            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowLocalhost3000");

            app.UseAuthorization();

            app.MapControllers();

            // Seed data
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                // Ensure the database and tables are created if they do not exist.
                // This will NOT recreate tables if they already exist.
                context.Database.EnsureCreated();

                // If you want to use migrations and apply schema changes, use Migrate() instead.
                // context.Database.Migrate();

                // Seed Roles
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role { Id = 1, Name = "Admin" },
                        new Role { Id = 2, Name = "Manager" },
                        new Role { Id = 3, Name = "Customer" }
                    );
                }

                // Seed Membership Tiers
                if (!context.MembershipTiers.Any())
                {
                    context.MembershipTiers.AddRange(
                        new MembershipTier { Id = 1, TierName = "Silver", MinTotalPayment = 0 },
                        new MembershipTier { Id = 2, TierName = "Gold", MinTotalPayment = 500000 },
                        new MembershipTier { Id = 3, TierName = "Diamond", MinTotalPayment = 1000000 }
                    );
                }

                // Seed Users
                if (!context.Users.Any())
                {
                    var passwordHasher = new PasswordHasher<User>();

                    context.Users.AddRange(
                        new User
                        {
                            Email = "admin@gmail.com",
                            PasswordHash = passwordHasher.HashPassword(null, "123456"),
                            FullName = "Admin User",
                            RoleId = 1,
                            TierId = 1,
                            TotalAmount = 0,
                            Status = true,
                            CartCount = 0
                        },
                        new User
                        {
                            Email = "manager@gmail.com",
                            PasswordHash = passwordHasher.HashPassword(null, "123456"),
                            FullName = "Manager User",
                            RoleId = 2,
                            TierId = 2,
                            TotalAmount = 500000,
                            Status = true,
                            CartCount = 0
                        },
                        new User
                        {
                            Email = "customer@gmail.com",
                            PasswordHash = passwordHasher.HashPassword(null, "123456"),
                            FullName = "Customer User",
                            RoleId = 3,
                            TierId = 3,
                            TotalAmount = 1000000,
                            Status = true,
                            CartCount = 0
                        }
                    );
                }

                // Save changes to the database
                context.SaveChanges();
            }

            // Remove or comment out the redundant migration call below if using EnsureCreated above
            /*
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                db.Database.Migrate();
            }
            */

            app.Run();
        }
    }
}
