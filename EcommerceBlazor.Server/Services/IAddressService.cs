using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Server.Services
{
	public interface IAddressService
	{
		Task<ServiceResponse<Address>> GetAddress();
		Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address);
	}
}
