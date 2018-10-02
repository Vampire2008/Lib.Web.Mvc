namespace Lib.Web.Mvc.Kain.JqGrid
{
    /// <summary>
    /// Base class for classes which represents options for jqGrid Navigator controls
    /// </summary>
    public abstract class JqGridNavigatorControlOptions
    {
        #region Properties
        /// <summary>
        /// Gets or sets the value which defines if the jqGrid Navigator control added to the bottom pager should be coppied to the top pager.
        /// </summary>
        public bool CloneToTop { get; set; }

        /// <summary>
        /// Gets or sets the value which sets posistion of button
        /// </summary>
        public bool AddAfterInlineNavigator { get; set; }

        internal bool ProcessedElement { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the JqGridNavigatorControlOptions class.
        /// </summary>
        public JqGridNavigatorControlOptions()
        {
            CloneToTop = false;
            AddAfterInlineNavigator = false;
            ProcessedElement = false;
        }
        #endregion
    }
}
