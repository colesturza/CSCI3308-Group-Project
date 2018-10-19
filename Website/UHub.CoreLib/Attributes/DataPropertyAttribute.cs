using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Attributes
{
    /// <summary>
    /// Data attribute to designate DB autoload property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DataPropertyAttribute : Attribute
    {
        /// <summary>
        /// DB native name to be set if the property name does not match the DB column name
        /// </summary>
        public string DBNativeName { get; set; }

        /// <summary>
        /// Enable checks for column validity when reading from DB
        /// </summary>
        public bool EnableDBColumnValidation { get; private set; } = false;

        /// <summary>
        /// DataPropertyAttribute constructor
        /// </summary>
        public DataPropertyAttribute()
        {
            DBNativeName = null;
        }

        /// <summary>
        /// DataPropertyAttribute constructor with custom DB property name
        /// </summary>
        /// <param name="DBNativeName"></param>
        public DataPropertyAttribute(string DBNativeName)
        {
            this.DBNativeName = DBNativeName;
        }

        /// <summary>
        /// DataPropertyAttribute constructor with custom DB property name
        /// </summary>
        /// <param name="DBNativeName"></param>
        /// <param name="EnableDBColumnValidation"></param>
        public DataPropertyAttribute(string DBNativeName = null, bool EnableDBColumnValidation = false)
        {
            this.DBNativeName = DBNativeName;
            this.EnableDBColumnValidation = EnableDBColumnValidation;
        }
    }
}
