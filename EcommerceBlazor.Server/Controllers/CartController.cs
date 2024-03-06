using EcommerceBlazor.Server.Services;
using EcommerceBlazor.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceBlazor.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
			_cartService = cartService;
		}

		[HttpPost("products")]
		public async Task<IActionResult> GetCartProducts(List<CartItem> cartItems)
		{
			var result = await _cartService.GetCartProducts(cartItems);
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> StoreCartItems(List<CartItem> cartItems)
		{
			var result = await _cartService.StoreCartItems(cartItems);
			return Ok(result);
		}

		[HttpPost("add")]
		public async Task<IActionResult> AddToCart(CartItem cartItem)
		{
			var result = await _cartService.AddToCart(cartItem);
			return Ok(result);
		}

		[HttpPut("update-quantity")]
		public async Task<IActionResult> UpdateQuantity(CartItem cartItem)
		{
			var result = await _cartService.UpdateQuantity(cartItem);
			return Ok(result);
		}

		[HttpDelete("{productId}/{productTypeId}")]
		public async Task<IActionResult> RemoveItemFromCart(int productId , int productTypeId)
		{
			var result = await _cartService.RemoveItemFromCart(productId , productTypeId);
			return Ok(result);
		}

		[HttpGet("count")]
		public async Task<IActionResult> GetCartItemsCount()
		{
			var result = await _cartService.GetCartItemsCount();
			return Ok(result);
		}

		[HttpGet]
		public async Task<IActionResult> GetDbCartProducts()
		{
			var result = await _cartService.GetDbCartProducts();
			return Ok(result);
		}
	}
}
