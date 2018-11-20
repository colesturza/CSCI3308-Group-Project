using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.Posts.DataInterop;
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


            var id = await PostWriter.TryCreatePostAsync(NewPost);
            return (id, PostResultCode.Success);

        }
    }

#pragma warning restore
}
