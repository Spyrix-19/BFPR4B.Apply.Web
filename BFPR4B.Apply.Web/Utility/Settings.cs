namespace BFPR4B.Apply.Web.Utility
{
	public class Settings
	{
		public static int N_SESSIONWARN = 10; //Session WARN in minutes
		public static int N_SESSIONEXPIRE = 120; //Session EXPIRE in minutes

		public static string INVALID_SESSION_ERR_MSG = "Your session has expired for your security.";

		/// <summary>
		/// This is the default error message if model validation is invalid.
		/// </summary> 
		public static string INVALID_MODEL_ERR_MSG = "Please check invalid fields.";

		/// <summary>
		/// This is the default error message if API error cannot be determined by the system.
		/// </summary>
		public static string API_ERR_MSG = "API error. Please try again.";

		/// <summary>
		/// This is the default error message if error cannot be determined by the system.
		/// </summary>
		public static string UNKNOWN_ERR_MSG = "Something went wrong. Please try again.";

		/// <summary>
		/// This is the default error message if user has no user access to certain features.
		/// </summary>
		public static string USER_ACCESS_ERR_MSG = "Your user accessibility does not allow you to access this feature. Please consult MIS/IT for assistance.";
	}
}
