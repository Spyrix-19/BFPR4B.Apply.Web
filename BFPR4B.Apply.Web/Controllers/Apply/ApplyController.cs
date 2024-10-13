using BFPR4B.Apply.Web._keenthemes.libs;
using BFPR4B.Apply.Web.Services.IServices.Apply;
using BFPR4B.Apply.Web.Utility;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace BFPR4B.Apply.Web.Controllers.Apply
{
    public class ApplyController : Controller
    {
		private readonly IKTTheme _theme;
		private readonly IApplyService _applyService;
		private readonly IDataProtectionProvider _dataProtectionProvider;
		private readonly AccessTokenHelper _accessTokenHelper;

		public ApplyController(AccessTokenHelper accessTokenHelper, IDataProtectionProvider dataProtectionProvider, IApplyService applyService, IKTTheme theme)
		{
			_dataProtectionProvider = dataProtectionProvider;
			_accessTokenHelper = accessTokenHelper;
			_applyService = applyService;
			_theme = theme;
		}

		public IActionResult Index()
        {
			return View(_theme.GetPageView("Apply", "Index.cshtml"));
		}





    }
}
