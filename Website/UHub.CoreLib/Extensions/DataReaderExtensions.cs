using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Extensions
{
    internal static class DataReaderExtensions
    {
        /// <summary>
        /// Convert SQL DataRow to <see cref="DBEntityBase"/> via autoloader
        /// </summary>
        /// <typeparam name="T"><see cref="DBEntityBase"/> return type</typeparam>
        /// <param name="row">SQL DataRow</param>
        /// <returns></returns>
        internal static T ToCustomDBType<T>(this SqlDataReader reader) where T : DBEntityBase
        {
            if (reader == null)
            {
                return null;
            }

            //TODO: TEST THIS
            /*/
            return (T)(object)row;
            /*/
            return (T)(dynamic)reader;
            //*/
        }

        /// <summary>
        /// Check if a dataRow has a specific column value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        internal static bool HasColumn(this DataRow reader, string column)
        {
            return reader.Table.Columns.Contains(column);
        }

    }
}
