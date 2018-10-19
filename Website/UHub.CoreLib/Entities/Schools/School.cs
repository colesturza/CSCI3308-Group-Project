using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Attributes;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Entities.Schools.Interfaces;

namespace UHub.CoreLib.Entities.Schools
{
    [DataClass]
    public sealed partial class School : DBEntityBase, ISchool_R_Public
    {
        [DataProperty(EnableDBColumnValidation: false)]
        public long? ID { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Name { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string State { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string City { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsEnabled { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public bool IsReadOnly { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string Description { get; set; }

        [DataProperty(EnableDBColumnValidation: false)]
        public string DomainValidator { get; private set; }

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
