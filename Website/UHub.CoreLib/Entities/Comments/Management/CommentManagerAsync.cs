using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Comments.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Comments.Management
{
#pragma warning disable 612, 618
    public static partial class CommentManager
    {
        public static async Task<(long? CommentID, CommentResultCode ResultCode)> TryCreateCommentAsync(Comment NewComment)
        {

            Shared.TryCreate_HandleAttrTrim(ref NewComment);

            Shared.TryCreate_AttrConversionHandler(ref NewComment);

            var attrValidateCode = Shared.TryCreate_ValidateCommentAttrs(NewComment);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }


            long? id = null;
            try
            {
                id = await CommentWriter.CreateCommentAsync(NewComment);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentOutOfRangeException)
            {
                return (null, CommentResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentNullException)
            {
                return (null, CommentResultCode.NullArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is ArgumentException)
            {
                return (null, CommentResultCode.InvalidArgument);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidCastException)
            {
                return (null, CommentResultCode.InvalidArgumentType);
            }
            catch (AggregateException ex) when (ex.InnerException is InvalidOperationException)
            {
                return (null, CommentResultCode.InvalidOperation);
            }
            catch (AggregateException ex) when (ex.InnerException is AccessForbiddenException)
            {
                return (null, CommentResultCode.AccessDenied);
            }
            catch (Exception ex)
            {
                CoreFactory.Singleton.Logging.CreateErrorLogAsync("8C7BB118-E28E-4E86-8868-537D5DAD155F", ex);
                return (null, CommentResultCode.UnknownError);
            }




            if (id == null)
            {
                return (id, CommentResultCode.UnknownError);
            }

            return (id, CommentResultCode.Success);

        }
    }


#pragma warning restore
}
