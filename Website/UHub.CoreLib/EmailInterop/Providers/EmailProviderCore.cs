﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.EmailInterop.Templates;

namespace UHub.CoreLib.EmailInterop.Providers
{
    public abstract partial class EmailProvider
    {

        internal abstract void Validate();

    }
}
