using System.Web.Mvc;

namespace Lib.Web.Mvc.JqGridFork
{
    /// <summary>
    /// jqGrid configuration container
    /// </summary>
    [ModelBinder(typeof(ModelBinders.JqGridJsonModelBinder))]
    public class JqGridConfiguration
    {
        #region Properties
        /// <summary>
        /// Options
        /// </summary>
        public JqGridOptions Settings { get; set; }
        #endregion
    }
}
