using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Shared;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Services
{
	public class AddressService : IAddressService
	{
		private readonly AppDbContext _context;
		private readonly IAuthService _authService;

		public AddressService(AppDbContext context , IAuthService authService)
        {
			_context = context;
			_authService = authService;
		}
        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
		{
			var response = new ServiceResponse<Address>();
			var dbaddress = (await GetAddress()).Data;

			if(dbaddress == null)
			{
				address.UserId = _authService.GetUserId();
				await _context.Addresses.AddAsync(address);
				response.Data = address;
			}
			else
			{
				dbaddress.FirstName = address.FirstName;
				dbaddress.LastName = address.LastName;
				dbaddress.City = address.City;
				dbaddress.Country = address.Country;
				dbaddress.Zip = address.Zip;
				dbaddress.Street = address.Street;
				response.Data = dbaddress;
			}
			await _context.SaveChangesAsync();
			return response;

		}

		public async Task<ServiceResponse<Address>> GetAddress()
		{
			var userId = _authService.GetUserId();
			var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

			return new ServiceResponse<Address> { Data = address };
		}
	}
}
