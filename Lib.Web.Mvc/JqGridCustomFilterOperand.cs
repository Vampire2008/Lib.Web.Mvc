namespace Lib.Web.Mvc.JqGridFork
{
	/// <summary>
	/// Custom filter operand
	/// </summary>
	public class JqGridCustomFilterOperand
	{
		/// <summary>
		/// Gets or sets unique operation name
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets operand name
		/// </summary>
		public string Operand { get; set; }

		/// <summary>
		/// Gets or sets operand description
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets name of function that makes filtering
		/// </summary>
		public string Action { get; set; }
	}
}
