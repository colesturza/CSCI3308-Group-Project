using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.Management
{
#pragma warning disable 612,618

    public static partial class SchoolClubManager
    {
        public static async Task<(long? ClubID, SchoolClubResultCode ResultCode)> TryCreateClubAsync(SchoolClub NewClub)
        {
            Shared.TryCreate_HandleAttrTrim(ref NewClub);

            Shared.TryCreate_AttrConversionHandler(ref NewClub);

            var attrValidateCode = Shared.TryCreate_ValidateClubAttrs(NewClub);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }



            long? id = null;
            try
            {
                id = await SchoolClubWriter.CreateClubAsync(NewClub);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentOutOfRangeException)
            {
                return (null, SchoolClubResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentNullException)
            {
                return (null, SchoolClubResultCode.NullArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentException)
            {
                return (null, SchoolClubResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidCastException)
            {
                return (null, SchoolClubResultCode.InvalidArgumentType);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidOperationException)
            {
                return (null, SchoolClubResultCode.InvalidOperation);
            }
            catch (AggregateException ex) when (ex.InnerException is AccessForbiddenException)
            {
                return (null, SchoolClubResultCode.AccessDenied);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("11D2E2C7-C000-4BA1-8EF7-D7857E3B2C25", ex);
                return (null, SchoolClubResultCode.UnknownError);
            }





            if (id == null)
            {
                return (id, SchoolClubResultCode.UnknownError);
            }
            return (id, SchoolClubResultCode.Success);

        }

    }
#pragma warning restore

}
