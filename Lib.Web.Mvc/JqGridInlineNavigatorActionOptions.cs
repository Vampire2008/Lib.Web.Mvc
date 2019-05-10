namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Inline Navigator action.
	/// </summary>
	public class JqGridInlineNavigatorActionOptions
	{

		#region Fields

		private string onEditFunction;
		private string successFunction;
		private string url;
		private object extraParam;
		private string extraParamScript;
		private string afterSaveFunction;
		private string errorFunction;
		private string afterRestoreFunction;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the value indicating if user can use [Enter] key to save and [Esc] key to cancel.
		/// </summary>
		public bool? Keys { get; set; }

		internal bool IsOnEditFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after successfully accessing the row for editing.
		/// </summary>
		public string OnEditFunction
		{
			get => onEditFunction;
			set
			{
				onEditFunction = value;
				IsOnEditFunctionSetted = true;
			}
		}

		internal bool IsSuccessFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised immediately after the successful request to the server.
		/// </summary>
		public string SuccessFunction
		{
			get => successFunction;
			set
			{
				successFunction = value;
				IsSuccessFunctionSetted = true;
			}
		}

		internal bool IsUrlSetted { get; private set; }
		/// <summary>
		/// Gets or sets URL for the request (replaces the EditingUrl from JqGridOptions).
		/// </summary>
		public string Url
		{
			get => url;
			set
			{
				url = value;
				IsUrlSetted = true;
			}
		}

		internal bool IsExtraParamSetted { get; private set; }
		/// <summary>
		/// Gets or sets the extra values that will be posted with row values to the server.
		/// </summary>
		public object ExtraParam
		{
			get => extraParam;
			set
			{
				extraParam = value;
				IsExtraParamSetted = true;
			}
		}

		internal bool IsExtraParamScriptSetted { get; private set; }
		/// <summary>
		/// Gets or sets the JavaScript which will dynamically generate the extra values that will be posted with row values to the server (this property takes precedence over ExtraParam).
		/// </summary>
		public string ExtraParamScript
		{
			get => extraParamScript;
			set
			{
				extraParamScript = value;
				IsExtraParamScriptSetted = true;
			}
		}

		internal bool IsAfterSaveFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after the data is saved to the server.
		/// </summary>
		public string AfterSaveFunction
		{
			get => afterSaveFunction;
			set
			{
				afterSaveFunction = value;
				IsAfterSaveFunctionSetted = true;
			}
		}

		internal bool IsErrorFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised when error occurs during saving to the server.
		/// </summary>
		public string ErrorFunction
		{
			get => errorFunction;
			set
			{
				errorFunction = value;
				IsErrorFunctionSetted = true;
			}
		}

		internal bool IsAfterRestoreFunctionSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after restoring the row.
		/// </summary>
		public string AfterRestoreFunction
		{
			get => afterRestoreFunction;
			set
			{
				afterRestoreFunction = value;
				IsAfterRestoreFunctionSetted = true;
			}
		}

		/// <summary>
		/// Gets or sets the value indicating if the row should be restored in case of error while saving to the server.
		/// </summary>
		public bool? RestoreAfterError { get; set; }

		/// <summary>
		/// Gets or sets the type or request to make when data is sent to the server.
		/// </summary>
		public JqGridMethodTypes MethodType { get; set; } = JqGridMethodTypes.Default;
		#endregion
	}
}
