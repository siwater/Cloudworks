using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {

    /// <summary>
    /// Simple cache implementation to persist a small amount of data in the registry
    /// </summary>
    public class PersistentCache {

        private string CacheKey;
        private string CacheName;
        

        public PersistentCache(string cacheKey, string cacheName) {
            this.CacheKey = cacheKey;
            this.CacheName = cacheName;
        }

        /// <summary>
        /// Write data to a persistent cache
        /// </summary>
        /// <param name="userData"></param>
        public void Write (string userData) {
            CtxTrace.TraceVerbose ("UserData = {0}", userData);
            try {
                RegistryKey HKLM = Registry.LocalMachine;
                RegistryKey policyKey = HKLM.CreateSubKey(CacheKey);
                policyKey.SetValue(CacheName, userData);
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
        }

        /// <summary>
        ///  Read a cached initialisation string from persistent store
        /// </summary>
        /// <returns></returns>
        public string Read () {
            try {
                RegistryKey HKLM = Registry.LocalMachine;
                RegistryKey policyKey = HKLM.OpenSubKey(CacheKey);
                string data = (policyKey == null) ? null : policyKey.GetValue(CacheName) as string;
                CtxTrace.TraceVerbose ("Cache = \'{0}\'", ((data==null) ? "null" : data));
                return data;
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
            return null;
        }

        public void Clear() {
            CtxTrace.TraceVerbose();
            try {
                RegistryKey HKLM = Registry.LocalMachine;
                RegistryKey policyKey = HKLM.OpenSubKey(CacheKey);
                if (policyKey != null) {
                    HKLM.DeleteSubKey(CacheKey);
                }
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
        }

        public bool IsPresent {
            get {
                return !string.IsNullOrEmpty(Read());
            }
        }
    }
}
