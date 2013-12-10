using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Citrix.Cloudworks.Agent.Services {
    
    /// <summary>
    /// Utility class to record the state of user data processing. To ensure 
    /// user-data scripts are not repeatedly executed on reboot a flag is written into the
    /// file system when initialisation is complete.
    /// </summary>
    public class UserDataState {

        private string StateFile;

        public UserDataState(string dir) {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            StateFile = Path.Combine(dir, "init-done");
        }

        public bool InitialisationComplete {
            get {
               return File.Exists(StateFile);
            }
            set {
                if (value) {
                    File.Create(StateFile);
                } else {
                    if (File.Exists(StateFile)) {
                        File.Delete(StateFile);
                    }
                }
            }
        }

    }
}
