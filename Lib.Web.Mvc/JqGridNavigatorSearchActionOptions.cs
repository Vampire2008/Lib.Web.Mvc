using System.Collections.Generic;

namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Navigator search action.
	/// </summary>
	public class JqGridNavigatorSearchActionOptions : JqGridNavigatorActionOptions
	{
		#region Fields

		private string afterRedraw;
		private string afterShowSearch;
		private string beforeShowSearch;
		private string caption;
		private string searchText;
		private string onInitializeSearch;
		private string onReset;
		private string onSearch;
		private string resetText;
		private string layer;

		#endregion

		#region Properties

		internal bool IsAfterRedrawSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised every time the filter is redrawed .
		/// </summary>
		public string AfterRedraw
		{
			get => afterRedraw;
			set
			{
				afterRedraw = value;
				IsAfterRedrawSetted = true;
			}
		}

		internal bool IsAfterShowSearchSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised every time after the search dialog is shown.
		/// </summary>
		public string AfterShowSearch
		{
			get => afterShowSearch;
			set
			{
				afterShowSearch = value;
				IsAfterShowSearchSetted = true;
			}
		}

		internal bool IsBeforeShowSearchSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised every time before the search dialog is shown.
		/// </summary>
		public string BeforeShowSearch
		{
			get => beforeShowSearch;
			set
			{
				beforeShowSearch = value;
				IsBeforeShowSearchSetted = true;
			}
		}

		internal bool IsCaptionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the caption for the dialog.
		/// </summary>
		public string Caption
		{
			get => caption;
			set
			{
				caption = value;
				IsCaptionSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating if search action dialog should be closed after searching.
		/// </summary>
		public bool? CloseAfterSearch { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if search action dialog should be closed after reseting.
		/// </summary>
		public bool? CloseAfterReset { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the SearchRules should be validated.
		/// </summary>
		public bool? ErrorCheck { get; set; }

		internal bool IsSearchTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for search button.
		/// </summary>
		public string SearchText
		{
			get => searchText;
			set
			{
				searchText = value;
				IsSearchTextSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating if advanced searching is enabled.
		/// </summary>
		public bool? AdvancedSearching { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if advanced searching with a possibilities to define a complex condfitions is enabled.
		/// </summary>
		public bool? AdvancedSearchingWithGroups { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if added row (in advanced searching) should be copied from previous row.
		/// </summary>
		public bool? CloneSearchRowOnAdd { get; set; }

		internal bool IsOnInitiazeSearchSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised once when the dialog is created.
		/// </summary>
		public string OnInitializeSearch
		{
			get => onInitializeSearch;
			set
			{
				onInitializeSearch = value;
				IsOnInitiazeSearchSetted = true;
			}
		}

		internal bool IsOnResetSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised when the reset button is activated.
		/// </summary>
		public string OnReset
		{
			get => onReset;
			set
			{
				onReset = value;
				IsOnResetSetted = true;
			}
		}

		internal bool IsOnSearchSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised when the search button is activated.
		/// </summary>
		public string OnSearch
		{
			get => onSearch;
			set
			{
				onSearch = value;
				IsOnSearchSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating if the entry filter should be destroyed unbinding all the events and then constructed again.
		/// </summary>
		public bool? RecreateFilter { get; set; }

		internal bool IsResetTextSetted { get; private set; }
		/// <summary>
		/// Gets or sets the text for reset button.
		/// </summary>
		public string ResetText
		{
			get => resetText;
			set
			{
				resetText = value;
				IsResetTextSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the available search operators for the searching (if not defined on column).
		/// </summary>
		public JqGridSearchOperators? SearchOperators { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the dialog should appear automatically when the grid is constructed for first time.
		/// </summary>
		public bool? ShowOnLoad { get; set; }

		/// <summary>
		/// Gets or sets the value indicating if the query which is generated when the user defines the conditions for the search should be shown.
		/// </summary>
		public bool? ShowQuery { get; set; }

		/// <summary>
		/// Gets or sets the filters templates for advanced searching and avanced searching with groups.
		/// </summary>
		public IDictionary<string, JqGridRequestSearchingFilters> Templates { get; set; }

		internal bool IsLayerSetted { get; private set; }
		/// <summary>
		/// Gets or sets the valid DOM id of the element into which the filter is inserted as child.
		/// </summary>
		public string Layer
		{
			get => layer;
			set
			{
				layer = value;
				IsLayerSetted = true;
			}
		}
		#endregion
	}
}
