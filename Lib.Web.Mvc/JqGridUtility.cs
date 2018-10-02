using System.Data;
using System.Data.Linq;
using System.Web.Mvc;

namespace Lib.Web.Mvc.Kain.JqGrid
{
    internal static class JqGridUtility
    {
        #region Methods
        internal static bool IsValidForColumn(this ModelMetadata metadata)
        {
            return
                metadata.ShowForDisplay
                && metadata.ModelType != typeof(EntityState)
                && (!metadata.IsComplexType || metadata.ModelType == typeof(Binary) || metadata.ModelType == typeof(byte[]));
        }

        internal static string ToNullString(this string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "null" : s;
        }
        #endregion
    }
}
