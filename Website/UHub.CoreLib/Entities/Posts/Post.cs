﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Posts.Interfaces;

namespace UHub.CoreLib.Entities.Posts
{
    [DataClass]
    public sealed partial class Post : DBEntityBase, IPost_R_Public, IPost_C_Public, IPost_U_Public
    {
        [DataProperty]
        public long? ID { get; set; }

        [DataProperty]
        public bool IsEnabled { get; set; }

        [DataProperty]
        public bool IsReadOnly { get; set; }

        [DataProperty]
        public string Name { get; set; }

        [DataProperty]
        public string Content { get; set; }

        [DataProperty]
        public bool IsModified { get; set; }

        [DataProperty]
        public long ViewCount { get; set; }

        [DataProperty]
        public bool IsLocked { get; set; }

        [DataProperty]
        public bool CanComment { get; set; }

        [DataProperty]
        public bool IsPublic { get; set; }

        [DataProperty]
        public long ParentID { get; set; }

        [DataProperty]
        public bool IsDeleted { get; set; }

        [DataProperty]
        public long CreatedBy { get; set; }

        [DataProperty]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty]
        public long? ModifiedBy { get; set; }

        [DataProperty]
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
