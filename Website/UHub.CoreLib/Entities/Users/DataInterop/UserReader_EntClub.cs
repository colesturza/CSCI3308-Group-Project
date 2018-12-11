using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    public static partial class UserReader
    {
        public static IEnumerable<long> TryGetValidClubMemberships(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {

                return SqlWorker.ExecBasicQuery<long>(
                    _dbConn,
                    "[dbo].[User_GetValidClubMemberships_Light]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    },
                    (reader) =>
                    {
                        //READ FIRST COLUMN 
                        //read column 1 as long
                        //this column has the ID of clubs where user is an approved member
                        return reader.GetInt64(0);
                    });
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, "FCDB5385-E010-4AC3-B6E5-D3F7B9D28657");
                return null;
            }
        }


    }
}
