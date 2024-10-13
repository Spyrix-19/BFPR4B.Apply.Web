using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AccessTokenAuthorizationFilter : IAuthorizationFilter
{
	private readonly IConfiguration _configuration;
	private readonly IDataProtectionProvider _dataProtectionProvider;

	public AccessTokenAuthorizationFilter(IConfiguration configuration, IDataProtectionProvider dataProtectionProvider)
	{
		_configuration = configuration;
		_dataProtectionProvider = dataProtectionProvider;
	}

	public void OnAuthorization(AuthorizationFilterContext context)
	{
		string protectedAccessToken = GetProtectedAccessTokenFromRequest(context.HttpContext);

		if (string.IsNullOrEmpty(protectedAccessToken))
		{
			// Token is missing or invalid, redirect to the login page
			context.Result = new RedirectResult("/signin");
			return;
		}

		// Unprotect the token
		string accessToken = UnprotectAccessToken(protectedAccessToken);

		if (!IsValidAccessToken(accessToken))
		{
			// Token is missing or invalid, redirect to the login page
			context.Result = new RedirectResult("/signin");
			return;
		}
	}

	private string GetProtectedAccessTokenFromRequest(HttpContext context)
	{
		// Implement your logic to extract the protected access token from the request.
		// You might find it in a cookie or elsewhere.
		string protectedAccessToken = context.Request.Cookies["Access_Token"];
		return protectedAccessToken;
	}

	private string UnprotectAccessToken(string protectedAccessToken)
	{
		// Unprotect the protected token using DataProtectionProvider
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
			var key = Encoding.UTF8.GetBytes(_configuration["APISettings:SecretKey"]); // Replace with your actual key

			tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _configuration["APISettings:Issuer"], // Replace with your issuer
				ValidateAudience = true,
				ValidAudience = _configuration["APISettings:Audience"], // Replace with your audience
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			}, out SecurityToken validatedToken);

			//// Optionally, you can access claims or perform additional validation here.
			//// For example, check for a specific claim like "role".
			//var jwtToken = (JwtSecurityToken)validatedToken;
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
