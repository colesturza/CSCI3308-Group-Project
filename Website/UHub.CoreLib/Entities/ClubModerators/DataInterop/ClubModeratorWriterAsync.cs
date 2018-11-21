﻿using System;
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

namespace UHub.CoreLib.Entities.ClubModerators.DataInterop
{
    public sealed partial class ClubModeratorWriter
    {

        /// <summary>
        /// Attempts to create a new CMS club moderator in the database and returns the club moderators id if successful
        /// </summary>
        /// <param name="cmsClubModerator"></param>
        /// <returns></returns>
        internal static async Task<long?> TryCreateClubModeratorAsync(ClubModerator cmsClubModerator, long parentID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                long? ClubModeratorID = await SqlWorker.ExecScalarAsync<long?>(
                    _dbConn,
                    "[dbo].[SchoolClubModerator_Create]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = HandleParamEmpty(cmsClubModerator.UserID);
                        cmd.Parameters.Add("@IsOwner", SqlDbType.Bit).Value = HandleParamEmpty(cmsClubModerator.IsOwner);
                        cmd.Parameters.Add("@IsValid", SqlDbType.Bit).Value = HandleParamEmpty(cmsClubModerator.IsValid);
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = HandleParamEmpty(parentID);
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = HandleParamEmpty(cmsClubModerator.CreatedBy);
                        cmd.Parameters.Add("@IsReadonly", SqlDbType.Bit).Value = HandleParamEmpty(cmsClubModerator.IsReadOnly);
                    });

                if (ClubModeratorID == null)
                {
                    return null;
                }

                return ClubModeratorID;
            }
            catch (Exception ex)
            {
                var errCode = "BD0886CA-E8DB-4A0F-A574-ADA6FE36F4D5";
                Exception ex_outer = new Exception(errCode, ex);
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex_outer);

                return null;
            }
        }
    }
}
