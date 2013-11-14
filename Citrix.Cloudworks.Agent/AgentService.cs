/*
 * Copyright (c) 2013 Citrix Systems, Inc. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {

    public partial class AgentService : ServiceBase {

        public AgentService() {
            InitializeComponent();
        }

        public static string DisplayName {
            get {
                return "Citrix StackMate Agent";
            }
        }

        public static string Name {
            get {
                return "CtxCwSvc";
            }
        }

        public static string Description {
            get {
                return "Windows agent for Citrix StackMate";
            }
        }

        public static void StopService() {
            ServiceController ctl = new ServiceController(Name);
            ctl.Stop();
        }

        private CloudworksServices _Service;

        protected override void OnStart(string[] args) {
            CtxTrace.TraceInformation();
            _Service = new CloudworksServices();
            _Service.Start();
           
        }

        protected override void OnStop() {
            CtxTrace.TraceInformation();
            if (_Service != null) {
                _Service.Stop();
            }
        }
    }
}
