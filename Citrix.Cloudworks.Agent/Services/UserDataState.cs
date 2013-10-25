using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citrix.Cloudworks.Agent.Services {
    
    /// <summary>
    /// Utility class to record the state of user data processing. To ensure 
    /// user-data scripts are not repeatedly executed on reboot a flag is written into the
    /// registry when initialisation is complete.
    /// </summary>
    public static class UserDataState {

        private const string CacheKey = "SOFTWARE\\Policies\\Citrix\\StackMate";
        private const string CacheName = "InitialisationComplete";

        private static PersistentCache _PersitentCache;

        static UserDataState() {
            _PersitentCache = new PersistentCache(CacheKey, CacheName);
        }

        public static bool InitialisationComplete {
            get {
                bool result = true;
                bool.TryParse(_PersitentCache.Read(), out result);
                return result;
            }
            set {
                _PersitentCache.Write(value.ToString());
            }
        }

    }
}
