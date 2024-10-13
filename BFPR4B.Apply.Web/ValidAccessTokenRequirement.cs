using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class ValidAccessTokenRequirement : IAuthorizationRequirement
{
	public ValidAccessTokenRequirement()
	{
	}
}

public class ValidAccessTokenHandler : AuthorizationHandler<ValidAccessTokenRequirement>
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	private readonly string _secretKey;
	private readonly string _Issuer;
	private readonly string _Audience;

	public ValidAccessTokenHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
	{
		_httpContextAccessor = httpContextAccessor;
		////_secretKey = configuration.GetValue<string>("APISettings:SecretKey");
		////_Audience = configuration.GetValue<string>("APISettings:Audience");
		////_Issuer = configuration.GetValue<string>("APISettings:Issuer");
		///
		_secretKey = "ED9440DC-9E90-4468-A3CE-81560C89B93D-04E7A4B5-6C27-485E-9929-375400A884AB-A406029B-3D69-43BF-98E4-CC44A6049ACA-B4059FD7-4866-4361-9693-8980B3C6E536";
		_Audience = "https://localhost:7077/";
		_Issuer = "https://localhost:7077/";
	}

	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		ValidAccessTokenRequirement requirement)
	{
		// Get the access token from the claims
		var accessTokenClaim = context.User.FindFirst("Access_Token");

		if (accessTokenClaim != null)
		{
			var accessToken = accessTokenClaim.Value;

			// Example token validation logic
			if (IsValidToken(accessToken))
			{
				context.Succeed(requirement); // Token is valid
			}
		}

		return Task.CompletedTask;
	}

	// Example token validation logic (replace with your actual logic)
	private bool IsValidToken(string accessToken)
	{
		// Retrieve the access token from the request headers or cookies
		//string access_Token = GetAccessTokenFromRequest(context);

		if (string.IsNullOrEmpty(accessToken))
		{
			// Token is missing or invalid, redirect to login
			return false;
		}


		// You may validate the token's expiration, issuer, and audience here
		bool isTokenValid = ValidateJwtToken(accessToken);

		return isTokenValid;

		//string expectedToken = "your_expected_token_value"; // Replace with your valid token value

		//return accessToken == expectedToken; // Compare the token
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
			var key = Encoding.ASCII.GetBytes(_secretKey); // Replace with your actual key

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
