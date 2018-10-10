using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Extensions
{
    internal static class DataRowExtensions
    {
        /// <summary>
        /// Convert SQL DataRow to <see cref="CustomDBTypeBase"/> via autoloader
        /// </summary>
        /// <typeparam name="T"><see cref="CustomDBTypeBase"/> return type</typeparam>
        /// <param name="row">SQL DataRow</param>
        /// <returns></returns>
        internal static T ToCustomDBType<T>(this DataRow row) where T : CustomDBTypeBase
        {
            if (row == null)
            {
                return null;
            }

            //TODO: TEST THIS
            /*/
            return (T)(object)row;
            /*/
            return (T)(dynamic)row;
            //*/
        }

        /// <summary>
        /// Check if a dataRow has a specific column value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        internal static bool HasColumn(this DataRow row, string column)
        {
            return row.Table.Columns.Contains(column);
        }

    }
}
