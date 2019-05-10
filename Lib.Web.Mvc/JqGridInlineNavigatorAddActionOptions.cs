using Lib.Web.Mvc.JqGridFork.Constants;

namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Inline Navigator add action.
	/// </summary>
	public class JqGridInlineNavigatorAddActionOptions
	{
		#region Fields

		private string rowId;
		private object initData;

		#endregion

		#region Properties

		internal bool IsRowIdSetted { get; private set; }
		/// <summary>
		/// Gets or sets the id for the new row
		/// </summary>
		public string RowId
		{
			get => rowId;
			set
			{
				rowId = value;
				IsRowIdSetted = true;
			}
		}

		internal bool IsInitDataSetted { get; private set; }
		/// <summary>
		/// Gets or sets the initial values for the new row.
		/// </summary>
		public object InitData
		{
			get => initData;
			set
			{
				initData = value;
				IsInitDataSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the new row position.
		/// </summary>
		public JqGridNewRowPositions Position { get; set; } = JqGridNewRowPositions.Default;

		/// <summary>
		/// Gets or sets the value which defines if the DefaultValue from ColumnsModel should be used.
		/// </summary>
		public bool? UseDefaultValues { get; set; }

		/// <summary>
		/// Gets or sets the value which defines if formatters should be used.
		/// </summary>
		public bool? UseFormatter { get; set; }

		/// <summary>
		/// Gets or sets edit options.
		/// </summary>
		public JqGridInlineNavigatorActionOptions Options { get; set; }
		#endregion
	}
}
