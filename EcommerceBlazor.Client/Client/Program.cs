global using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using EcommerceBlazor.Client;
using EcommerceBlazor.Client.Services;
using EcommerceBlazor.Shared;
using ECommerceBlazor.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace EcommerceBlazor.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddBlazoredLocalStorage();
			builder.Services.AddMudServices();
			builder.Services.AddScoped<IProductService, ProductService>();
			builder.Services.AddScoped<ICategoryService, CategoryService>();
			builder.Services.AddScoped<ICartService, CartService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IOrderService, OrderService>();
			builder.Services.AddScoped<IAddressService, AddressService>();
			builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["ip"]) });

            await builder.Build().RunAsync();
        }
    }
}