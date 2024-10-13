using System.Net;

namespace BFPR4B.Apply.Web.Models.System
{
	public class APIResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; }
		public string ErrorMessages { get; set; }
		public object Result { get; set; }
	}
}
