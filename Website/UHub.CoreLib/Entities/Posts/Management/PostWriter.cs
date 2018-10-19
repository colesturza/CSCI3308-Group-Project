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

namespace UHub.CoreLib.Entities.Posts.Management
{
    internal static partial class PostWriter
    {
        /// <summary>
        /// Attempts to create a new CMS post in the database and returns the PsotID if successful
        /// </summary>
        /// <param name="cmsPost"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        internal static long? TryCreatePost(Post cmsPost, long ParentID)
        {
            return TryCreatePost(cmsPost, ParentID, out _);
        }

        /// <summary>
        /// Attempts to create a new CMS post in the database and returns the PsotID if successful
        /// </summary>
        /// <param name="cmsPost"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        internal static long? TryCreatePost(Post cmsPost, long ParentID, out string ErrorMsg)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                long? PostID = SqlWorker.ExecScalar<long?>
                    (CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Post_Create]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsPost.Name);
                        cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsPost.Content);
                        cmd.Parameters.Add("@IsLocked", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.IsLocked);
                        cmd.Parameters.Add("@CanComment", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.CanComment);
                        cmd.Parameters.Add("@IsPublic", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.IsPublic);
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = HandleParamEmpty(cmsPost.CreatedBy);
                        cmd.Parameters.Add("@IsReadonly", SqlDbType.Bit).Value = HandleParamEmpty(cmsPost.IsReadOnly);
                    });

                if (PostID == null)
                {
                    throw new Exception(ResponseStrings.DBError.WRITE_UNKNOWN);
                }

                ErrorMsg = null;
                return PostID;
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
