using EcommerceBlazor.Server.Base;
using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Shared;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Services
{
	public class CategoryService : EntityBaseService<Category>, ICategoryService
	{
		private readonly AppDbContext _context;
        public CategoryService(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<ServiceResponse<List<Category>>> AddCategory(Category category)
		{
			category.Editing = category.IsNew = false;
			await _context.Categories.AddAsync(category);
			await _context.SaveChangesAsync();
			return await GetAdminCategories();
		}

		public async Task<ServiceResponse<List<Category>>> DeleteCategory(int id)
		{
			Category category = await GetCategoryById(id);
			if(category == null)
			{
				return new ServiceResponse<List<Category>> { Success = false , Message = "Category not found!!!" };
			}
			category.Deleted = true;
			await _context.SaveChangesAsync();
			return await GetAdminCategories();
		}

		public async Task<ServiceResponse<List<Category>>> GetAdminCategories()
		{
			var items = await _context.Categories.Where(c => !c.Deleted).ToListAsync();
			return new ServiceResponse<List<Category>> { Data = items };
		}

		public async Task<ServiceResponse<List<Category>>> GetCategories()
		{
			var items = await _context.Categories.Where(c => !c.Deleted && c.Visible).ToListAsync();
			return new ServiceResponse<List<Category>> { Data = items };
		}

		public async Task<Category> GetCategoryById(int id)
		{
			return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<ServiceResponse<List<Category>>> UpdateCategory(Category category)
		{
			Category dbcategory = await GetCategoryById(category.Id);

			if (dbcategory == null)
			{
				return new ServiceResponse<List<Category>> { Success = false, Message = "Category not found!!!" };
			}

			dbcategory.Name = category.Name;
			dbcategory.Url = category.Url;
			dbcategory.Visible = category.Visible;

			await _context.SaveChangesAsync();
			return await GetAdminCategories();
		}
	}
}
