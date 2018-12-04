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
            catch (EntityGoneException)
            {
                return (null, PostResultCode.InvalidOperation);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("05D71E7E-0D15-4D87-8ADB-16BBFD966B0C", ex);
                return (null, PostResultCode.UnknownError);
            }



            if (id == null)
            {
                return (id, PostResultCode.UnknownError);
            }
            return (id, PostResultCode.Success);

        }



        public static async Task<bool?> TryIncrementViewCountAsync(long PostID, long UserID)
        {

            bool? val = null;
            try
            {
                val = await PostWriter.IncrementViewCountAsync(PostID, UserID);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("7FB424FA-8548-47F1-AC05-A38183376902", ex);
            }


            return val;
        }



        public static async Task<PostResultCode> TryUpdatePostAsync(Post CmsPost)
        {
            if (CmsPost == null)
            {
                return PostResultCode.NullArgument;
            }



            Shared.TryCreate_HandleAttrTrim(ref CmsPost);

            Shared.TryCreate_AttrConversionHandler(ref CmsPost);


            var attrValidateCode = Shared.TryCreate_ValidatePostAttrs(CmsPost);
            if (attrValidateCode != 0)
            {
                return attrValidateCode;
            }



            try
            {
                await PostWriter.UpdatePostAsync(CmsPost);
            }
            catch (ArgumentOutOfRangeException)
            {
                return PostResultCode.InvalidArgument;
            }
            catch (ArgumentNullException)
            {
                return PostResultCode.NullArgument;
            }
            catch (ArgumentException)
            {
                return PostResultCode.InvalidArgument;
            }
            catch (InvalidCastException)
            {
                return PostResultCode.InvalidArgumentType;
            }
            catch (InvalidOperationException)
            {
                return PostResultCode.InvalidOperation;
            }
            catch (AccessForbiddenException)
            {
                return PostResultCode.AccessDenied;
            }
            catch (EntityGoneException)
            {
                return PostResultCode.InvalidOperation;
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("CDB83704-5E14-48DB-AEB9-FA947EA91D0B", ex);
                return PostResultCode.UnknownError;
            }


            return PostResultCode.Success;
        }
    }

#pragma warning restore
}
