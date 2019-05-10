namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Navigator edit action.
	/// </summary>
	public class JqGridNavigatorEditActionOptions : JqGridNavigatorModifyActionOptions, IJqGridNavigatorPageableFormActionOptions
	{
		#region Fields

		private string topInfo;
		private string bottomInfo;
		private string afterClickPgButtons;
		private string afterComplete;
		private string beforeCheckValues;
		private string onClickPgButtons;
		private string onInitializeForm;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the options for keyboard navigation.
		/// </summary>
		public JqGridFormKeyboardNavigation NavigationKeys { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the pager buttons should appear on the form.
		/// </summary>
		public bool? ViewPagerButtons { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the form should be recreated every time the dialog is activeted with the new options from colModel (if they are changed).
		/// </summary>
		public bool? RecreateForm { get; set; }

		/// <summary>
		/// Gets or sets value indicating where the row just added should be placed.
		/// </summary>
		public JqGridNewRowPositions AddedRowPosition { get; set; } = JqGridNewRowPositions.Default;

		internal bool IsTopInfoSetted { get; private set; }
		/// <summary>
		/// Gets or sets the information which is placed just after the modal header as additional row.
		/// </summary>
		public string TopInfo
		{
			get => topInfo;
			set
			{
				topInfo = value;
				IsTopInfoSetted = true;
			}
		}

		internal bool IsBottomInfoSetted { get; private set; }
		/// <summary>
		/// Gets or sets the information which is placed just after the buttons of the form as additional row.
		/// </summary>
		public string BottomInfo
		{
			get => bottomInfo;
			set
			{
				bottomInfo = value;
				IsBottomInfoSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating if add action dialog should be cleared after submiting.
		/// </summary>
		public bool? ClearAfterAdd { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if add action dialog should be closed after submiting.
		/// </summary>
		public bool? CloseAfterAdd { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if edit action dialog should be closed after submiting.
		/// </summary>
		public bool? CloseAfterEdit { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the user should confirm the changes uppon saving or cancel them.
		/// </summary>
		public bool? CheckOnSubmit { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the user should be prompted when leaving unsaved changes.
		/// </summary>
		public bool? CheckOnUpdate { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the form can be saved by pressing a key.
		/// </summary>
		public bool? SaveKeyEnabled { get; set; }

		/// <summary>
		/// Gets or sets the key for saving.
		/// </summary>
		public char? SaveKey { get; set; }

		/// <summary>
		/// Gets or sets the icon for the save button.
		/// </summary>
		public JqGridFormButtonIcon SaveButtonIcon { get; set; }

		/// <summary>
		/// Gets or sets the icon for the close button.
		/// </summary>
		public JqGridFormButtonIcon CloseButtonIcon { get; set; }

		internal bool IsAfterClickPgButtonsSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after the data for the new row is loaded from the grid (if navigator buttons are enabled in edit mode).
		/// </summary>
		public string AfterClickPgButtons
		{
			get => afterClickPgButtons;
			set
			{
				afterClickPgButtons = value;
				IsAfterClickPgButtonsSetted = true;
			}
		}

		internal bool IsAfterCompleteSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised fires immediately after all actions and events are completed and the row is inserted or updated in the grid.
		/// </summary>
		public string AfterComplete
		{
			get => afterComplete;
			set
			{
				afterComplete = value;
				IsAfterCompleteSetted = true;
			}
		}

		internal bool IsBeforeCheckValuesSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised before checking the values (if checking is defined in colModel via editrules option).
		/// </summary>
		public string BeforeCheckValues
		{
			get => beforeCheckValues;
			set
			{
				beforeCheckValues = value;
				IsBeforeCheckValuesSetted = true;
			}
		}

		internal bool IsOnClickPgButtonsSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised immediately after the previous or next button is clicked, before leaving the current row (if navigator buttons are enabled in edit mode).
		/// </summary>
		public string OnClickPgButtons
		{
			get => onClickPgButtons;
			set
			{
				onClickPgButtons = value;
				IsOnClickPgButtonsSetted = true;
			}
		}

		internal bool IsOnInitializeFormSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised only once when creating the form for editing and adding.
		/// </summary>
		public string OnInitializeForm
		{
			get => onInitializeForm;
			set
			{
				onInitializeForm = value;
				IsOnInitializeFormSetted = true;
			}
		}
		#endregion
	}
}
