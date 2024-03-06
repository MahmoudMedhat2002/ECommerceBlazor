using Blazored.LocalStorage;
using EcommerceBlazor.Shared;
using ECommerceBlazor.Shared;
using System.Net.Http.Json;

namespace EcommerceBlazor.Client.Services
{
	public class CartService : ICartService
	{

		private readonly ILocalStorageService _localStorage;
		private readonly HttpClient _httpClient;
		private readonly IAuthService _authService;
		public CartService(ILocalStorageService localStorage , HttpClient httpClient , IAuthService authService)
        {
			_localStorage = localStorage;
			_httpClient = httpClient;
			_authService = authService;
		}

		public event Action OnChange;

		public async Task AddToCartAsync(CartItem cartItem)
		{

			if (await _authService.IsUserAuthenticated())
			{
				await _httpClient.PostAsJsonAsync("/api/Cart/add", cartItem);
			}
			else
			{
				var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
				if (cart == null)
				{
					cart = new List<CartItem>();
				}
				var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);

				if (sameItem == null)
				{
					cart.Add(cartItem);
				}
				else
				{
					sameItem.Quantity += cartItem.Quantity;
				}

				await _localStorage.SetItemAsync("cart", cart);
			}
			
			await GetCartItemsCount();
		}

		public async Task GetCartItemsCount()
		{
			if(await _authService.IsUserAuthenticated())
			{
				var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
				var count = result.Data;

				await _localStorage.SetItemAsync("cartItemsCount", count);
			}
			else
			{
				var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
				await _localStorage.SetItemAsync("cartItemsCount", cart != null ? cart.Count : 0);
			}

			OnChange.Invoke();
		}

		public async Task<List<CartProductResponse>> GetCartProducts()
		{
			if(await _authService.IsUserAuthenticated())
			{
				var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("/api/Cart");
				return response.Data;
			}
			else
			{
				var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
				if (cartItems == null)
					return new List<CartProductResponse>();
				var response = await _httpClient.PostAsJsonAsync<List<CartItem>>("/api/Cart/products", cartItems);

				var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

				return cartProducts.Data;
			}
			
		}

		public async Task RemoveProductFromCart(int productId, int productTypeId)
		{
			if (await _authService.IsUserAuthenticated())
			{
				await _httpClient.DeleteAsync($"/api/Cart/{productId}/{productTypeId}");
			}
			else
			{
				var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

				if (cart == null)
				{
					return;
				}

				var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

				if (cartItem != null)
				{
					cart.Remove(cartItem);
					await _localStorage.SetItemAsync("cart", cart);
				}
			}
		}

		public async Task StoreCartItems(bool emptyLocalCart = false)
		{
			var localStorage = await _localStorage.GetItemAsync<List<CartItem>>("cart");

			if (localStorage == null)
			{
				return;
			}

			await _httpClient.PostAsJsonAsync("api/cart", localStorage);

			if(emptyLocalCart)
			{
				await _localStorage.RemoveItemAsync("cart");
			}
		}

		public async Task UpdateQuantity(CartProductResponse product)
		{

			if(await _authService.IsUserAuthenticated())
			{
				var request = new CartItem
				{
					ProductId = product.ProductId,
					Quantity = product.Quantity,
					ProductTypeId = product.ProductTypeId
				};

				await _httpClient.PutAsJsonAsync("/api/Cart/update-quantity", request);
			}
			else
			{
				var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

				if (cart == null)
				{
					return;
				}

				var cartItem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);

				if (cartItem != null)
				{
					cartItem.Quantity = product.Quantity;
					await _localStorage.SetItemAsync("cart", cart);
				}
			}
		}

	}
}
