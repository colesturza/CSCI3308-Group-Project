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
            catch (AggregateException ex) when (ex.InnerException is ArgumentOutOfRangeException)
            {
                return (null, SchoolMajorResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentNullException)
            {
                return (null, SchoolMajorResultCode.NullArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentException)
            {
                return (null, SchoolMajorResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidCastException)
            {
                return (null, SchoolMajorResultCode.InvalidArgumentType);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidOperationException)
            {
                return (null, SchoolMajorResultCode.InvalidOperation);
            }
            catch (AggregateException ex) when (ex.InnerException is AccessForbiddenException)
            {
                return (null, SchoolMajorResultCode.AccessDenied);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("DB6AC4FD-4FB1-4448-B218-5FBBCFC80739", ex);
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
