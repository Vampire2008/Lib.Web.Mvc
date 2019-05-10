namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for form editing button icon.
	/// </summary>
	public class JqGridFormButtonIcon
	{
		#region Fields

		private string @class;
		private JqGridFormButtonIconPositions position = JqGridFormButtonIconPositions.Default;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the value indicating if icon is enabled.
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Gets or sets the icon position (relative to the text). If not sets to default, then Enabled is sets to true.
		/// </summary>
		public JqGridFormButtonIconPositions Position
		{
			get => position;
			set
			{
				position = value;
				if (position != JqGridFormButtonIconPositions.Default)
					Enabled = true;
			}
		}
		internal bool IsClassSetted { get; private set; }

		/// <summary>
		/// Gets or sets the icon class. Required set the Position not to Default.
		/// </summary>
		public string Class
		{
			get => @class;
			set
			{
				@class = value;
				IsClassSetted = true;
			}
		}

		#endregion
	}
}
