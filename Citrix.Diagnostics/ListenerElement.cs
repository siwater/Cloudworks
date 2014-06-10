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

namespace Citrix.Diagnostics {


    /// <summary>
    /// Represents a "listeners/add" element in a config file
    /// </summary>
    internal class ListenerElement {

        // extract some random type from the system.diagnostic namespace for future reference
        static Type reference = typeof (Trace);

        private ListenerElement (string n, string t, string i) {
            Name = n; initializeData = i; TypeName = t;
        }

        public string Name;
        public string TypeName;
        public string initializeData;

        public static ListenerElement Create (ElementInformation eI) {
            string name = eI.Properties["name"].Value as string;
            string type = eI.Properties["type"].Value as string;
            string initializeData = eI.Properties["initializeData"].Value as string;
            return new ListenerElement (name, type, initializeData);
        }

        /// <summary>
        ///  Returns true if this entry in the config describes the supplied TraceListener object
        /// </summary>
        public bool Describes (TraceListener listener) {
            return (Name == listener.Name) && listener.GetType ().FullName.StartsWith (TypeName);
        }

        public Type GetTraceListenerType () {
            string fqn = TypeName;
            if (fqn.StartsWith (reference.Namespace) && !fqn.Contains (",")) {
                fqn += ", " + reference.Assembly.FullName;
            }
            return Type.GetType (fqn);
        }
    }

}
