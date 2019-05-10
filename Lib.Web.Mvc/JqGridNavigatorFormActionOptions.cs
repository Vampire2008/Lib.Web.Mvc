namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for jqGrid Navigator form editing action.
	/// </summary>
	public abstract class JqGridNavigatorFormActionOptions : JqGridNavigatorActionOptions
	{
		#region Fields

		private string beforeInitData;
		private string beforeShowForm;

		#endregion

		#region Properties

		internal bool IsBeforeInitDataSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised before initializing the new form data.
		/// </summary>
		public string BeforeInitData
		{
			get => beforeInitData; set
			{
				beforeInitData = value;
				IsBeforeInitDataSetted = true;
			}
		}

		internal bool IsBeforShowFormSetted { get; private set; }
		/// <summary>
		/// Gets or sets the function for event which is raised before showing the form with the new data.
		/// </summary>
		public string BeforeShowForm
		{
			get => beforeShowForm;
			set
			{
				beforeShowForm = value;
				IsBeforShowFormSetted = true;
			}
		}
		#endregion
	}
}
