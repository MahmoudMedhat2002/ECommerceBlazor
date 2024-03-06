using EcommerceBlazor.Shared;
using ECommerceBlazor.Shared;
using System.Net.Http.Json;

namespace EcommerceBlazor.Client.Services
{
	public class ProductService : IProductService
	{
		private readonly HttpClient _httpClient;
        public ProductService(HttpClient HttpClient)
        {
            _httpClient = HttpClient;
        }

		public List<Product> Products { get; set; } = new List<Product>();
		public string? Message { get; set; } = "Products Loading ....";
		public int CurrentPage { get; set; } = 1;
		public int PageCount { get; set; } = 0;
		public string LastSearchText { get; set; } = string.Empty;
		public List<Product> AdminProducts { get; set; }

		public event Action ProductsChanged;
		public event Action ProductChange;

		public async Task<Product> CreateProduct(Product product)
		{
			var result = await _httpClient.PostAsJsonAsync("api/product", product);
			var newProduct = (await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>()).Data;
			return newProduct;
		}

		public Task DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task DeleteProduct(Product product)
		{
			var result = await _httpClient.DeleteAsync($"api/product/{product.Id}");
		}

		public async Task GetAdminProducts()
		{
			var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("/api/Product/admin");
			AdminProducts = result.Data;
			CurrentPage = 1;
			PageCount = 0;

			if (AdminProducts.Count == 0)
				Message = "Products not found !!!";
		}

		public async Task<ServiceResponse<Product>> GetByIdAsync(int id)
		{
			return await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>("/api/Product/"+id);
		}

		public async Task GetProducts(string? categoryname = null)
		{
			var result = categoryname == null ? 
				await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("/api/Product/featured") :
				await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("/api/Product/category/"+categoryname);
			if (result != null && result.Data != null)
			{
				Products = result.Data;
			}

			CurrentPage = 1;
			PageCount = 0;

			if (Products.Count == 0)
				Message = "No Products was found";

			ProductChange.Invoke();
		}

		public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryname)
		{
			return await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("/api/Product/category/"+categoryname);
		}

		public async Task<List<string>> GetProductSearchSuggestions(string searchText)
		{
			var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>("/api/Product/searchsuggestions/" + searchText);
			return result.Data;
		}

		public Task InsertAsync(Product entity)
		{
			throw new NotImplementedException();
		}

		public async Task SearchProducts(string searchText, int page)
		{
			LastSearchText = searchText;
			var result = await _httpClient.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"/api/Product/search/{searchText}/{page}");
			if(result != null && result.Data != null)
			{
				Products = result.Data.Products;
				CurrentPage = result.Data.CurrentPage;
				PageCount = result.Data.Pages;
			}
			if (Products.Count == 0)
				Message = "No Products Found";
			ProductChange.Invoke();
		}

		public Task UpdateAsync(int id, Product entity)
		{
			throw new NotImplementedException();
		}

		public async Task<Product> UpdateProduct(Product product)
		{
			var result = await _httpClient.PutAsJsonAsync("api/product", product);
			return (await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>()).Data;
		}
	}
}
