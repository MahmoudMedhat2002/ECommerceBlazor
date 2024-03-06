using EcommerceBlazor.Server.Base;
using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Server.Services
{
	public interface ICategoryService : IEntityBaseService<Category>
	{
		Task<ServiceResponse<List<Category>>> GetCategories();
		Task<Category> GetCategoryById(int id);
		Task<ServiceResponse<List<Category>>> GetAdminCategories();
		Task<ServiceResponse<List<Category>>> AddCategory(Category category);
		Task<ServiceResponse<List<Category>>> UpdateCategory(Category category);
		Task<ServiceResponse<List<Category>>> DeleteCategory(int id);
	}
}
