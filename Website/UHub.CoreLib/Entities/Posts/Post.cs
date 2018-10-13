using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;

namespace UHub.CoreLib.Entities.Posts
{
    [DataClass]
    public sealed partial class Post
    {
        [DataProperty(EnableDBColumnValidation: false)]
        public long? ID { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsEnabled { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsReadOnly { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Name { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Content { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsModified { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public long ViewCount { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsLocked { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool CanComment { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsPublic { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsDeleted { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool CreatedBy { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool ModifiedBy { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset ModifiedDate { get; set; }
    }
}
