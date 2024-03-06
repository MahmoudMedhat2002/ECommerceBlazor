using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Server.Services
{
	public interface ICartService
	{
		Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems);
		Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems);
		Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts(int? userId = null);
		Task<ServiceResponse<int>> GetCartItemsCount();
		Task<ServiceResponse<bool>> AddToCart(CartItem cartItem);
		Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem);
		Task<ServiceResponse<bool>> RemoveItemFromCart(int productId , int productTypeId);

	}
}
