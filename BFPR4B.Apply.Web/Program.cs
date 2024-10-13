using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.IdentityModel.Tokens;
using BFPR4B.Apply.Web._keenthemes;
using BFPR4B.Apply.Web._keenthemes.libs;
using System.Text;
using BFPR4B.Apply.Web.Utility;
using BFPR4B.Apply.Web.Services.IServices.Gentable;
using BFPR4B.Apply.Web.Services.Services.Gentable;
using BFPR4B.Apply.Web.Services.IServices.Apply;
using BFPR4B.Apply.Web.Services.Services.Apply;

var builder = WebApplication.CreateBuilder(args);

//// Add logging configuration
//builder.ConfigureLogging((hostingContext, logging) =>
//{
//    logging.AddConsole(); // You can add other logging providers as needed
//    logging.AddDebug();
//    // Configure log levels and other settings as needed
//});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IKTTheme, KTTheme>();
builder.Services.AddScoped<AccessTokenHelper>();        // use for signin only
builder.Services.AddScoped<AccessTokenValidator>();  // use for entire controller
builder.Services.AddScoped<AccessTokenAuthorizationFilter>();
//builder.Services.AddScoped<IAuthorizationFilter, AccessTokenAuthorizationFilter>();

builder.Services.AddSingleton<IKTBootstrapBase, KTBootstrapBase>();

builder.Services.AddHttpClient<IApplyService, ApplyService>();
builder.Services.AddScoped<IApplyService, ApplyService>();

builder.Services.AddHttpClient<IGentableService, GentableService>();
builder.Services.AddScoped<IGentableService, GentableService>();


//builder.Services.AddSingleton<IAuthorizationHandler, ValidAccessTokenHandler>();

builder.Services.AddDistributedMemoryCache();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//}).AddCookie(options =>
//{
//    options.Cookie.HttpOnly = true;
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//    options.LoginPath = "/signin";
//    options.LogoutPath = "/logout";
//    //options.AccessDeniedPath = "/accessdenied";
//    options.SlidingExpiration = true;
//});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
	options.Cookie.Name = "AuthTokenCookie";
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
	options.LoginPath = "/signin";
	options.LogoutPath = "/logout";
	options.AccessDeniedPath = "/accessdenied";
	options.SlidingExpiration = true;
	options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
	options.Events = new CookieAuthenticationEvents
	{
		OnRedirectToLogout = context =>
		{
			// Clear other cookies or perform any additional actions before logout.
			// You can access HttpContext to clear cookies if needed.

			context.Response.Cookies.Delete("Access_Token"); // Replace with the name of other cookies you want to clear
			context.Response.Cookies.Delete("AuthTokenCookie"); // Replace with the name of other cookies you want to clear

			return Task.CompletedTask;
		}
	};
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["APISettings:Issuer"], // Replace with your issuer
		ValidateAudience = true,
		ValidAudience = builder.Configuration["APISettings:Audience"], // Replace with your audience
		ValidateLifetime = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["APISettings:SecretKey"])) // Replace with your key
	};
});

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(1); // Adjust as needed
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder =>
	{
		builder.WithOrigins("https://localhost:7060") // Allow requests from this origin
			   .AllowAnyHeader()
			   .AllowAnyMethod();
	});
});

builder.Services.AddHsts(options =>
{
	options.IncludeSubDomains = true;
	options.Preload = true;
	options.MaxAge = TimeSpan.FromMinutes(5);
	options.ExcludedHosts.Add("region4b.bfp.gov.ph");
	options.ExcludedHosts.Add("https://localhost:7060");
});

builder.Services.AddDataProtection()
	.SetApplicationName("BFPR4B.GAD.Web")
	.PersistKeysToFileSystem(new DirectoryInfo(@"c:\keys"))
	.SetDefaultKeyLifetime(TimeSpan.FromDays(14));



//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("ValidAccessToken", policy =>
//    {
//        policy.RequireAuthenticatedUser(); // Require authenticated users
//        policy.AddRequirements(new ValidAccessTokenRequirement());
//    });
//});

//builder.Services.AddSingleton<IAuthorizationHandler, ValidAccessTokenHandler>();

IConfiguration themeConfiguration = new ConfigurationBuilder()
							.AddJsonFile("_keenthemes/config/themesettings.json")
							.Build();

IConfiguration iconsConfiguration = new ConfigurationBuilder()
							.AddJsonFile("_keenthemes/config/icons.json")
							.Build();

KTThemeSettings.init(themeConfiguration);
KTIconsSettings.init(iconsConfiguration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/error");
	var options = new RewriteOptions().AddRedirectToHttps();
	app.UseRewriter(options);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// Place the middleware here
app.Use(async (context, next) => {

	await next(); // Invoke the next middleware

	//if (!context.User.Identity.IsAuthenticated)
	//{
	//    if (!context.Request.Path.StartsWithSegments("/Auth/signin"))
	//    {
	//        // Redirect to the login page if not authenticated and not already on the login page
	//        context.Response.Redirect("/Auth/signin");
	//        return;
	//    }
	//}

	if (context.Response.StatusCode == 404)
	{
		context.Request.Path = "/notfound";
		await next(); // Invoke the next middleware if the response status code is 404
	}

	if (context.Response.StatusCode == 401)
	{
		context.Request.Path = "/accessdenied";
		await next(); // Invoke the next middleware if the response status code is 404
	}
});

//app.UseMiddleware<ValidAccessTokenRequirement>();

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();

//app.UseMiddleware<TokenValidationMiddleware>();

app.UseAuthorization();


//app.UseMiddleware<ValidAccessTokenHandler>();

app.UseThemeMiddleware();

app.MapControllers();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Apply}/{action=Index}/{id?}");

app.Run();




