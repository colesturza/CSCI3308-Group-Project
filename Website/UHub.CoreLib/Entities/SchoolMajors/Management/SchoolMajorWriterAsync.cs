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

namespace UHub.CoreLib.Entities.SchoolMajors.Management
{
    internal static partial class SchoolMajorWriter
    {

        /// <summary>
        /// Attempts to create a new CMS school major in the database and returns the SchoolMajorID if successful
        /// </summary>
        /// <param name="cmsSchoolMajor"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        internal static async Task<long?> TryCreateSchoolMajorAsync(SchoolMajor cmsSchoolMajor, long ParentID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                long? schoolMajorID = await SqlWorker.ExecScalarAsync<long?>
                    (CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[SchoolMajor_Create]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsSchoolMajor.Name);
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = HandleParamEmpty(cmsSchoolMajor.Description);
                        cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = ParentID;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = HandleParamEmpty(cmsSchoolMajor.CreatedBy);
                        cmd.Parameters.Add("@IsReadonly", SqlDbType.BigInt).Value = HandleParamEmpty(cmsSchoolMajor.IsReadonly);
                    });

                if (schoolMajorID == null)
                {
                    throw new Exception(ResponseStrings.DBError.WRITE_UNKNOWN);
                }

                
                return schoolMajorID;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex);
                return null;
            }
        }
    }
}