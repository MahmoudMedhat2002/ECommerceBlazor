using EcommerceBlazor.Shared;
using Stripe.Checkout;

namespace EcommerceBlazor.Server.Services
{
	public interface IPaymentService
	{
		Task<Session> CreateCheckoutSession();
		Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request);
	}
}
