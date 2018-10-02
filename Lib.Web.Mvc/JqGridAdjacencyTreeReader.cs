namespace Lib.Web.Mvc.Kain.JqGrid
{
    /// <summary>
    /// JqGrid Tree Reader for adjacency model
    /// </summary>
    public class JqGridAdjacencyTreeReader : JqGridTreeReader
    {
        /// <summary>
        /// This filed indicates if the record has a parent with an id of parent_id_field. If the parent id is NULL the element is a root element.
        /// </summary>
        public string ParentIdField { get; set; }
    }
}