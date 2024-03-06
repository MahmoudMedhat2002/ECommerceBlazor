using EcommerceBlazor.Shared;

namespace EcommerceBlazor.Client.Services
{
	public interface IAddressService
	{
		Task<Address> GetAddress();
		Task<Address> AddOrUpdateAddress(Address address);
	}
}
