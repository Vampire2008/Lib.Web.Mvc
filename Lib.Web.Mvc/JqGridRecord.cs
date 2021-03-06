﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Lib.Web.Mvc.JqGridFork
{
    /// <summary>
    /// Class which represents record for jqGrid.
    /// </summary>
    public class JqGridRecord
    {
        #region Properties
        /// <summary>
        /// Gets the record cells values.
        /// </summary>
        public List<object> Values { get; }

        /// <summary>
        /// Gets the record value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        public string Id { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the JqGridRecord class.
        /// </summary>
        /// <param name="id">The record identifier.</param>
        /// <param name="values">The record cells values.</param>
        public JqGridRecord(string id, List<object> values)
        {
            Id = id;
            Values = values;
            Value = null;
        }

        /// <summary>
        /// Initializes a new instance of the JqGridRecord class.
        /// </summary>
        /// <param name="id">The record identifier.</param>
        /// <param name="value">The record values</param>
        public JqGridRecord(string id, object value)
        {
            Id = id;
            Value = value;
            Values = GetValuesAsList();
        }
        #endregion

        #region Methods
        protected internal virtual List<object> GetValuesAsList()
        {
            List<object> values = new List<object>();

            if (Value != null)
            {
                ModelMetadata jqGridModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, Value.GetType());
                IEnumerable<string> availableProperties = jqGridModelMetadata.Properties.Where(p => p.IsValidForColumn()).Select(p => p.PropertyName);
                foreach (PropertyDescriptor property in from PropertyDescriptor prop in TypeDescriptor.GetProperties(Value) where availableProperties.Contains(prop.Name) select prop)
                    values.Add(property.GetValue(Value));
            }

            return values;
        }

        protected internal virtual Dictionary<string, object> GetValuesAsDictionary()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            if (Value != null)
            {
                ModelMetadata jqGridModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, Value.GetType());
                IEnumerable<string> availableProperties = jqGridModelMetadata.Properties.Where(p => p.IsValidForColumn()).Select(p => p.PropertyName);
                foreach (PropertyDescriptor property in from PropertyDescriptor prop in TypeDescriptor.GetProperties(Value) where availableProperties.Contains(prop.Name) select prop)
                    values.Add(property.Name, property.GetValue(Value));
            }

            return values;
        }
        #endregion
    }
}
