using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Entities.Posts
{
    [DataClass]
    public sealed partial class Post : DBEntityBase
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
        public long CreatedBy { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public long ModifiedBy { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
