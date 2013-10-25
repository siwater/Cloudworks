/*
 * Copyright (c) 2006 - 2007 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Citrix.Configuration {


    /// <summary>
    /// An MMC 3.0 snap-in appears to be called from unmanaged code so the standard config file stuff doesn't work.
    /// This class acts as a partial wrapper for System.Configuration.ConfigurationManager to load a configuration
    /// file if one can be found. It looks for an exe config file (i.e. mmc.exe.config in the case of mmc
    /// snap-in), an app.config or a my-snap-in.dll.config.
    /// </summary>
    public static class ConfigurationService {

        static ConfigurationService () {
            LoadConfiguration ();
        }

        /// <summary>
        ///  First look for an exe.config for the current executable, then a generic app.config. If that
        /// fails, see if there is a config file for the root assembly
        /// </summary>
        private static void LoadConfiguration () {

            // exe.config
            _Configuration = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
            if (_Configuration.HasFile) {
                return;
            }

            // app.config
            string dir = Path.GetDirectoryName (Assembly.GetCallingAssembly ().Location);
            string config = Path.Combine (dir, "App.config");
            if (LoadConfiguration (config)) {
                return;
            }

           // dll.config
            config = FindRootAssemblyLocation (dir);
            if (config != null) {
                LoadConfiguration (config + ".config");
            }
        }

        private static bool LoadConfiguration (string config) {
            if (File.Exists (config)) {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap ();
                map.ExeConfigFilename = config;
                _Configuration = ConfigurationManager.OpenMappedExeConfiguration (map, ConfigurationUserLevel.None);
                return true;
            }
            return false;
        }


        // Walk up the stack looking for the first Assembly in the specified directory.
        private static string FindRootAssemblyLocation (string Dir) {
            StackTrace trace = new StackTrace ();
            StackFrame [] frames = trace.GetFrames ();
            for (int i = frames.Length-1; i > 0; i--) {
                string path = frames[i].GetMethod().DeclaringType.Assembly.Location;
                if (path.StartsWith (Dir)) {
                    return path;
                }
            }
            return null;
        }

        private static System.Configuration.Configuration _Configuration;

        public static ConfigurationSection GetSection (string SectionName) {
            return _Configuration.GetSection (SectionName);
        }

        private static NameValueCollection _AppSettings;
        public static NameValueCollection AppSettings {
            get {
                if (_AppSettings == null) {
                    _AppSettings = new NameValueCollection ();
                    foreach (KeyValueConfigurationElement kv in _Configuration.AppSettings.Settings) {
                        _AppSettings.Add (kv.Key, kv.Value);
                    }
                }
                return _AppSettings;
            }
        }
    }
}
