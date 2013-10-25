/*
 * Copyright (c) 2006 - 2007 Citrix Systems, Inc. All Rights Reserved.
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
