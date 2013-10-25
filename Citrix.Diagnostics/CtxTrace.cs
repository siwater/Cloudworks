/*
 * Copyright (c) 2006 - 2007 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Citrix.Configuration;

namespace Citrix.Diagnostics {

    /// <summary>
    /// Control run-time diagnostics
    /// </summary>
    public class CtxTrace {

        /// <summary>
        /// This is the default switch that will trace errors only. It gets used if you can't be bothered
        /// to call the Initialize or InitializeDLL methods to configure a specific switch.
        /// </summary>
        private static CtxSwitch _CtxSwitch = CtxSwitch.Create ("ctx-trace");
        private static bool _TraceThreads = false;

        public static TraceLevel Level {
            get {
                return _CtxSwitch.Level;
            }
        }

        public static bool Initialized { get; private set; }

        #region Initialization for App and SnapIn

        /// <summary>
        /// Initialize tracing for an application (web or windows). The config file has been loaded automatically so
        /// it just remains to create the appropriate switch
        /// </summary>
        /// <param name="traceClass"></param>
        public static void Initialize (string traceClass) {
            _CtxSwitch = CtxSwitch.Create (traceClass);
            CheckListeners (traceClass);
        }

        public static void Initialize (string traceClass, bool TraceThreads) {
            _CtxSwitch = CtxSwitch.Create (traceClass);
            CheckListeners (traceClass);
            _TraceThreads = TraceThreads;
        }

        public static void Initialize (string traceClass, TraceLevel traceLevel) {
            _CtxSwitch = CtxSwitch.Create (traceClass);
            _CtxSwitch.Level = traceLevel;
            CheckListeners (traceClass);
        }


        /// <summary>
        /// Intialize tracing for a snap-in loaded into MMC.
        /// </summary>
        /// <param name="traceClass"></param>
        public static void InitializeDLL (string traceClass) {
            ConfigurationSection section = ConfigurationService.GetSection ("system.diagnostics");
            ConfigureListeners (section);
            CtxSwitch configSwitch = CtxSwitch.Create (traceClass, section);
            if (configSwitch != null) {
                _CtxSwitch = configSwitch;
            }
        }

        // Write only errors to event log
        private class ErrorsOnlyFilter : TraceFilter {
            public override bool ShouldTrace (TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data) {
                return ((eventType == TraceEventType.Critical) || (eventType == TraceEventType.Error));
            }
        }

        // Ensure there is at least one useful listener
        private static void CheckListeners (string traceClass) {
            Initialized = true;
            foreach (TraceListener traceListener in Trace.Listeners) {
                if ((traceListener is TextWriterTraceListener) || (traceListener is LogWriterTraceListener)) {
                    TraceInformation("Working listener called " + traceListener.Name);
                    return;
                }
            }
            string path = GetDefaultLogFileLocation ();
            TraceListener listener;
            if (path != null) {
                string logfile = Path.Combine (path, traceClass + ".log");
                listener = new TextWriterTraceListener (logfile);
                TraceInformation ("Added listener writing to " + logfile);
            } else {
                listener = new EventLogTraceListener (traceClass);
                listener.Filter = new ErrorsOnlyFilter ();
                TraceInformation ("Added listener writing to event log");
            }
            Trace.Listeners.Add (listener);
            Trace.AutoFlush = true;
        }

        private static string GetDefaultLogFileLocation () {
            string path = @"C:\Temp";
            if (Directory.Exists (path)) {
                return path;
            }
            path = Environment.GetEnvironmentVariable ("TEMP");
            if (Directory.Exists (path)) {
                return path;
            }
            path = Environment.GetEnvironmentVariable ("TMP");
            if (Directory.Exists (path)) {
                return path;
            }
            return null;
        }

        // process each listener in the app.config
        private static void ConfigureListeners (ConfigurationSection section) {
            Trace.AutoFlush = true;
            ConfigurationElement trace = section.ElementInformation.Properties["trace"].Value as ConfigurationElement;
            ConfigurationElementCollection listeners = trace.ElementInformation.Properties["listeners"].Value as ConfigurationElementCollection;
            foreach (ConfigurationElement ce in listeners) {
                ConfigureListener (ListenerElement.Create (ce.ElementInformation));
            }
        }

        private static void ConfigureListener (ListenerElement configElement) {
            foreach (TraceListener traceListener in Trace.Listeners) {
                if (configElement.Describes (traceListener)) {
                    return;
                }
            }
            Add (configElement);
        }

        private static void Add (ListenerElement configElement) {
            Type type = configElement.GetTraceListenerType ();
            if (type != null) {
                ConstructorInfo ci = type.GetConstructor (new Type[] { typeof (string) });
                if (ci != null) {
                    TraceListener tl = ci.Invoke (new object[] { configElement.initializeData }) as TraceListener;
                    Trace.Listeners.Add (tl);
                }
            }
        }

        #endregion


        #region Trace Errors

        public static void TraceError (Exception e) {
            if (_CtxSwitch.TraceError) {
                InternalWriteLine (TraceLevel.Error, e);
            }
        }

        public static void TraceError (Exception e, string msg) {
            if (_CtxSwitch.TraceError) {
                InternalWriteLine (TraceLevel.Error, msg);
                InternalWriteLine (TraceLevel.Error, e);
            }
        }

        public static void TraceError (string msg) {
            if (_CtxSwitch.TraceError) {
                InternalWriteLine (TraceLevel.Error, msg);
            }
        }

        public static void TraceError (string Format, params object[] args) {
            if (_CtxSwitch.TraceError) {
                InternalWriteLine (TraceLevel.Error, String.Format (Format, args));
            }
        }

        #endregion

        #region Trace Warnings

        public static void TraceWarning (object obj) {
            if (_CtxSwitch.TraceWarning) {
                InternalWriteLine (TraceLevel.Warning, obj);
            }
        }
        public static void TraceWarning (string Format, params object[] args) {
            if (_CtxSwitch.TraceWarning) {
                InternalWriteLine (TraceLevel.Warning, String.Format (Format, args));
            }
        }

        #endregion

        #region Trace Information

        public static void TraceInformation () {
            if (_CtxSwitch.TraceInfo) {
                InternalWriteLine (TraceLevel.Info, null);
            }
        }
        public static void TraceInformation (object obj) {
            if (_CtxSwitch.TraceInfo) {
                InternalWriteLine (TraceLevel.Info, obj);
            }
        }
        public static void TraceInformation (string Format, params object[] args) {
            if (_CtxSwitch.TraceInfo) {
                InternalWriteLine (TraceLevel.Info, String.Format (Format, args));
            }
        }

        #endregion

        #region Trace Verbose

        public static void TraceVerbose () {
            if (_CtxSwitch.TraceVerbose) {
                InternalWriteLine (TraceLevel.Verbose, null);
            }
        }
        public static void TraceVerbose (object obj) {
            if (_CtxSwitch.TraceVerbose) {
                InternalWriteLine (TraceLevel.Verbose, obj);
            }
        }

        public static void TraceVerbose (string Format, params object[] args) {
            if (_CtxSwitch.TraceVerbose) {
                InternalWriteLine (TraceLevel.Verbose, String.Format (Format, args));
            }
        }

        #endregion

        private static string LevelString (TraceLevel level) {
            switch (level) {
                case TraceLevel.Error: return " [E]";
                case TraceLevel.Warning: return " [W]";
                case TraceLevel.Info: return " [I]";
                case TraceLevel.Verbose: return " [V]";
                default: return "";
            }
        }

        private static string ThreadId {
            get {
                return (_TraceThreads) ? " [thread:" + System.Threading.Thread.CurrentThread.GetHashCode ().ToString () + "]" : "";
            }
        }

        private static void InternalWriteLine (TraceLevel level, object obj) {
            StackFrame frame = new StackFrame (2, true);
            MethodBase method = frame.GetMethod ();
            string name = "Unknown";
            if (method != null)
            {
                var typeName = method.DeclaringType == null ? "UnknownType" : method.DeclaringType.Name;
                name = " " + typeName + "." + method.Name;
            }

            string msg = (obj == null) ? "" : ": " + obj.ToString();
            string timestamp = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            Trace.WriteLine(timestamp + LevelString(level) + ThreadId + name + msg);
        }
    }
}
