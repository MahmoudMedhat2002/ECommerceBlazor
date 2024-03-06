using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Server.Services
{
	public interface IOrderService
	{
		Task<ServiceResponse<bool>> PlaceOrder(int userId);
		Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrders();
		Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId);

	}
}
