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

        public static (long? PostID, PostResultCode ResultCode) TryCreatePost(Post NewPost)
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
                id = PostWriter.CreatePost(NewPost);
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
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("CDB83704-5E14-48DB-AEB9-FA947EA91D0B", ex);
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
