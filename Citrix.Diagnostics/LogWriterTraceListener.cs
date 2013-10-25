/*
 * Copyright (c) 2006 - 2007 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Diagnostics;
using System.IO;

namespace Citrix.Diagnostics {

    public class LogWriterTraceListener : TextWriterTraceListener {

        public LogWriterTraceListener (string initializeData) : base (Expand (initializeData)) {
        }

        private static string Expand (string initializeData) {
            string path = Environment.ExpandEnvironmentVariables (initializeData);
            try {
                // FileInfo creates the directory if it does not exist (!)
                FileInfo fi = new FileInfo (path);
            } catch (Exception) {
            }     
            return path;
        }
    }
}
