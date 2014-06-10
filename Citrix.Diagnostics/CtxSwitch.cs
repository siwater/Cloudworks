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
