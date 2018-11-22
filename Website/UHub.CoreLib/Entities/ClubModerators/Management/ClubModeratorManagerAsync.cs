using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.ClubModerators.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.ClubModerators.Management
{
#pragma warning disable 612, 618
    public static partial class ClubModeratorManager
    {
        public static async Task<(long? ClubModID, ClubModeratorResultCode ResultCode)> TryCreateClubModeratorAsync(ClubModerator NewModerator, long ParentID)
        {

            long? id = null;
            try
            {
                id = await ClubModeratorWriter.CreateClubModeratorAsync(NewModerator, ParentID);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentOutOfRangeException)
            {
                return (null, ClubModeratorResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentNullException)
            {
                return (null, ClubModeratorResultCode.NullArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentException)
            {
                return (null, ClubModeratorResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidCastException)
            {
                return (null, ClubModeratorResultCode.InvalidArgumentType);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidOperationException)
            {
                return (null, ClubModeratorResultCode.InvalidOperation);
            }
            catch (AggregateException ex) when (ex.InnerException is AccessForbiddenException)
            {
                return (null, ClubModeratorResultCode.AccessDenied);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("9B1CF137-A021-40DB-ADEC-6F09E277DEA2", ex);
                return (null, ClubModeratorResultCode.UnknownError);
            }




            if (id == null)
            {
                return (id, ClubModeratorResultCode.UnknownError);
            }

            return (id, ClubModeratorResultCode.Success);
        }

    }
#pragma warning restore

}
