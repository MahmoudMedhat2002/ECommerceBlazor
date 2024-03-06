global using ECommerceBlazor.Shared;
using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceBlazor.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(c => c.AddPolicy("MyPolicy" , 
                options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            builder.Services.AddControllers();

            builder.Services.AddScoped<IProductService , ProductService>();
			builder.Services.AddScoped<ICategoryService, CategoryService>();
			builder.Services.AddScoped<ICartService, CartService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IOrderService, OrderService>();
			builder.Services.AddScoped<IPaymentService, PaymentService>();
			builder.Services.AddScoped<IAddressService, AddressService>();
			builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

			builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseCors("MyPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}