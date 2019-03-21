using Lib.Web.Mvc.JqGridFork.Constants;

namespace Lib.Web.Mvc.JqGridFork
{
    /// <summary>
    /// Class which represents options for jqGrid Navigator separator.
    /// </summary>
    public class JqGridNavigatorSeparatorOptions : JqGridNavigatorControlOptions
    {
        #region Properties
        /// <summary>
        /// Gets or sets the class for the separator.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the content for the separator.
        /// </summary>
        public string Content { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the JqGridNavigatorSeparatorOptions class.
        /// </summary>
        public JqGridNavigatorSeparatorOptions()
        {
            Class = JqGridNavigatorDefaults.SeparatorClass;
            Content = string.Empty;
        }
        #endregion
    }
}
