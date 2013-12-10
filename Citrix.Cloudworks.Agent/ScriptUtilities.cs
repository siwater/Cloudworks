using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.Win32;

using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {
    
    public static class ScriptUtilities {

        private const string PowerShellSettingsKey = @"SOFTWARE\Microsoft\PowerShell\1\ShellIds\Microsoft.PowerShell";
        private const string ExecutionPolicyKey = "ExecutionPolicy";


        public static string SetPowerShellExectionPolicy(string policy) {
            RegistryKey HKLM = Registry.LocalMachine;
            RegistryKey policyKey = HKLM.CreateSubKey(PowerShellSettingsKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
            string oldPolicy = policyKey.GetValue(ExecutionPolicyKey) as string;
            policyKey.SetValue(ExecutionPolicyKey, policy);
            return oldPolicy;
        }

        public static void CreateDirectory(string dir) {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        public static void ExecuteProcess(string executable, string args, string workingDirectory) {        
            try {
                ProcessWrapper process = new ProcessWrapper(executable, args, workingDirectory);
                process.Execute();             
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
        }
    }
}
