using EcommerceBlazor.Shared;
using ECommerceBlazor.Shared;

namespace EcommerceBlazor.Client.Services
{
    public interface ICategoryService
    {
		event Action OnChange;
        public List<Category> Categories { get; set; }
        public List<Category> AdminCategories { get; set; }
        Task GetCategories();
        Task GetAdminCategories();
		Task AddCategory(Category category);
		Task UpdateCategory(Category category);
		Task DeleteCategory(int categoryId);
		Category CreateNewCategory();
	}
}
