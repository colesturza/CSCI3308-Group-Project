using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Entities.SchoolClubs.Management
{
    internal static partial class SchoolClubWriter
    {

        /// <summary>
        /// Create DB entity for school club
        /// </summary>
        /// <param name="Club"></param>
        /// <returns></returns>
        public static async Task<long?> TryCreateClubAsync(SchoolClub Club)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                long? clubID = await SqlWorker.ExecScalarAsync<long?>
                    (CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[SchoolClub_Create]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(Club.Name);
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = HandleParamEmpty(Club.Description);
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = Club.SchoolID;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = Club.CreatedBy;
                        cmd.Parameters.Add("@IsReadOnly", SqlDbType.BigInt).Value = Club.IsReadOnly;
                    });

                if (clubID == null)
                {
                    return null;
                }

                return clubID;
            }
            catch (Exception ex)
            {
                var errCode = "493296D4-9F35-4823-B0D0-48D9C30F3A86";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }
    }
}
