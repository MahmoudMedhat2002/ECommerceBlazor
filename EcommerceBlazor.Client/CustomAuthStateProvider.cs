using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace EcommerceBlazor.Client
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorage;
		private readonly HttpClient _http;
		public CustomAuthStateProvider(ILocalStorageService localStorage , HttpClient http)
        {
            _localStorage = localStorage;
			_http = http;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var authtoken = await _localStorage.GetItemAsStringAsync("authToken");
			var identity = new ClaimsIdentity();
			_http.DefaultRequestHeaders.Authorization = null;

			if(!string.IsNullOrEmpty(authtoken))
			{
				try
				{
					identity = new ClaimsIdentity(ParseClaimsFromJwt(authtoken), "jwt");
					_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authtoken.Replace("\"", ""));
				}
				catch
				{
					await _localStorage.RemoveItemAsync("authToken");
					identity = new ClaimsIdentity();
				}
			}

			var user = new ClaimsPrincipal(identity);
			var state = new AuthenticationState(user);

			NotifyAuthenticationStateChanged(Task.FromResult(state));
			return state;
		}

		private byte[] ParseBase64WithoutPadding(string base64)
		{
			switch(base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}

		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			var payload = jwt.Split('.')[1];
			var jwtBytes = ParseBase64WithoutPadding(payload);
			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jwtBytes);

			var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

			return claims;
		}
	}
}
