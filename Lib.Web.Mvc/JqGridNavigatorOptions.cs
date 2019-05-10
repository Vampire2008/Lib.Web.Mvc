using Lib.Web.Mvc.JqGridFork.Constants;

namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Navigator.
	/// </summary>
	public class JqGridNavigatorOptions : JqGridNavigatorOptionsBase
	{

		#region Fields

		private string _alertCaption;
		private string _alertText;
		private string _deleteIcon;
		private string _deleteText;
		private string _deleteToolTip;
		private string _refreshIcon;
		private string _refreshText;
		private string _refreshToolTip;
		private string _afterRefresh;
		private string _beforeRefresh;
		private string _searchIcon;
		private string _searchText;
		private string _searchToolTip;
		private string _viewIcon;
		private string _viewText;
		private string _viewToolTip;
		private string _addFunction;
		private string _editFunction;
		private string _deleteFunction;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the pager to which the Navigator should be attached to (jqGrid can have only one Navigator).
		/// </summary>
		public JqGridNavigatorPagers Pager { get; set; }

		internal bool IsAlertCaptionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the caption for warning which appears when user try to edit, delete or view a row without selecting it.
		/// </summary>
		public string AlertCaption
		{
			get => _alertCaption;
			set
			{
				_alertCaption = value;
				IsAlertCaptionSetted = true;
			}
		}

		internal bool IsAlertTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for warning which appears when user try to edit, delete or view a row without selecting it.
		/// </summary>
		public string AlertText
		{
			get => _alertText;
			set
			{
				_alertText = value;
				IsAlertTextSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the value which defines if all the actions from the bottom pager should be coppied to the top pager.
		/// </summary>
		public bool? CloneToTop { get; set; }

		/// <summary>
		/// Gets or sets the value which defines if the warning dialog can be closed with ESC key.
		/// </summary>
		public bool? CloseOnEscape { get; set; }

		/// <summary>
		/// Gets or set the value which defines if delete action is enabled (default true).
		/// </summary>
		public bool? Delete { get; set; }

		internal bool IsDeleteIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for delete action.
		/// </summary>
		public string DeleteIcon
		{
			get => _deleteIcon;
			set
			{
				_deleteIcon = value;
				IsDeleteIconSetted = true;
			}
		}

		internal bool IsDeleteTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for delete action.
		/// </summary>
		public string DeleteText
		{
			get => _deleteText;
			set
			{
				_deleteText = value;
				IsDeleteTextSetted = true;
			}
		}

		internal bool IsDeleteToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for delete action.
		/// </summary>
		public string DeleteToolTip
		{
			get => _deleteToolTip;
			set
			{
				_deleteToolTip = value;
				IsDeleteToolTipSetted = true;
			}
		}

		/// <summary>
		/// Gets or set the value which defines if refresh action is enabled (default true).
		/// </summary>
		public bool? Refresh { get; set; }

		internal bool IsRefreshIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for refresh action.
		/// </summary>
		public string RefreshIcon
		{
			get => _refreshIcon;
			set
			{
				_refreshIcon = value;
				IsRefreshIconSetted = true;
			}
		}

		internal bool IsRefreshTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for refresh action.
		/// </summary>
		public string RefreshText
		{
			get => _refreshText;
			set
			{
				_refreshText = value;
				IsRefreshTextSetted = true;
			}
		}

		internal bool IsRefreshToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for refresh action.
		/// </summary>
		public string RefreshToolTip
		{
			get => _refreshToolTip;
			set
			{
				_refreshToolTip = value;
				IsRefreshToolTipSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the mode for refresh action.
		/// </summary>
		public JqGridRefreshModes RefreshMode { get; set; } = JqGridRefreshModes.Default;

		internal bool IsAfterRefreshSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after the refresh button is clicked.
		/// </summary>
		public string AfterRefresh
		{
			get => _afterRefresh;
			set
			{
				_afterRefresh = value;
				IsAfterRefreshSetted = true;
			}
		}

		internal bool IsBeforeRefreshSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised before the refresh button is clicked.
		/// </summary>
		public string BeforeRefresh
		{
			get => _beforeRefresh;
			set
			{
				_beforeRefresh = value;
				IsBeforeRefreshSetted = true;
			}
		}

		/// <summary>
		/// Gets or set the value which defines if search action is enabled (default true).
		/// </summary>
		public bool? Search { get; set; }

		internal bool IsSearchIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for search action.
		/// </summary>
		public string SearchIcon
		{
			get => _searchIcon;
			set
			{
				_searchIcon = value;
				IsSearchIconSetted = true;
			}
		}

		internal bool IsSearchTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for search action.
		/// </summary>
		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				IsSearchTextSetted = true;
			}
		}

		internal bool IsSearchToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for search action.
		/// </summary>
		public string SearchToolTip
		{
			get => _searchToolTip;
			set
			{
				_searchToolTip = value;
				IsSearchToolTipSetted = true;
			}
		}

		/// <summary>
		/// Gets or set the value which defines if view action is enabled (default true).
		/// </summary>
		public bool? View { get; set; }

		internal bool IsViewIconSetted { get; private set; }
		/// <summary>
		/// Gets or sets the icon (form UI theme images) for view action.
		/// </summary>
		public string ViewIcon
		{
			get => _viewIcon;
			set
			{
				_viewIcon = value;
				IsViewIconSetted = true;
			}
		}

		internal bool IsViewTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for view action.
		/// </summary>
		public string ViewText
		{
			get => _viewText;
			set
			{
				_viewText = value;
				IsViewTextSetted = true;
			}
		}

		internal bool IsViewToolTipSetted { get; private set; }
		/// <summary>
		/// Gets or sets the tooltip for view action.
		/// </summary>
		public string ViewToolTip
		{
			get => _viewToolTip;
			set
			{
				_viewToolTip = value;
				IsViewToolTipSetted = true;
			}
		}

		internal bool IsAddFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the custom function to replace the build in add function.
		/// </summary>
		public string AddFunction
		{
			get => _addFunction;
			set
			{
				_addFunction = value;
				IsAddFunctionSetted = true;
			}
		}

		internal bool IsEditFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the custom function to replace the build in edit function.
		/// </summary>
		public string EditFunction
		{
			get => _editFunction;
			set
			{
				_editFunction = value;
				IsEditFunctionSetted = true;
			}
		}

		internal bool IsDeleteFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the custom function to replace the build in delete function.
		/// </summary>
		public string DeleteFunction
		{
			get => _deleteFunction;
			set
			{
				_deleteFunction = value;
				IsDeleteFunctionSetted = true;
			}
		}

		#endregion
	}
}
