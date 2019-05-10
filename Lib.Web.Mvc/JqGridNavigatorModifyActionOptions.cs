namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Navigator form editing add/edit or delete action.
	/// </summary>
	public abstract class JqGridNavigatorModifyActionOptions : JqGridNavigatorFormActionOptions
	{
		#region Fields

		private string url;
		private object extraData;
		private string extraDataScript;
		private object ajaxOptions;
		private string afterShowForm;
		private string afterSubmit;
		private string beforeSubmit;
		private string onClickSubmit;
		private string serializeData;
		private string errorTextFormat;

		#endregion

		#region Properties

		internal bool IsUrlSetted { get; private set; }
		/// <summary>
		/// Gets or sets the url for action requests.
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

		/// <summary>
		/// Gets or sets the type of request to make when data is sent to the server.
		/// </summary>
		public JqGridMethodTypes MethodType { get; set; } = JqGridMethodTypes.Default;

		/// <summary>
		/// Gets or sets the value indicating if grid should be reloaded after submiting.
		/// </summary>
		public bool? ReloadAfterSubmit { get; set; }

		internal bool IsExtraDataSetted { get; private set; }
		/// <summary>
		/// Gets or sets an object used to add content to the data posted to the server.
		/// </summary>
		public object ExtraData
		{
			get => extraData;
			set
			{
				extraData = value;
				IsExtraDataSetted = true;
			}
		}

		internal bool IsExtraDataScriptSetted { get; private set; }
		/// <summary>
		/// Gets or sets the JavaScript which will dynamically generate the object which will be used to add content to the data posted to the server (this property takes precedence over ExtraData).
		/// </summary>
		public string ExtraDataScript
		{
			get => extraDataScript;
			set
			{
				extraDataScript = value;
				IsExtraDataScriptSetted = true;
			}
		}

		internal bool IsAjaxOptionSetted { get; private set; }
		/// <summary>
		/// Gets or sets ajax settings for the request.
		/// </summary>
		public object AjaxOptions
		{
			get => ajaxOptions;
			set
			{
				ajaxOptions = value;
				IsAjaxOptionSetted = true;
			}
		}

		internal bool IsAfterShowFormSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after showing the form.
		/// </summary>
		public string AfterShowForm
		{
			get => afterShowForm;
			set
			{
				afterShowForm = value;
				IsAfterShowFormSetted = true;
			}
		}

		internal bool IsAfterSubmitSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after response has been received from server.
		/// </summary>
		public string AfterSubmit
		{
			get => afterSubmit;
			set
			{
				afterSubmit = value;
				IsAfterSubmitSetted = true;
			}
		}

		internal bool IsBeforeSubmitSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised before the data is submitted to the server.
		/// </summary>
		public string BeforeSubmit
		{
			get => beforeSubmit;
			set
			{
				beforeSubmit = value;
				IsBeforeSubmitSetted = true;
			}
		}

		internal bool IsOnClickSubmitSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised after the submit button is clicked and the postdata is constructed.
		/// </summary>
		public string OnClickSubmit
		{
			get => onClickSubmit;
			set
			{
				onClickSubmit = value;
				IsOnClickSubmitSetted = true;
			}
		}

		internal bool IsSerializeDataSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which can serialize the data passed to the ajax request from the form.
		/// </summary>
		public string SerializeData
		{
			get => serializeData;
			set
			{
				serializeData = value;
				IsSerializeDataSetted = true;
			}
		}

		internal bool IsErrorTextFormatSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised when error occurs from the ajax call and can be used for better formatting of the error messages.
		/// </summary>
		public string ErrorTextFormat
		{
			get => errorTextFormat;
			set
			{
				errorTextFormat = value;
				IsErrorTextFormatSetted = true;
			}
		}
		#endregion
	}
}
