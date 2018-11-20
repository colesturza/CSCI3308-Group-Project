using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Comments.DataInterop;

namespace UHub.CoreLib.Entities.Comments.Management
{
#pragma warning disable 612, 618
    public static partial class CommentManager
    {
        public static (long? CommentID, CommentResultCode ResultCode) TryCreateComment(Comment NewComment)
        {

            Shared.TryCreate_HandleAttrTrim(ref NewComment);

            Shared.TryCreate_AttrConversionHandler(ref NewComment);

            var attrValidateCode = Shared.TryCreate_ValidateCommentAttrs(NewComment);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }


            var id = CommentWriter.TryCreateComment(NewComment);
            if (id == null)
            {
                return (id, CommentResultCode.UnknownError);
            }
            return (id, CommentResultCode.Success);

        }
    }


#pragma warning restore
}
