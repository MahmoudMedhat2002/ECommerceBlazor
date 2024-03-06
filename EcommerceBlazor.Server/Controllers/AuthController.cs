using EcommerceBlazor.Server.Services;
using EcommerceBlazor.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceBlazor.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
		public async Task<IActionResult> Register(UserRegister request)
		{
			var response = await _authService.Register(new User
			{
				Email = request.Email
			}, request.Password);

			if (!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login(UserLogin request)
		{
			var response = await _authService.Login(request.Email , request.Password);

			if(!response.Success)
			{
				return BadRequest(response);
			}

			return Ok(response);
		}
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var response = await _authService.ChangePassword(int.Parse(userId), newPassword);

			if (!response.Success)
			{
				return BadRequest(response);
			}
			
			return Ok(response);
		}
	}
}
