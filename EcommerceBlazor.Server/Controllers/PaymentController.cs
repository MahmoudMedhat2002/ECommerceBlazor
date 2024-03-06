using EcommerceBlazor.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBlazor.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;

		public PaymentController(IPaymentService paymentService)
        {
			_paymentService = paymentService;
		}

		[HttpPost("checkout")]
		public async Task<IActionResult> CreateCheckoutSession()
		{
			var session = await _paymentService.CreateCheckoutSession();
			return Ok(session.Url);
		}

		[HttpPost]
		public async Task<IActionResult> FulfillOrder()
		{
			var response = await _paymentService.FulfillOrder(Request);

			if(!response.Success)
			{
				return BadRequest(response.Message);
			}

			return Ok(response);
		}
	}
}
