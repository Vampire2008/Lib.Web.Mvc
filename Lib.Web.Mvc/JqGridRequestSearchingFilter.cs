namespace Lib.Web.Mvc.JqGridFork
{
    /// <summary>
    /// Class which represents filter in request from jqGrid.
    /// </summary>
    public class JqGridRequestSearchingFilter
    {
        #region Properties
        /// <summary>
        /// Gets the searching column name.
        /// </summary>
        public string SearchingName { get; set; }

        /// <summary>
        /// Gets the searching value.
        /// </summary>
        public string SearchingValue { get; set; }

        /// <summary>
        /// Gets the searching operator.
        /// </summary>
        public JqGridSearchOperators SearchingOperator { get; set; }
        #endregion
    }
}
