using static BFPR4B.Apply.Utility.SD;

namespace BFPR4B.Apply.Web.Models.System
{
	public class APIRequest
	{
		public ApiType ApiType { get; set; } = ApiType.GET;
		public string ApiUrl { get; set; }
		public object Data { get; set; }
		public string AccessToken { get; set; }
	}
}
