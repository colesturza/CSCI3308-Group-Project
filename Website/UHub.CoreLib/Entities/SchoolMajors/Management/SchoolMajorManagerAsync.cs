using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolMajors.Management
{
#pragma warning disable 612,618

    public static partial class SchoolMajorManager
    {
        public static async Task<(long? ClubID, SchoolMajorResultCode ResultCode)> TryCreateMajorAsync(SchoolMajor NewMajor, long ParentID)
        {

            Shared.TryCreate_HandleAttrTrim(ref NewMajor);

            Shared.TryCreate_AttrConversionHandler(ref NewMajor);

            var attrValidateCode = Shared.TryCreate_ValidateMajorAttrs(NewMajor);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }



            long? id = null;
            try
            {
                id = await SchoolMajorWriter.CreateSchoolMajorAsync(NewMajor, ParentID);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, SchoolMajorResultCode.InvalidArgument);
            }
            catch (ArgumentNullException)
            {
                return (null, SchoolMajorResultCode.NullArgument);
            }
            catch (ArgumentException)
            {
                return (null, SchoolMajorResultCode.InvalidArgument);
            }
            catch (InvalidCastException)
            {
                return (null, SchoolMajorResultCode.InvalidArgumentType);
            }
            catch (InvalidOperationException)
            {
                return (null, SchoolMajorResultCode.InvalidOperation);
            }
            catch (AccessForbiddenException)
            {
                return (null, SchoolMajorResultCode.AccessDenied);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("0AD04BF9-0E72-4EAB-97D5-23EF8F2507D4", ex);
                return (null, SchoolMajorResultCode.UnknownError);
            }




            if (id == null)
            {
                return (id, SchoolMajorResultCode.UnknownError);
            }
            return (id, SchoolMajorResultCode.Success);

        }

    }
#pragma warning restore

}
