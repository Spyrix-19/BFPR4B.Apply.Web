using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BFPR4B.Apply.Web
{
	public class TokenValidationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IConfiguration configuration;

		private string _secretKey;
		private string _Audience;
		private string _Issuer;


		public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
		{
			_next = next;
			_secretKey = configuration.GetValue<string>("APISettings:SecretKey");
			_Audience = configuration.GetValue<string>("APISettings:Audience");
			_Issuer = configuration.GetValue<string>("APISettings:Issuer");
		}

		public async Task Invoke(HttpContext context)
		{
			// Check if the access token is valid and not expired here
			if (IsValidAccessToken(context))
			{
				await _next(context);
			}
			else
			{
				// Redirect to the login page if the token is invalid or expired
				context.Response.Redirect("/signin");
			}
		}

		private bool IsValidAccessToken(HttpContext context)
		{
			// Retrieve the access token from the request headers or cookies
			string accessToken = GetAccessTokenFromRequest(context);

			if (string.IsNullOrEmpty(accessToken))
			{
				// Token is missing or invalid, redirect to login
				return false;
			}

			// You may validate the token's expiration, issuer, and audience here
			bool isTokenValid = ValidateJwtToken(accessToken);

			return isTokenValid;
		}

		private string GetAccessTokenFromRequest(HttpContext context)
		{
			// Replace with your logic to extract the access token from the request.
			// You might find it in the Authorization header, a cookie, or a query parameter.
			string accessToken = context.Request.Headers["Authorization"];

			if (!string.IsNullOrEmpty(accessToken) && accessToken.StartsWith("Bearer "))
			{
				accessToken = accessToken.Substring("Bearer ".Length).Trim();
			}

			return accessToken;
		}

		private bool ValidateJwtToken(string accessToken)
		{
			if (string.IsNullOrWhiteSpace(accessToken) || !accessToken.StartsWith("Bearer "))
			{
				return false; // Invalid token format
			}

			var token = accessToken.Substring("Bearer ".Length);

			// Replace with your token validation logic using a JWT library like System.IdentityModel.Tokens.Jwt
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.UTF8.GetBytes(_secretKey); // Replace with your actual key

				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = _Issuer, // Replace with your issuer
					ValidateAudience = true,
					ValidAudience = _Audience, // Replace with your audience
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				}, out SecurityToken validatedToken);

				var jwtToken = (JwtSecurityToken)validatedToken;

				// Check additional claims or perform other validations as needed
				// For example, you can check a custom claim like user role
				var userRoleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
				if (userRoleClaim != null && userRoleClaim.Value == "admin")
				{
					// Token is valid and user has the "admin" role
					return true;
				}

				return false; // Token is valid but user does not have the required role
			}
			catch (Exception)
			{
				return false; // Token validation failed
			}
		}
	}
}
