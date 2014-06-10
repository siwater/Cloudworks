/*
 * Copyright (c) 2013-2014 Citrix Systems, Inc.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * 'Software'), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
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
