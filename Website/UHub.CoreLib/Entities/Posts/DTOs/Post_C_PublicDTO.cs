﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Posts.Interfaces;

namespace UHub.CoreLib.Entities.Posts.DTOs
{
    [DtoClass(typeof(Post))]
    public sealed partial class Post_C_PublicDTO : DtoEntityBase, IPost_C_Public
    {
        
        public string Name { get; set; }
        public string Content { get; set; }
        public bool CanComment { get; set; }
        public bool IsPublic { get; set; }
        public long ParentID { get; set; }
    }
}
