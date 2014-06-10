/*
 * Copyright (c) 2007-2007 Citrix Systems, Inc.
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
