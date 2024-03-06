using EcommerceBlazor.Shared;
using System.Net.Http.Json;

namespace EcommerceBlazor.Client.Services
{
	public class AuthService : IAuthService
	{
		private readonly HttpClient _http;
		private readonly AuthenticationStateProvider _authStateProvider;

		public AuthService(HttpClient http,AuthenticationStateProvider authStateProvider)
        {
            _http = http;
			_authStateProvider = authStateProvider;
		}

		public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request)
		{
			var response = await _http.PostAsJsonAsync("/api/Auth/change-password", request.Password);
			return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
		}

		public async Task<bool> IsUserAuthenticated()
		{
			return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
		}

		public async Task<ServiceResponse<string>> Login(UserLogin request)
		{
			var response = await _http.PostAsJsonAsync("/api/Auth/login", request);
			return await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();
		}

		public async Task<ServiceResponse<int>> Register(UserRegister request)
		{
			var response = await _http.PostAsJsonAsync("/api/Auth/register", request);
			return await response.Content.ReadFromJsonAsync<ServiceResponse<int>>();
		}
	}
}
