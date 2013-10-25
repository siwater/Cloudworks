/*
 * Copyright (c) 2006 - 2007 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Win32;

namespace Citrix.Diagnostics {


    public class CtxSwitch : TraceSwitch {

        private static TraceLevel GetTraceLevel (string Value) {
            try {
                int v = int.Parse (Value);
                if (Enum.IsDefined (typeof (TraceLevel), v)) {
                    return (TraceLevel)v;
                } else if (v > (int)TraceLevel.Verbose) {
                    Debug.WriteLine ("");
                    return TraceLevel.Verbose;
                }
            } catch (Exception e) {
                Trace.WriteLine (e);
            }
            return TraceLevel.Error;
        }

        public static CtxSwitch Create (string Name, ConfigurationSection configSection) {
            foreach (ConfigurationElement ce in configSection.ElementInformation.Properties["switches"].Value as ConfigurationElementCollection) {
                string switchName = ce.ElementInformation.Properties["name"].Value as string;
                string switchValue = ce.ElementInformation.Properties["value"].Value as string;
                if (switchName == Name) {
                    CtxSwitch sw = new CtxSwitch (Name);
                    sw.Level = GetTraceLevel (switchValue);
                    return sw;
                }
            }
            return null;
        }
        
        public static CtxSwitch Create (string Name) {
            return new CtxSwitch (Name);
        }

        private CtxSwitch (string Name)
            : base (Name, "Control provisioning system trace level", "Error") {
        }

 
    }
}
