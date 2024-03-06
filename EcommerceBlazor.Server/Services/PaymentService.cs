using EcommerceBlazor.Shared;
using Stripe;
using Stripe.Checkout;

namespace EcommerceBlazor.Server.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly ICartService _cartService;
		private readonly IAuthService _authService;
		private readonly IOrderService _orderService;

		const string secret = "whsec_62b2237e0ad57b4702f66bdd0bcb5053eb7df0bf200af5613b2d63ada6500f85";

		public PaymentService(ICartService cartService , IAuthService authService , IOrderService orderService)
        {
			StripeConfiguration.ApiKey = "sk_test_51Ome0dKAi7xrMT8xAHq6qWS7OdmBfzxbclHT44pndmXEqywbY43Lde1lb05LTHSIKzens3CqEqhvXdMIQogs9EXC00r28pUfw7";
			_cartService = cartService;
			_authService = authService;
			_orderService = orderService;
		}
        public async Task<Session> CreateCheckoutSession()
		{
			var products = (await _cartService.GetDbCartProducts()).Data;
			var lineItems = new List<SessionLineItemOptions>();

			products.ForEach(p => lineItems.Add(new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					UnitAmountDecimal = p.Price * 100,
					Currency = "usd",
					ProductData = new SessionLineItemPriceDataProductDataOptions
					{
						Name = p.Title,
						Images = new List<string> { p.ImageUrl}
					}
				},
				Quantity = p.Quantity
			}));

			var options = new SessionCreateOptions
			{
				CustomerEmail = _authService.GetUserEmail(),
				ShippingAddressCollection = new SessionShippingAddressCollectionOptions
				{
					AllowedCountries = new List<string> { "US" }
				},
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = lineItems,
				Mode = "payment",
				SuccessUrl = "https://localhost:7016/order-success",
				CancelUrl = "https://localhost:7016/cart"
			};

			var service = new SessionService();
			var session = service.Create(options);

			await _orderService.PlaceOrder(_authService.GetUserId());

			return session;
        }

		public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
		{
			var json = await new StreamReader(request.Body).ReadToEndAsync();

			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], secret);

				if(stripeEvent.Type == Events.CheckoutSessionCompleted)
				{
					var session = stripeEvent.Data.Object as Session;
					var user = _authService.GetUserByEmail(session.CustomerEmail);
				}

				return new ServiceResponse<bool> { Data = true };
			}
			catch (StripeException e)
			{
				return new ServiceResponse<bool> { Data = false , Success = false , Message = e.Message };
			}
		}
	}
}
