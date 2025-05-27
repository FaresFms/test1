using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using test1.Bl;
using test1.Models;

namespace test1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var config = builder.Configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RoleClaimType = ClaimTypes.Role,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception}");
                        return Task.CompletedTask;
                    }
                };
            });
            /*        builder.Services.AddCors(options =>
                    {
                        options.AddPolicy("AllowFrontend",
                            policy =>
                            {
                                policy.WithOrigins("http://192.168.1.112:3000")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials(); // ≈–« ⁄„  ” Œœ„ «·ﬂÊﬂÌ“ √Ê «·ÂÌœ—
                            });
                    });*/
            builder.Services.AddMemoryCache();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials(); // ≈–« ⁄„  ” Œœ„ «·ﬂÊﬂÌ“ √Ê «·ÂÌœ—
                    });
            });
            builder.Services.AddScoped<IAdmin, ClsAdmin>();
            builder.Services.AddScoped<ICar, ClsCars>();
            builder.Services.AddScoped<IAd, ClsAd>();
            builder.Services.AddScoped<Isettings, ClsSettings>();
            builder.Services.AddScoped<IReview, ClsReview>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAuthorization();
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<RentCarContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
           ServiceLifetime.Scoped);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<RentCarContext>()
        .AddDefaultTokenProviders();
       
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5059); // Listen to all IPs on port 5059
            });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401; // Unauthorized »œ·« „‰ 302
                    return Task.CompletedTask;
                };
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
       
            app.UseStaticFiles();
      
            app.UseRouting();
            app.UseCors("AllowFrontend");
        
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
