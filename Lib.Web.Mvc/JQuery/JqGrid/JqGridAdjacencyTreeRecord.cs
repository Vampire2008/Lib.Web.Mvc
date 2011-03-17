﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web.Mvc.JQuery.JqGrid
{
    /// <summary>
    /// Class which represents TreeGrid record for jqGrid in adjacency model.
    /// </summary>
    public class JqGridAdjacencyTreeRecord : JqGridTreeRecord
    {
        #region Properties
        /// <summary>
        /// Gets the id of parent of this record.
        /// </summary>
        public string ParentId { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the JqGridAdjacencyTreeRecord class.
        /// </summary>
        /// <param name="id">The record identifier.</param>
        /// <param name="values">The list of values for cells.</param>
        /// <param name="level">The level of the record in the hierarchy.</param>
        /// <param name="parentId">The id of parent of this record.</param>
        public JqGridAdjacencyTreeRecord(string id, List<object> values, int level, string parentId)
            : base(id, values, level)
        {
            ParentId = parentId;
        }
        #endregion
    }
}
