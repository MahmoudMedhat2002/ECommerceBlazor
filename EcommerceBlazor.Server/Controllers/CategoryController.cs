using EcommerceBlazor.Server.Services;
using EcommerceBlazor.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBlazor.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			this._categoryService = categoryService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var response = await _categoryService.GetCategories();
			return Ok(response);
		}

		[HttpGet("admin")]
		public async Task<IActionResult> GetAdminCategories()
		{
			var response = await _categoryService.GetAdminCategories();
			return Ok(response);
		}

		[HttpDelete("admin/{id}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var response = await _categoryService.DeleteCategory(id);
			return Ok(response);
		}

		[HttpPost("admin")]
		public async Task<IActionResult> AddCategory(Category category)
		{
			var response = await _categoryService.AddCategory(category);
			return Ok(response);

		}

		[HttpPut("admin")]
		public async Task<IActionResult> UpdateCategory(Category category)
		{
			var response = await _categoryService.UpdateCategory(category);
			return Ok(response);
		}
	}
}
