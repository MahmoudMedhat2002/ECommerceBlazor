using EcommerceBlazor.Server.Services;
using EcommerceBlazor.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBlazor.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AddressController : ControllerBase
	{
		private readonly IAddressService _addressService;

		public AddressController(IAddressService addressService)
        {
			_addressService = addressService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAddress()
		{
			var response = await _addressService.GetAddress();
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> AddOrUpdateAddress(Address address)
		{
			var response = await _addressService.AddOrUpdateAddress(address);
			return Ok(response);
		}
	}
}
