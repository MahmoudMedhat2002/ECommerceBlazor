using EcommerceBlazor.Server.Models;
using EcommerceBlazor.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace EcommerceBlazor.Server.Services
{
	public class AuthService : IAuthService
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _config;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(AppDbContext context , IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
			_config = config;
			_httpContextAccessor = httpContextAccessor;
		}

		public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

		public async Task<ServiceResponse<string>> Login(string email, string password)
		{
			var response = new ServiceResponse<string>();

			var user = await _context.Users.FirstOrDefaultAsync(user => user.Email.ToLower().Equals(email.ToLower()));

			if(user == null || !VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
			{
				response.Success = false;
				response.Message = "Invalid Credentials";
			}
			else
			{
				response.Data = CreateToken(user);
			}

			return response;
		}

		public async Task<ServiceResponse<int>> Register(User user, string password)
		{
			if(await UserExists(user.Email))
			{
				return new ServiceResponse<int>
				{
					Success = false,
					Message = "User already exists"
				};
			}

			CreatePassword(password , out byte[] passwordHash , out byte[] passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();

			return new ServiceResponse<int>
			{
				Data = user.Id,
				Message = "Registeration Complete"
			};


		}

		public async Task<bool> UserExists(string email)
		{
			if(await _context.Users.AnyAsync(user => user.Email.ToLower().Equals(email.ToLower())))
			{
				return true;
			}
			return false;
		}

		private void CreatePassword(string password , out byte[] passwordHash , out byte[] passwordSalt)
		{
			using(var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		private bool VerifyPassword(string password , byte[] passwordHash, byte[] passwordSalt)
		{
			byte[] computeHash;
			using(var hmac = new HMACSHA512(passwordSalt))
			{
				computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
			return passwordHash.SequenceEqual(computeHash);
		}

		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
				new Claim (ClaimTypes.Name, user.Email),
				new Claim (ClaimTypes.Role, user.Role)
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken
			(
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: creds
			);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}

		public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
		{
			var user = await _context.Users.FindAsync(userId);

			if(user == null)
			{
				return new ServiceResponse<bool>
				{
					Success = false,
					Message = "User not found."
				};
			}

			CreatePassword(newPassword, out byte[] PasswordHash, out byte[] PasswordSalt);

			user.PasswordHash = PasswordHash;
			user.PasswordSalt = PasswordSalt;

			await _context.SaveChangesAsync();

			return new ServiceResponse<bool>
			{
				Data = true,
				Message = "Password has changed Successfully"
			};
		}

		public string GetUserEmail()
		{
			return  _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
		}

		public async Task<User> GetUserByEmail(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
		}
	}
}
