using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.SchoolClubs.Interfaces;

namespace UHub.CoreLib.Entities.SchoolClubs.DTOs
{
    [DtoClass(typeof(SchoolClub))]
    public sealed partial class SchoolClub_R_PublicDTO : DtoEntityBase, ISchoolClub_R_Public
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
        public string Description { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset CreatedDate { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
