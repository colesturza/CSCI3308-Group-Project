using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Config;
using UHub.CoreLib.Security.Accounts;

namespace UHub.CoreLib.Management
{
    public static class CoreFactory
    {

        private static bool isInstantiated = false;
        private static CoreManager _singleton;
        public static CoreManager Singleton
        {
            get
            {
                if (isInstantiated)
                {
                    return _singleton;
                }

                throw new InvalidOperationException("CoreManager not initialized");
            }
        }



        public static void Initialize(CmsConfiguration_Grouped config)
        {
            if (isInstantiated)
            {
                throw new InvalidOperationException("CoreManager already initialized");
            }


            _singleton = new CoreManager(config);
            isInstantiated = true;


        }



    }
}
