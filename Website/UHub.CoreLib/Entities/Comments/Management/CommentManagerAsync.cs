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
        public static async Task<(long? CommentID, CommentResultCode ResultCode)> TryCreateCommentAsync(Comment NewComment)
        {

            Shared.TryCreate_HandleAttrTrim(ref NewComment);

            Shared.TryCreate_AttrConversionHandler(ref NewComment);

            var attrValidateCode = Shared.TryCreate_ValidateCommentAttrs(NewComment);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }


            var id = await CommentWriter.TryCreateCommentAsync(NewComment);
            return (id, CommentResultCode.Success);

        }
    }


#pragma warning restore
}
