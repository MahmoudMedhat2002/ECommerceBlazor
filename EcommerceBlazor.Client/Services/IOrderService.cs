using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Client.Services
{
	public interface IOrderService
	{
		Task<string> PlaceOrder();
		Task<List<OrderOverviewResponse>> GetOrders();
		Task<OrderDetailsResponse> GetOrderDetails(int orderId);
	}
}
