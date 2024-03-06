using EcommerceBlazor.Shared;
using ECommerceBlazor.Shared;

namespace EcommerceBlazor.Client.Services
{
    public interface IProductService
    {
		event Action ProductChange;

		string? Message { get; set; }
		int CurrentPage { get; set; }
		int PageCount { get; set; }
		string LastSearchText { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> AdminProducts { get; set; }
        Task GetProducts(string? categoryname = null);
        Task GetAdminProducts();
		Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryname);
		Task<ServiceResponse<Product>> GetByIdAsync(int id);
		Task InsertAsync(Product entity);
		Task UpdateAsync(int id, Product entity);
		Task DeleteAsync(int id);
		Task SearchProducts(string searchText, int page);
		Task<List<string>> GetProductSearchSuggestions(string searchText);
		Task<Product> CreateProduct(Product product);
		Task<Product> UpdateProduct(Product product);
		Task DeleteProduct(Product product);
	}
}
