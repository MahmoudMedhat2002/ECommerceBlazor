﻿using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Shared;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceBlazor.Server.Services
{
	public class CartService : ICartService
	{
		private readonly AppDbContext _context;
		private readonly IAuthService _authService;

		public CartService(AppDbContext context , IAuthService authService )
        {
			_context = context;
			_authService = authService;
        }
        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
		{
			var result = new ServiceResponse<List<CartProductResponse>>
			{
				Data = new List<CartProductResponse>()
			};

			foreach (var cartItem in cartItems)
			{
				var product = await _context.Products.Where(p => p.Id == cartItem.ProductId).FirstOrDefaultAsync();

				if(product == null)
				{
					continue;
				}

				var productVariant = await _context.ProductVariants.Where(v => v.ProductId == cartItem.ProductId
				&& v.ProductTypeId == cartItem.ProductTypeId).Include(v => v.ProductType).FirstOrDefaultAsync();

				if (productVariant == null)
				{
					continue;
				}

				var cartProduct = new CartProductResponse
				{
					ProductId = product.Id,
					Title = product.Title,
					Price = productVariant.Price,
					ImageUrl = product.ImageURL,
					ProductType = productVariant.ProductType.Name,
					ProductTypeId = productVariant.ProductTypeId,
					Quantity = cartItem.Quantity
				};

				result.Data.Add(cartProduct);
			}

			return result;
		}

		public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
		{
			cartItems.ForEach(cartItem => cartItem.UserId = _authService.GetUserId());
			_context.CartItems.AddRange(cartItems);
			await _context.SaveChangesAsync();

			return await GetDbCartProducts();
		}

		public async Task<ServiceResponse<int>> GetCartItemsCount()
		{
			int count = (await _context.CartItems.Where(ci => ci.UserId == _authService.GetUserId()).ToListAsync()).Count;
			return new ServiceResponse<int> { Data = count };
		}

		public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts(int? userId = null)
		{
			if(userId == null)
				userId = _authService.GetUserId();

			return await GetCartProducts(await _context.CartItems.Where(ci => ci.UserId == userId).ToListAsync());
		}

		public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
		{
			cartItem.UserId = _authService.GetUserId();

			var sameItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
			ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == cartItem.UserId);

            if (sameItem == null)
            {
                _context.CartItems.Add(cartItem);
            }
			else
			{
				sameItem.Quantity += cartItem.Quantity;
			}

			await _context.SaveChangesAsync();

			return new ServiceResponse<bool> { Data = true };

        }

		public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem)
		{
			var dbCartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
			ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == _authService.GetUserId());

			if(dbCartItem == null)
			{
				return new ServiceResponse<bool>
				{
					Data = false,
					Success = false,
					Message = "The cart item does not exist"
				};
			}

			dbCartItem.Quantity = cartItem.Quantity;
			await _context.SaveChangesAsync();

			return new ServiceResponse<bool> { Data = true };
		}

		public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
		{
			var dbCartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == productId &&
			ci.ProductTypeId == productTypeId && ci.UserId == _authService.GetUserId());

			if (dbCartItem == null)
			{
				return new ServiceResponse<bool>
				{
					Data = false,
					Success = false,
					Message = "The cart item does not exist"
				};
			}

			_context.CartItems.Remove(dbCartItem);
			await _context.SaveChangesAsync();

			return new ServiceResponse<bool> { Data = true };
		}
	}
}
