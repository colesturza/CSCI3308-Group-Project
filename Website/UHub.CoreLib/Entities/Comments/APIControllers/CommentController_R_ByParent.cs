﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.Entities.Comments.DTOs;
using UHub.CoreLib.Entities.Comments.DataInterop;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Comments.APIControllers
{
    public sealed partial class CommentController
    {
        [HttpPost()]
        [Route("GetByParent")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByParent(long parentID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }



            var comments = await CommentReader.GetCommentsByParentAsync(parentID);

            var outSet = comments.Select(x => x.ToDto<Comment_R_PublicDTO>());
            return Ok(outSet);
        }
    }
}
