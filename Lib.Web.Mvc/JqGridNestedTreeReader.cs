namespace Lib.Web.Mvc.Kain.JqGrid
{
    /// <summary>
    /// JqGrid Tree Reader for Nested model
    /// </summary>
    public class JqGridNestedTreeReader : JqGridTreeReader
    {
        /// <summary>
        /// The rowid of the field to the left.
        /// </summary>
        public string LeftField { get; set; }
        /// <summary>
        /// The rowid of the field to the right
        /// </summary>
        public string RightField { get; set; }
    }
}