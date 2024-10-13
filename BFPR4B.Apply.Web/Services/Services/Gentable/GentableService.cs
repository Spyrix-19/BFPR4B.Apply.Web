using BFPR4B.Apply.Utility;
using BFPR4B.Apply.Web.Models.System;
using BFPR4B.Apply.Web.Services.IServices.Gentable;
using BFPR4B.Apply.Web.Services.Services.Base;

namespace BFPR4B.Apply.Web.Services.Services.Gentable
{
	public class GentableService : BaseService, IGentableService
	{
		private readonly IHttpClientFactory _clientFactory;
		private string _apiURL;

		public GentableService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
		{
			_clientFactory = clientFactory;
			_apiURL = configuration.GetValue<string>("ServiceUrls:BFPR4B.Apply.API");
		}

		public async Task<T> GetGentableAsync<T>(string searchKey, string tablename, int parentcode, int subparentcode)
		{

			// Build the API URL using UriBuilder
			var uriBuilder = new UriBuilder(_apiURL + "/api/v1/Gentable/Ledger")
			{
				Query = $"searchkey={Uri.EscapeDataString(searchKey)}&tablename={Uri.EscapeDataString(tablename)}&parentcode={parentcode}&subparentcode={subparentcode}"
			};

			// Log the request URL (optional)
			Console.WriteLine($"Requesting: {uriBuilder.ToString()}");

			// Send the API request
			return await SendAsync<T>(new APIRequest
			{
				ApiType = SD.ApiType.GET,
				ApiUrl = uriBuilder.ToString(),
				AccessToken = null // Consider adding access token if needed
			});
		}
	}
}
