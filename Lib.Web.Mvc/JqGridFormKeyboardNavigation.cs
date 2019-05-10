namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Class which represents options for form editing navigation keys.
	/// </summary>
	public class JqGridFormKeyboardNavigation
	{
		#region Fields

		private char? recordUp;

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the value indicating if keyboard navigation is enabled.
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Gets or sets the key for "record up". If set this it automatically sets Enabled to true.
		/// </summary>
		public char? RecordUp
		{
			get => recordUp;
			set
			{
				recordUp = value;
				Enabled = true;
			}
		}

		/// <summary>
		/// Gets or sets the key for "record down". Required to be set RecordUp not null
		/// </summary>
		public char? RecordDown { get; set; }
		#endregion
	}
}
