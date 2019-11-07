using System;
using System.Web.Mvc;

namespace Lib.Web.Mvc.JqGridFork.DataAnnotations
{
	/// <summary>
	/// Specifies the excel export options
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class JqGridColumnExportAttribute : Attribute, IMetadataAware
	{

		#region Fields
		private JqGridExportOptions _exportOptions = new JqGridExportOptions();
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets use build in parses to detect the values and format it according to the formatCode definitions.
		/// </summary>
		public bool ExcelParser
		{
			get => _exportOptions.ExcelParser ?? false;
			set => _exportOptions.ExcelParser = value;
		}

		internal bool IsExcelFormatSetted { get; private set; }
		/// <summary>
		/// Gets or sets Excel format code
		/// </summary>
		public string ExcelFormat
		{
			get => _exportOptions.ExcelFormat;
			set => _exportOptions.ExcelFormat = value;
		}

		internal bool IsReplaceFormatSetted { get; private set; }
		/// <summary>
		/// Gets or sets name of function that can manipulate the value and make some replacement on it
		/// </summary>
		public string ReplaceFormat
		{
			get => _exportOptions.ReplaceFormat;
			set => _exportOptions.ReplaceFormat = value;
		}
		#endregion

		#region IMetadataAware
		/// <summary>
		/// Provides metadata to the model metadata creation process.
		/// </summary>
		/// <param name="metadata">The model metadata.</param>
		public void OnMetadataCreated(ModelMetadata metadata)
		{
			metadata.SetColumnExportOptions(_exportOptions);
		}
		#endregion
	}
}
