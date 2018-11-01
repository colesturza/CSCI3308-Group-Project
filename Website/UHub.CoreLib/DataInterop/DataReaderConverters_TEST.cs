using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.ClubModerators;
using UHub.CoreLib.ErrorHandling.Exceptions;

namespace UHub.CoreLib.DataInterop
{
    /// <summary>
    /// Alternate method of converting set of data records to objects. Not faster than original method 
    /// </summary>
    internal class DataReaderConverters
    {

        private static IEnumerable<UHub.CoreLib.Entities.Posts.Post> ConvertToPost(SqlDataReader reader)
        {
            var columnSet = reader.GetSchemaTable().Columns;
            var ID_Idx = reader.GetOrdinal("ID");
            var IsEnabled_Idx = reader.GetOrdinal("IsEnabled");
            var IsReadOnly_Idx = reader.GetOrdinal("IsReadOnly");
            var Name_Idx = reader.GetOrdinal("Name");
            var Content_Idx = reader.GetOrdinal("Content");
            var IsModified_Idx = reader.GetOrdinal("IsModified");
            var ViewCount_Idx = reader.GetOrdinal("ViewCount");
            var IsLocked_Idx = reader.GetOrdinal("IsLocked");
            var CanComment_Idx = reader.GetOrdinal("CanComment");
            var IsPublic_Idx = reader.GetOrdinal("IsPublic");
            var ParentID_Idx = reader.GetOrdinal("ParentID");
            var IsDeleted_Idx = reader.GetOrdinal("IsDeleted");
            var CreatedBy_Idx = reader.GetOrdinal("CreatedBy");
            var CreatedDate_Idx = reader.GetOrdinal("CreatedDate");
            var ModifiedBy_Idx = reader.GetOrdinal("ModifiedBy");
            var ModifiedDate_Idx = reader.GetOrdinal("ModifiedDate");




            while (reader.Read())
            {
                var obj = new UHub.CoreLib.Entities.Posts.Post();


                if (reader.GetValue(ID_Idx) != DBNull.Value)
                {
                    obj.ID = reader.GetFieldValue<System.Int64>(ID_Idx);
                }
                else
                {
                    obj.ID = null;
                }
                obj.IsEnabled = reader.GetFieldValue<System.Boolean>(IsEnabled_Idx);
                obj.IsReadOnly = reader.GetFieldValue<System.Boolean>(IsReadOnly_Idx);
                obj.Name = reader.GetFieldValue<System.String>(Name_Idx);
                obj.Content = reader.GetFieldValue<System.String>(Content_Idx);
                obj.IsModified = reader.GetFieldValue<System.Boolean>(IsModified_Idx);
                obj.ViewCount = reader.GetFieldValue<System.Int64>(ViewCount_Idx);
                obj.IsLocked = reader.GetFieldValue<System.Boolean>(IsLocked_Idx);
                obj.CanComment = reader.GetFieldValue<System.Boolean>(CanComment_Idx);
                obj.IsPublic = reader.GetFieldValue<System.Boolean>(IsPublic_Idx);
                obj.ParentID = reader.GetFieldValue<System.Int64>(ParentID_Idx);
                obj.IsDeleted = reader.GetFieldValue<System.Boolean>(IsDeleted_Idx);
                obj.CreatedBy = reader.GetFieldValue<System.Int64>(CreatedBy_Idx);
                obj.CreatedDate = reader.GetFieldValue<System.DateTimeOffset>(CreatedDate_Idx);
                if (reader.GetValue(ModifiedBy_Idx) != DBNull.Value)
                {
                    reader.GetFieldValue<System.Int64>(ModifiedBy_Idx);
                }
                else
                {
                    obj.ModifiedBy = null;
                }
                if (reader.GetValue(ModifiedDate_Idx) != DBNull.Value)
                {
                    obj.ModifiedDate = reader.GetFieldValue<System.DateTimeOffset>(ModifiedDate_Idx);
                }
                else
                {
                    obj.ModifiedDate = null;
                }


                yield return obj;
            }


        }


        public static List<T> ExecBasicQuery2<T>(string SqlConn, string CmdName, Action<SqlCommand> SetParams, Func<SqlDataReader, IEnumerable<T>> ReturnValParseFunc)
        {

            using (var conn = new SqlConnection(SqlConn))
            {
                conn.Open();
                using (var cmd = new SqlCommand(CmdName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    SetParams?.Invoke(cmd);


                    var hasError = false;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = cmd.ExecuteReader();
                    }
                    catch (SystemDisabledException)
                    {
                        hasError = true;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        hasError = true;
                        var errCode = ex.Message.Substring(0, 4);
                        var errMsg = ex.Message.Substring(ex.Message.IndexOf(": ") + 2);


                        if (errCode == "400:")
                        {
                            //general argument exception
                            if (errMsg.Contains("Invalid request arguments"))
                                throw new ArgumentException(errMsg);
                            //specific argument does not meet requirements
                            if (Regex.IsMatch(errMsg, "^Invalid [a-zA-Z ]+ argument$"))
                                throw new ArgumentOutOfRangeException("", errMsg);
                            //argument is null/empty
                            else if (errMsg.Contains("cannot be null or empty"))
                                throw new ArgumentNullException("", errMsg);
                            //argument casting error
                            else if (errMsg.Contains("value cannot be converted"))
                                throw new InvalidDBCastException(errMsg);
                            //general
                            else
                                throw new InvalidOperationException(errMsg);
                        }
                        //invalid authentication/authorization
                        else if (errCode == "403:")
                        {
                            throw new AccessForbiddenException(errMsg);
                        }
                        //not found
                        else if (errCode == "404:")
                        {
                            throw new KeyNotFoundException(errMsg);
                        }
                        //duplicate conflict
                        else if (errCode == "409:")
                        {
                            throw new DuplicateNameException(errMsg);
                        }
                        //parent gone
                        else if (errCode == "410:")
                        {
                            throw new EntityGoneException(errMsg);
                        }
                        //invalid file type
                        else if (errCode == "415:")
                        {
                            throw new InvalidOperationException(errMsg);
                        }
                        //throw original exception
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        if (hasError)
                        {
                            try
                            {
                                reader.Close();
                            }
                            catch { }
                        }
                    }

                    var resultList = new List<T>();
                    if (!hasError)
                    {
                        resultList = ReturnValParseFunc(reader).ToList();
                        reader.Close();

                        return resultList;
                    }
                    return null;

                }
            }
        }

    }
}
