﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Web.Mvc.JQuery.JqGrid
{
    /// <summary>
    /// Abstract class which represents TreeGrid record for jqGrid.
    /// </summary>
    public abstract class JqGridTreeRecord : JqGridRecord
    {
        #region Properties
        /// <summary>
        /// Gets or set the level of the record in the hierarchy.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets or sets value wich defines if the record is leaf (default false).
        /// </summary>
        public bool Leaf { get; set; }

        /// <summary>
        /// Gets or sets the value which defines whether this element should be expanded during the loading (default false).
        /// </summary>
        public bool Expanded { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the JqGridTreeRecord class.
        /// </summary>
        /// <param name="id">The record identifier.</param>
        /// <param name="values">The list of values for cells.</param>
        /// <param name="level">The level of the record in the hierarchy.</param>
        public JqGridTreeRecord(string id, List<object> values, int level)
            : base(id, values)
        {
            Level = level;
            Leaf = false;
            Expanded = false;
        }
        #endregion
    }
}
