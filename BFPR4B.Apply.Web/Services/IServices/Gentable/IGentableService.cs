namespace BFPR4B.Apply.Web.Services.IServices.Gentable
{
	public interface IGentableService
	{
		Task<T> GetGentableAsync<T>(string searchKey, string tablename, int parentcode, int subparentcode);
	}
}
