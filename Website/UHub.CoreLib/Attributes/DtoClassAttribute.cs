using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DtoClassAttribute : Attribute
    {

        public Type ConversionType { get; }


        public DtoClassAttribute(Type ConversionType)
        {
            this.ConversionType = ConversionType;
        }
    }
}
