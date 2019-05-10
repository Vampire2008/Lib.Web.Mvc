using System;
using Lib.Web.Mvc.JqGridFork.Constants;

namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents base options for jqGrid navigators.
	/// </summary>
	public abstract class JqGridNavigatorOptionsBase
	{

		#region Fields

		private string _addIcon;
		private string _addText;
		private string _addToolTip;
		private string _editIcon;
		private string _editText;
		private string _editToolTip;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or set the value which defines if add action is enabled (default true).
		/// </summary>
		public bool? Add { get; set; }

		internal bool IsAddIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for add action.
		/// </summary>
		public string AddIcon
		{
			get => _addIcon;
			set
			{
				_addIcon = value;
				IsAddIconSetted = true;
			}
		}

		internal bool IsAddTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for add action.
		/// </summary>
		public string AddText
		{
			get => _addText;
			set
			{
				_addText = value;
				IsAddTextSetted = true;
			}
		}

		internal bool IsAddToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for add action.
		/// </summary>
		public string AddToolTip
		{
			get => _addToolTip;
			set
			{
				_addToolTip = value;
				IsAddToolTipSetted = true;
			}
		}

		/// <summary>
		/// Gets or set the value which defines if edit action is enabled (default true).
		/// </summary>
		public bool? Edit { get; set; }

		internal bool IsEditIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for edit action.
		/// </summary>
		public string EditIcon
		{
			get => _editIcon;
			set
			{
				_editIcon = value;
				IsEditIconSetted = true;
			}
		}

		internal bool IsEditTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for edit action.
		/// </summary>
		public string EditText
		{
			get => _editText;
			set
			{
				_editText = value;
				IsEditTextSetted = true;
			}
		}

		internal bool IsEditToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for edit action.
		/// </summary>
		public string EditToolTip
		{
			get => _editToolTip;
			set
			{
				_editToolTip = value;
				IsEditToolTipSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the position of the Navigator buttons in the pager.
		/// </summary>
		public JqGridAlignments Position { get; set; } = JqGridAlignments.Default;
		#endregion
	}
}
