/*
 * Copyright (c) 2013 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {

    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            CtxTrace.Initialize("cloudworks-agent", true);
            ErrorHandler.Initialize();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new AgentService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
