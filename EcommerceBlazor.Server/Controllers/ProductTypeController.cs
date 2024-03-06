using EcommerceBlazor.Server.Services;
using EcommerceBlazor.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProductTypes()
        {
            var result = await _productTypeService.GetProductTypes();
            return Ok(result);
        }
		[HttpPost]
		public async Task<IActionResult> AddProductType(ProductType productType)
		{
			var result = await _productTypeService.AddProductType(productType);
			return Ok(result);
		}
		[HttpPut]
		public async Task<IActionResult> UpdateProductType(ProductType productType)
		{
			var result = await _productTypeService.UpdateProductType(productType);
			return Ok(result);
		}
	}
}
