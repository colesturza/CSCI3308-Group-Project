using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;

namespace UHub.CoreLib.Entities.Posts.Management
{
#pragma warning disable 612,618

    public static partial class PostManager
    {
        public static async Task<(long? PostID, PostResultCode ResultCode)> TryCreatePostAsync(Post NewPost)
        {
            Shared.TryCreate_HandleAttrTrim(ref NewPost);

            Shared.TryCreate_AttrConversionHandler(ref NewPost);

            var attrValidateCode = Shared.TryCreate_ValidatePostAttrs(NewPost);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }



            long? id = null;
            try
            {
                id = await PostWriter.CreatePostAsync(NewPost);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, PostResultCode.InvalidArgument);
            }
            catch (ArgumentNullException)
            {
                return (null, PostResultCode.NullArgument);
            }
            catch (ArgumentException)
            {
                return (null, PostResultCode.InvalidArgument);
            }
            catch (InvalidCastException)
            {
                return (null, PostResultCode.InvalidArgumentType);
            }
            catch (InvalidOperationException)
            {
                return (null, PostResultCode.InvalidOperation);
            }
            catch (AccessForbiddenException)
            {
                return (null, PostResultCode.AccessDenied);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("45CB4726-3D28-4D65-A0FE-AB53EFA3C705", ex);
                return (null, PostResultCode.UnknownError);
            }






            if (id == null)
            {
                return (id, PostResultCode.UnknownError);
            }
            return (id, PostResultCode.Success);

        }
    }

#pragma warning restore
}
