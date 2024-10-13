using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BFPR4B.Apply.Web.Utility
{
	public class AccessTokenHelper
	{
		private readonly IConfiguration _configuration;

		public AccessTokenHelper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GetAccessToken(HttpContext context)
		{
			return context.Session.GetString("Access_Token");
		}

		public bool IsValidToken(string token)
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				return false; // Invalid token format
			}

			// Replace with your token validation logic using a JWT library like System.IdentityModel.Tokens.Jwt
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.UTF8.GetBytes(_configuration["APISettings:SecretKey"]); // Replace with your actual key

				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = _configuration["APISettings:Issuer"], // Replace with your issuer
					ValidateAudience = true,
					ValidAudience = _configuration["APISettings:Audience"], // Replace with your audience
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				}, out SecurityToken validatedToken);

				var jwtToken = (JwtSecurityToken)validatedToken;

				//// Check additional claims or perform other validations as needed
				//// For example, you can check a custom claim like user role
				//var userRoleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
				//if (userRoleClaim != null && userRoleClaim.Value == "admin")
				//{
				//    // Token is valid and user has the "admin" role
				//    return true;
				//}

				return true; // Token is valid but user does not have the required role
			}
			catch (Exception)
			{
				return false; // Token validation failed
			}
		}
	}
}
