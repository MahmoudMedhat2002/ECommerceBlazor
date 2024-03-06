using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Shared;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly AppDbContext _context;

        public ProductTypeService(AppDbContext context)
        {
            _context = context;
        }

		public async Task<ServiceResponse<List<ProductType>>> AddProductType(ProductType productType)
		{
            productType.IsNew = productType.Editing = false;
			await _context.ProductTypes.AddAsync(productType);
            await _context.SaveChangesAsync();
            return await GetProductTypes();
		}

		public async Task<ServiceResponse<List<ProductType>>> UpdateProductType(ProductType productType)
		{
			var dbProductType = await _context.ProductTypes.FindAsync(productType.Id);
            if(dbProductType == null)
            {
                return new ServiceResponse<List<ProductType>> 
                {
                    Success = false,
                    Message = "ProductType not found!!!"
                };
            }
            dbProductType.Name = productType.Name;
            await _context.SaveChangesAsync();
            return await GetProductTypes();
		}

		public async Task<ServiceResponse<List<ProductType>>> GetProductTypes()
        {
            var productTypes = await _context.ProductTypes.ToListAsync();
            return new ServiceResponse<List<ProductType>> { Data = productTypes };
        }
    }
}
