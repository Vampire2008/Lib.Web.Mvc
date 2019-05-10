namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Inline Navigator.
	/// </summary>
	public class JqGridInlineNavigatorOptions : JqGridNavigatorOptionsBase
	{
		#region Fields

		private string _saveIcon;
		private string _saveText;
		private string _saveToolTip;
		private string _cancelIcon;
		private string _cancelText;
		private string _cancelToolTip;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or set the value which defines if save action is enabled (default true).
		/// </summary>
		public bool? Save { get; set; }

		internal bool IsSaveIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for save action.
		/// </summary>
		public string SaveIcon
		{
			get => _saveIcon;
			set
			{
				_saveIcon = value;
				IsSaveIconSetted = true;
			}
		}

		internal bool IsSaveTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for save action.
		/// </summary>
		public string SaveText
		{
			get => _saveText;
			set
			{
				_saveText = value;
				IsSaveTextSetted = true;
			}
		}

		internal bool IsSaveToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for save action.
		/// </summary>
		public string SaveToolTip
		{
			get => _saveToolTip;
			set
			{
				_saveToolTip = value;
				IsSaveToolTipSetted = true;
			}
		}

		/// <summary>
		/// Gets or set the value which defines if cancel action is enabled (default true).
		/// </summary>
		public bool? Cancel { get; set; }

		internal bool IsCancelIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for cancel action.
		/// </summary>
		public string CancelIcon
		{
			get => _cancelIcon;
			set
			{
				_cancelIcon = value;
				IsCancelIconSetted = true;
			}
		}

		internal bool IsCancelTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for cancel action.
		/// </summary>
		public string CancelText
		{
			get => _cancelText;
			set
			{
				_cancelText = value;
				IsCancelTextSetted = true;
			}
		}

		internal bool IsCancelToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for cancel action.
		/// </summary>
		public string CancelToolTip
		{
			get => _cancelToolTip;
			set
			{
				_cancelToolTip = value;
				IsCancelToolTipSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the options for edit, save and cancel actions
		/// </summary>
		public JqGridInlineNavigatorActionOptions ActionOptions { get; set; }

		/// <summary>
		/// Gets or sets the options for add action.
		/// </summary>
		public JqGridInlineNavigatorAddActionOptions AddActionOptions { get; set; }
		#endregion
	}
}
