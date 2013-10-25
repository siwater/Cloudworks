/*
 * Copyright (c) 2013 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Security.Permissions;
using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {

    internal class ErrorHandler {

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlAppDomain)]
        internal static void Initialize () {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler (OnUnhandledException);
        }

        static void OnUnhandledException (object sender, UnhandledExceptionEventArgs args) {
            CtxTrace.TraceInformation();
            try {
                Exception e = args.ExceptionObject as Exception;
                CtxTrace.TraceError (e.Message + "\n" + e.StackTrace);
                AgentService.StopService();
            } catch (Exception ex) {
                CtxTrace.TraceError (ex);
            }
       }
    }
}
