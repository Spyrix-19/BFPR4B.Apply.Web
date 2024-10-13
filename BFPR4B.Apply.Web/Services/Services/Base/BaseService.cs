using AutoMapper.Internal;
using BFPR4B.Apply.Web.Models.System;
using System.Net.Http.Headers;
using System.Text;
using BFPR4B.Apply.Utility;
using Newtonsoft.Json;

namespace BFPR4B.Apply.Web.Services.Services.Base
{
	public class BaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public BaseService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<T> SendAsync<T>(APIRequest apiRequest)
		{
			try
			{
				var client = _httpClientFactory.CreateClient("R4BMimaropa.API");
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


				if (!string.IsNullOrEmpty(apiRequest.AccessToken))
				{
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
				}


				var requestMessage = new HttpRequestMessage
				{
					RequestUri = new Uri(apiRequest.ApiUrl),
					Method = HttpMethod.Get // Default to GET
				};

				switch (apiRequest.ApiType)
				{
					case SD.ApiType.POST:
						requestMessage.Method = HttpMethod.Post;
						break;
					case SD.ApiType.PUT:
						requestMessage.Method = HttpMethod.Put;
						break;
					case SD.ApiType.DELETE:
						requestMessage.Method = HttpMethod.Delete;
						break;
					default:
						requestMessage.Method = HttpMethod.Get;
						break;
				}

				if (apiRequest.Data != null)
				{
					var jsonData = JsonConvert.SerializeObject(apiRequest.Data);
					requestMessage.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
				}

				var apiResponse = await client.SendAsync(requestMessage);

				if (apiResponse.IsSuccessStatusCode)
				{
					var apiContent = await apiResponse.Content.ReadAsStringAsync();
					var result = JsonConvert.DeserializeObject<T>(apiContent);
					return result;
				}
				else
				{
					var errorDto = new APIResponse
					{
						ErrorMessages = $"HTTP Error: {apiResponse.RequestMessage}",
						IsSuccess = false
					};
					return (T)(object)errorDto;
				}
			}
			catch (Exception ex)
			{
				var errorDto = new APIResponse
				{
					ErrorMessages = ex.Message,
					IsSuccess = false
				};
				return (T)(object)errorDto;
			}
		}
	}
}
