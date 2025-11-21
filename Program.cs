
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.ApplicationLayer.Services;
using PerfumeryBackend.DatabaseLayer;
using System.Text;
using PerfumeryBackend.DatabaseLayer.Repositories;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;
//using PerfumeryBackend.MainLayer.Services;

namespace PerfumeryBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                WebRootPath = "wwwroot",
                ContentRootPath = Directory.GetCurrentDirectory()
            });

            ConfigureServices(builder);
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder) 
        {
            string jwtKeyValue = builder.Configuration.GetSection("Jwt")["Key"]
                ?? throw new Exception("Key value for jwt token was not founded");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()   // Разрешить все источники
                          .AllowAnyMethod()   // Разрешить все HTTP-методы
                          .AllowAnyHeader();  // Разрешить все заголовки
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKeyValue)
                        )
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["syndetheite"];
                            return Task.CompletedTask;
                        }
                    };
                });

            //Contollers
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            //Swagger     
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //BD
            builder.Services.AddDbContext<PerfumeryDbContext>(
                options => {
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //DI

            //Repositories
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductVariationsRepository, ProductVariationsRepository>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            //Service Dependencies
            //--Auth
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            builder.Services.AddScoped<IAvatarService, AvatarService>();
            

            //Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductVariationService, ProductVariationService>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
        }

    }
}
