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
            catch (AggregateException ex) when (ex.InnerException is ArgumentOutOfRangeException)
            {
                return (null, PostResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentNullException)
            {
                return (null, PostResultCode.NullArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentException)
            {
                return (null, PostResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidCastException)
            {
                return (null, PostResultCode.InvalidArgumentType);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidOperationException)
            {
                return (null, PostResultCode.InvalidOperation);
            }
            catch (AggregateException ex) when (ex.InnerException is AccessForbiddenException)
            {
                return (null, PostResultCode.AccessDenied);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("02A39800-18D1-4E43-8E3B-A2D6BCF30302", ex);
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
