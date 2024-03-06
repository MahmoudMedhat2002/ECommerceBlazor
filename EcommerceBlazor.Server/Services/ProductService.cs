using EcommerceBlazor.Server.Base;
using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Shared;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace EcommerceBlazor.Server.Services
{
	public class ProductService : EntityBaseService<Product> , IProductService
	{
        private readonly AppDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ProductService(AppDbContext context , IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<ServiceResponse<Product>> CreateProduct(Product product)
		{
			foreach(var variant in product.Variants)
            {
                variant.ProductType = null;
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return new ServiceResponse<Product> { Data = product };
		}

		public async Task<ServiceResponse<bool>> DeleteProduct(int productId)
		{
			var dbProduct = await _context.Products.FindAsync(productId);

            if (dbProduct == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Product not found !!!"
                };
            }

            dbProduct.Deleted = true;
            _context.SaveChanges();
            return new ServiceResponse<bool> { Data = true };
        }

		public async Task<ServiceResponse<List<Product>>> GetAdminProducts()
		{
			var products = await _context.Products.Where(p => !p.Deleted).Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.ProductType).Include(p => p.Images).ToListAsync();

			var response = new ServiceResponse<List<Product>>();
			if (products != null)
			{
				response.Data = products;
			}
			return response;
		}

		public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
		{
			var products = await _context.Products.Include(p => p.Variants.Where(v => v.Visible && !v.Deleted))
                .Where(p => p.Featured && p.Visible && !p.Deleted).Include(p => p.Images).ToListAsync();

            var response = new ServiceResponse<List<Product>>
            {
                Data = products
            };

            return response;
		}

		public async Task<ServiceResponse<Product>> GetProductAsync(int id)
        {
			var response = new ServiceResponse<Product>();
            Product product = null;

            if (_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
				product = await _context.Products.Include(p => p.Variants.Where(v => !v.Deleted)).ThenInclude(v => v.ProductType)
                    .Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);
			}
            else
            {
				product = await _context.Products.Include(p => p.Variants.Where(v => v.Visible && !v.Deleted)).ThenInclude(v => v.ProductType)
                    .Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id && p.Visible && !p.Deleted);
			}
            if(product != null)
            {
                response.Data = product;
            }
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductByCategoryAsync(string categoryname)
        {
            var products = await _context.Products.Include(p => p.Variants.Where(v => v.Visible && !v.Deleted)).Include(p => p.Images)
				.Where(p => p.Category.Url.ToLower().Equals(categoryname.ToLower()) && p.Visible && !p.Deleted).ToListAsync();
            var response = new ServiceResponse<List<Product>>()
            {
                Data = products
            };
            return response;
        }

		public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
		{
            var products = await _context.Products.Where(p => p.Title.ToLower().Contains(searchText.ToLower()) || 
            p.Description.ToLower().Contains(searchText.ToLower()) && p.Visible && !p.Deleted).ToListAsync();

            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if(product.Title.Contains(searchText , StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if(product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();

                    var words = product.Description.Split().Select(s => s.Trim(punctuation));

                    foreach(var word in words)
                    {
                        if(word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>>
            {
                Data = result
            };

			
		}

		public async Task<ServiceResponse<List<Product>>> GetProductsWithVariants()
        {
            var products = await _context.Products.Where(p => p.Visible && !p.Deleted)
                .Include(p => p.Variants.Where(v => v.Visible && !v.Deleted)).ToListAsync();
            var response = new ServiceResponse<List<Product>>()
            {
                Data = products
            };
            return response;
        }

		public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText , int page)
		{

            var Allproducts = await _context.Products.Where(p => p.Title.ToLower().Contains(searchText.ToLower()) || 
            p.Description.ToLower().Contains(searchText.ToLower()) && p.Visible && !p.Deleted).Include(p => p.Images).ToListAsync();

            int count = Allproducts.Count;

            var pageResults = 2f;

            var pageCount = Math.Ceiling(count / pageResults);

            var products = Allproducts.Skip((page - 1) * (int)pageResults).Take((int)pageResults).ToList();

            var response = new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int) pageCount
                }
            };

			return response;
		}

		public async Task<ServiceResponse<Product>> UpdateProduct(Product product)
		{
			var dbProduct = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == product.Id);

			if (dbProduct == null)
			{
				return new ServiceResponse<Product>
				{
					Success = false,
					Message = "Product not found !!!"
				};
			}

            dbProduct.Title = product.Title;
            dbProduct.Description = product.Description;
            dbProduct.ImageURL = product.ImageURL;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.Visible = product.Visible;
            dbProduct.Featured = product.Featured;

            var productImages = dbProduct.Images;
            _context.Images.RemoveRange(productImages);

            dbProduct.Images = product.Images;

            foreach(var variant in product.Variants)
            {
                var dbVariant = await _context.ProductVariants
                    .SingleOrDefaultAsync(v => v.ProductId == variant.ProductId && v.ProductTypeId == variant.ProductTypeId);

                if(dbVariant == null)
                {
                    variant.ProductType = null;
                    await _context.ProductVariants.AddAsync(variant);
                }
                else
                {
                    dbVariant.ProductTypeId = variant.ProductTypeId;
                    dbVariant.Price = variant.Price;
                    dbVariant.OriginalPrice = variant.OriginalPrice;
                    dbVariant.Visible = variant.Visible;
                    dbVariant.Deleted = variant.Deleted;
                }
            }

            await _context.SaveChangesAsync();
            return new ServiceResponse<Product> { Data = product };
		}
	}
}
