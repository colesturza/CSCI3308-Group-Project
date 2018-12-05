using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Logging.DataInterop
{
    internal static partial class LocalEventWriter
    {

        public static async Task<bool> TryWriteEventAsync(EventLogData EventData)
        {

            try
            {
                if (!CoreFactory.Singleton.IsEnabled)
                {
                    throw new SystemDisabledException();
                }


                await SqlWorker.ExecNonQueryAsync(
                    CoreFactory.Singleton.Properties.CmsDBConfig,
                    "[dbo].[Logging_CreateEventLog]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@EventTypeID", SqlDbType.SmallInt).Value = HandleParamEmpty(EventData.EventType);
                        cmd.Parameters.Add("@EventID", SqlDbType.NVarChar).Value = HandleParamEmpty(EventData.EventID);
                        cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = HandleParamEmpty(EventData.Content);
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = HandleParamEmpty(EventData.CreatedBy);
                    });


                return true;
            }
            catch
            {
                return false;
            }


        }

    }
}
