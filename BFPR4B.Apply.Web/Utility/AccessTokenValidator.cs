using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BFPR4B.Apply.Web.Utility
{
	public class AccessTokenValidator
	{
		private readonly IDataProtectionProvider _dataProtectionProvider;
		private readonly IConfiguration _configuration;

		public AccessTokenValidator(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
		{
			_dataProtectionProvider = dataProtectionProvider;
			_configuration = configuration;
		}

		public (bool, string) ValidateAccessToken(HttpContext context)
		{
			string protectedAccessToken = GetProtectedAccessTokenFromRequest(context);

			if (string.IsNullOrEmpty(protectedAccessToken))
			{
				// Token is missing or invalid, redirect to the login page
				return (false, "");
			}

			string accessToken = UnprotectAccessToken(protectedAccessToken);

			if (!IsValidAccessToken(accessToken))
			{
				// Token is missing or invalid, redirect to the login page
				return (false, "");
			}

			return (true, accessToken);
		}

		private string GetProtectedAccessTokenFromRequest(HttpContext context)
		{
			string protectedAccessToken = context.Request.Cookies["Access_Token"];
			return protectedAccessToken;
		}

		private string UnprotectAccessToken(string protectedAccessToken)
		{
			var protector = _dataProtectionProvider.CreateProtector("AccessTokenProtection");
			string accessToken = protector.Unprotect(protectedAccessToken);
			return accessToken;
		}

		private bool IsValidAccessToken(string accessToken)
		{
			if (string.IsNullOrWhiteSpace(accessToken))
			{
				return false; // Invalid token format
			}

			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.UTF8.GetBytes(_configuration["APISettings:SecretKey"]);

				tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = _configuration["APISettings:Issuer"],
					ValidateAudience = true,
					ValidAudience = _configuration["APISettings:Audience"],
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				}, out _);

				return true;
			}
			catch (Exception)
			{
				return false; // Token validation failed
			}
		}
	}
}
