using System.ComponentModel.DataAnnotations;

namespace BFPR4B.Apply.Web.Models.Model.Gentable
{
	public class GentableModel
	{
		[Key]
		public int Detno { get; set; } = 0;
		public string Recordcode { get; set; } = "";
		public string Description { get; set; } = "";
		public int Parentcode { get; set; } = 0;
		public string ParentcodeName { get; set; } = "";
		public int Subparentcode { get; set; } = 0;
		public string SubparentcodeName { get; set; } = "";
		public string Tablename { get; set; } = "";
		public bool Required { get; set; } = false;
		public int Sortorder { get; set; } = 0;
		public string EncodedbyName { get; set; } = "";
		public int Createdby { get; set; } = 0;
		public DateTime Datecreated { get; set; } = Convert.ToDateTime("1/1/1900");
	}
}
