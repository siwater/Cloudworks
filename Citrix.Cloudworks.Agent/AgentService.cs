/*
 * Copyright (c) 2013-2014 Citrix Systems, Inc.
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
                return "Citrix Cloudworks Agent";
            }
        }

        public static string Name {
            get {
                return "CtxCwSvc";
            }
        }

        public static string Description {
            get {
                return "Windows agent for Citrix Cloudworks";
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
