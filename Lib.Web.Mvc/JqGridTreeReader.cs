namespace Lib.Web.Mvc.JqGridFork
{
    public abstract class JqGridTreeReader
    {
        /// <summary>
        /// This field determines the level in the hierarchy of the element.
        /// </summary>
        public string LevelField { get; set; }
        /// <summary>
        /// This field should tell the grid that the element is leaf.
        /// </summary>
        public string LeafField { get; set; }
        /// <summary>
        /// This field tells the grid whether this element should be expanded during the loading (true or false).
        /// </summary>
        public string ExpandedField { get; set; }
        /// <summary>
        /// This field is optional and indicates if the node and its children are loaded.
        /// </summary>
        public string Loaded { get; set; }
        /// <summary>
        /// This field is optional and if set replaces icon for the leaf field. The content should be a valid icon name from the used css framework.
        /// </summary>
        public string IconField { get; set; }

    }
}