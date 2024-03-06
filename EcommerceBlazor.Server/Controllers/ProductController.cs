using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Server.Services;
using EcommerceBlazor.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		[HttpGet("admin")]
		public async Task<IActionResult> GetAdminProducts()
		{
			var response = await _productService.GetAdminProducts();
			if (response == null)
			{
				response.Success = false;
				response.Message = "There was an error !!!";
			}
			return Ok(response);
		}

		[HttpPut]
		public async Task<IActionResult> UpdateProduct(Product product)
		{
			var response = await _productService.UpdateProduct(product);
			return Ok(response);
		}

		[HttpDelete("{productId}")]
		public async Task<IActionResult> DeleteProduct(int productId)
		{
			var response = await _productService.DeleteProduct(productId);
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> CreateProduct(Product product)
		{
			var response = await _productService.CreateProduct(product);
			return Ok(response);
		}

		[HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productService.GetProductsWithVariants();
            if(response == null)
            {
                response.Success = false;
                response.Message = "There was an error !!!";
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
		public async Task<IActionResult> GetById(int id)
        {
            var response = await _productService.GetProductAsync(id);
			if(response == null)
            {
                response.Success = false;
                response.Message = "There was an error !!!";
            }
            
            return Ok(response);
        }

        [HttpGet("category/{categoryname}")]
        public async Task<IActionResult> GetByCategoryName(string categoryname)
        {
            var response = await _productService.GetProductByCategoryAsync(categoryname);
            return Ok(response);
        }

		[HttpGet("search/{searchText}/{page}")]
		public async Task<IActionResult> SearchProducts(string searchText , int page = 1)
		{
			var response = await _productService.SearchProducts(searchText , page);
            return Ok(response);
		}

		[HttpGet("searchsuggestions/{searchText}")]
		public async Task<IActionResult> SearchProductsSuggestions(string searchText)
		{
			var response = await _productService.GetProductSearchSuggestions(searchText);
			return Ok(response);
		}

		[HttpGet("featured")]
		public async Task<IActionResult> GetFeaturedProducts()
		{
			var response = await _productService.GetFeaturedProducts();
			return Ok(response);
		}
	}
}
