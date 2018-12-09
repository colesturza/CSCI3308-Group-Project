using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Entities.Posts.DTOs;
using UHub.CoreLib.Entities.Posts.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.Entities.Schools.DataInterop;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Security;
using UHub.CoreLib.Entities.Posts.Management;
using System.Dynamic;
using UHub.CoreLib.Entities.Users.DataInterop;

namespace UHub.CoreLib.Entities.Posts.APIControllers
{
    public sealed partial class PostController
    {
        [HttpPost()]
        [Route("GetByID")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetByID(long PostID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var postInternal = await PostReader.TryGetPostAsync(PostID);
            if (postInternal == null)
            {
                return NotFound();
            }

            var taskPostCreateUser = UserReader.GetUserAsync(postInternal.CreatedBy);
            var parentID = postInternal.ParentID;
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var taskPostClub = SchoolClubReader.TryGetClubAsync(parentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(parentID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(parentID, cmsUser.ID.Value);


            var postPublic = postInternal.ToDto<Post_R_PublicDTO>();
            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            if ((sanitizerMode & HtmlSanitizerMode.OnRead) != 0)
            {
                postPublic.Content = postPublic.Content.SanitizeHtml().HtmlDecode();
            }


            var postClub = await taskPostClub;
            if (postClub != null)
            {
                //verify same school
                if (postClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var IsUserBanned = await taskIsUserBanned;
                //ensure not banned
                if (IsUserBanned)
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }


                var IsUserMember = await taskIsUserMember;


                


                //check for member status
                if (IsUserMember || postInternal.IsPublic)
                {
                    var taskIncrementCount = PostManager.TryIncrementViewCountAsync(PostID, cmsUser.ID.Value);
                    var postCreateUser = await taskPostCreateUser;


                    var postPublicWithUser = new
                    {
                        postPublic.ID,
                        postPublic.IsReadOnly,
                        postPublic.Name,
                        postPublic.Content,
                        postPublic.IsModified,
                        postPublic.ViewCount,
                        postPublic.IsLocked,
                        postPublic.CanComment,
                        postPublic.IsPublic,
                        postPublic.ParentID,
                        postPublic.CreatedBy,
                        postPublic.CreatedDate,
                        postPublic.ModifiedBy,
                        postPublic.ModifiedDate,
                        postCreateUser.Username
                    };


                    await taskIncrementCount;
                    return Ok(postPublicWithUser);
                }
                else
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }
            }


            // This is what happens if the parent is a school.
            //verify same school
            if (postInternal.ParentID != cmsUser.SchoolID)
            {
                return NotFound();
            }


            var taskIncrementCountOuter = PostManager.TryIncrementViewCountAsync(PostID, cmsUser.ID.Value);
            var postCreateUserOuter = await taskPostCreateUser;



            var postPublicWithUserOuter = new
            {
                postPublic.ID,
                postPublic.IsReadOnly,
                postPublic.Name,
                postPublic.Content,
                postPublic.IsModified,
                postPublic.ViewCount,
                postPublic.IsLocked,
                postPublic.CanComment,
                postPublic.IsPublic,
                postPublic.ParentID,
                postPublic.CreatedBy,
                postPublic.CreatedDate,
                postPublic.ModifiedBy,
                postPublic.ModifiedDate,
                postCreateUserOuter.Username
            };


            await taskIncrementCountOuter;
            return Ok(postPublicWithUserOuter);
        }



        [HttpPost()]
        [Route("GetRevisionsByID")]
        [ApiAuthControl]
        public async Task<IHttpActionResult> GetRevisionsByID(long PostID)
        {
            string status = "";
            HttpStatusCode statCode = HttpStatusCode.BadRequest;
            if (!this.ValidateSystemState(out status, out statCode))
            {
                return Content(statCode, status);
            }


            var postEnum = await PostReader.TryGetPostRevisionsAsync(PostID);
            if (postEnum == null)
            {
                return NotFound();
            }
            var postList = postEnum.ToList();
            if (postList.Count == 0)
            {
                return NotFound();
            }


            var postParentID = postList.First().ParentID;
            var cmsUser = CoreFactory.Singleton.Auth.GetCurrentUser().CmsUser;



            var taskPostClub = SchoolClubReader.TryGetClubAsync(postParentID);
            var taskIsUserBanned = SchoolClubReader.TryIsUserBannedAsync(postParentID, cmsUser.ID.Value);
            var taskIsUserMember = SchoolClubReader.TryValidateMembershipAsync(postParentID, cmsUser.ID.Value);


            List<Post_R_PublicDTO> postListPublic = postList.Select(x => x.ToDto<Post_R_PublicDTO>()).ToList();
            var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
            if ((sanitizerMode & HtmlSanitizerMode.OnRead) != 0)
            {
                postListPublic.ForEach(x =>
                {
                    x.Content = x.Content.SanitizeHtml().HtmlDecode();
                });
            }



            var postClub = await taskPostClub;
            if (postClub != null)
            {
                //verify same school
                if (postClub.SchoolID != cmsUser.SchoolID)
                {
                    return NotFound();
                }

                var IsUserBanned = await taskIsUserBanned;
                //ensure not banned
                if (IsUserBanned)
                {
                    return Content(HttpStatusCode.Forbidden, "Access Denied");
                }


                var IsUserMember = await taskIsUserMember;
                //check for member status
                if (!IsUserMember)
                {
                    postListPublic = postListPublic.Where(x => x.IsPublic).ToList();
                }


                return Ok(postListPublic);
            }


            // This is what happens if the parent is a school.
            //verify same school
            if (postParentID != cmsUser.SchoolID)
            {
                return NotFound();
            }


            return Ok(postListPublic);
        }
    }
}
