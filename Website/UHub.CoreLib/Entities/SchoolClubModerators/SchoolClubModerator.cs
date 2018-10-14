using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Entities.SchoolClubModerators
{
    [DataClass]
    public sealed partial class SchoolClubModerator : DBEntityBase
    {
        [DataProperty(EnableDBColumnValidation: false)]
        public long? ID { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsEnabled { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsReadOnly { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public long UserID { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsOwner { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsValid { get; set; }

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
