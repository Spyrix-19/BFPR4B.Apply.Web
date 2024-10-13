using BFPR4B.Apply.Web._keenthemes.libs;
using BFPR4B.Apply.Web.Models.Model.Gentable;
using BFPR4B.Apply.Web.Utility;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BFPR4B.Apply.Web.Models.System;
using System.Net;
using BFPR4B.Apply.Web.Services.IServices.Gentable;

namespace BFPR4B.Apply.Web.Controllers.Gentable
{
	public class GentableController : Controller
	{
		private readonly IKTTheme _theme;
		private readonly IGentableService _gentableService;
		private readonly IDataProtectionProvider _dataProtectionProvider;
		private readonly AccessTokenHelper _accessTokenHelper;

		public GentableController(AccessTokenHelper accessTokenHelper, IDataProtectionProvider dataProtectionProvider, IGentableService gentableService, IKTTheme theme)
		{
			_dataProtectionProvider = dataProtectionProvider;
			_accessTokenHelper = accessTokenHelper;
			_gentableService = gentableService;
			_theme = theme;
		}

		[HttpGet("/gentable")]
		public async Task<IActionResult> GetGentableAsync(string tablename)
		{
			try
			{
				List<GentableModel> list = new List<GentableModel>();


				var _response = await _gentableService.GetGentableAsync<APIResponse>("", tablename, 0, 0);

				if (_response != null && _response.IsSuccess)
				{
					list = JsonConvert.DeserializeObject<List<GentableModel>>(Convert.ToString(_response.Result));

					// Prepare the response object
					var response = new
					{
						data = list // Your list of UserModel
					};

					string json = JsonConvert.SerializeObject(response);

					return Content(json, "application/json");
				}

				return BadRequest(new { IsSuccess = false, ErrorMessages = Settings.API_ERR_MSG });
			}
			catch (Exception exception)
			{
				return Json(new { error = Convert.ToInt32(HttpStatusCode.InternalServerError), message = Settings.UNKNOWN_ERR_MSG });
			}
		}

	}
}
