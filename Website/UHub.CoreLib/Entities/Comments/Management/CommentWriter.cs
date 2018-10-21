using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Comments.Management
{
    internal static partial class CommentWriter
    {
        /// <summary>
        /// Attempts to create a new CMS comment in the database and returns the CommentID if successful
        /// </summary>
        /// <param name="cmsComment"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        internal static long? TryCreateComment(Comment cmsComment, long ParentID)
        {
            return TryCreateComment(cmsComment, ParentID, out _);
        }

        /// <summary>
        /// Attempts to create a new CMS comment in the database and returns the CommentID if successful
        /// </summary>
        /// <param name="cmsComment"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        internal static long? TryCreateComment(Comment cmsComment, long ParentID, out string ErrorMsg)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                long? CommentID = SqlWorker.ExecScalar<long?>
                    (CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Comment_Create]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsComment.Content);
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = HandleParamEmpty(cmsComment.CreatedBy);
                        cmd.Parameters.Add("@IsReadonly", SqlDbType.Bit).Value = HandleParamEmpty(cmsComment.IsReadOnly);
                    });

                if (CommentID == null)
                {
                    throw new Exception(ResponseStrings.DBError.WRITE_UNKNOWN);
                }

                ErrorMsg = null;
                return CommentID;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex);
                ErrorMsg = ex.Message;
                return null;
            }
        }
    }
}
