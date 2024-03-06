using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Client.Services
{
	public interface ICartService
	{
		event Action OnChange;
		Task AddToCartAsync(CartItem cartItem);
		Task<List<CartProductResponse>> GetCartProducts();
		Task RemoveProductFromCart(int productId, int productTypeId);

		Task UpdateQuantity(CartProductResponse product);
		Task StoreCartItems(bool emptyLocalCart);
		Task GetCartItemsCount();
	}
}
