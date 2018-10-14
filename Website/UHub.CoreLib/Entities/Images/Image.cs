﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Entities.Images
{
    [DataClass]
    public sealed partial class Image : DBEntityBase
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
        public long ViewCount { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string FilePath { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Description { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string FileHash_SHA256 { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string SourceName { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string SourceType { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string DownloadName { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string ParentID { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsDeleted { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public long CreatedBy { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public long? ModifiedBy { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
