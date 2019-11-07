namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Excel export options
	/// </summary>
	public class JqGridExportOptions
	{

		#region Fields
		private string _excelFormat;
		private string _replaceFormat;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets use build in parses to detect the values and format it according to the formatCode definitions.
		/// </summary>
		public bool? ExcelParser { get; set; }

		internal bool IsExcelFormatSetted { get; private set; }
		/// <summary>
		/// Gets or sets Excel format code
		/// </summary>
		public string ExcelFormat
		{
			get => _excelFormat;
			set
			{
				_excelFormat = value;
				IsExcelFormatSetted = true;
			}
		}

		internal bool IsReplaceFormatSetted { get; private set; }
		/// <summary>
		/// Gets or sets name of function that can manipulate the value and make some replacement on it
		/// </summary>
		public string ReplaceFormat
		{
			get => _replaceFormat;
			set
			{
				_replaceFormat = value;
				IsReplaceFormatSetted = true;
			}
		}
		#endregion
	}
}
