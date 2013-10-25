/*
 * Copyright (c) 2013 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {

    using Services;

    /// <summary>
    /// This is the container for the various services run inside the StackMate Agent, It can be executed in a range 
    /// of contexts (console app, windows service etc)
    /// </summary>
    public class CloudworksServices {

        private UserDataService userDataService;

        public void Start() {
            CtxTrace.TraceInformation();
            try {
                userDataService = new UserDataService();
                userDataService.Start();
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
        }

        public void Stop() {
            CtxTrace.TraceInformation();
            try {
                userDataService.Stop();
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
        }
    }
}
