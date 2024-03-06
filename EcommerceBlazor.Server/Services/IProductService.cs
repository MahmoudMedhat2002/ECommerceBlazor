using EcommerceBlazor.Server.Base;
using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Server.Services
{
	public interface IProductService : IEntityBaseService<Product>
	{
		Task<ServiceResponse<List<Product>>> GetProductByCategoryAsync(string categoryname);
        Task<ServiceResponse<List<Product>>> GetProductsWithVariants();
        Task<ServiceResponse<Product>> GetProductAsync(int id);
		Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page);
		Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText);
		Task<ServiceResponse<List<Product>>> GetFeaturedProducts();
		Task<ServiceResponse<List<Product>>> GetAdminProducts();
		Task<ServiceResponse<Product>> CreateProduct(Product product);
		Task<ServiceResponse<Product>> UpdateProduct(Product product);
		Task<ServiceResponse<bool>> DeleteProduct(int productId);
	}
}
