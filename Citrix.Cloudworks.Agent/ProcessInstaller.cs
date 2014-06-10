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
 * 
 */
using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Citrix.Cloudworks.Agent {


    [RunInstaller(true)]
    public class ProcessInstaller : Installer {

        private ServiceProcessInstaller _serviceProcessInstaller;

        private ServiceInstaller _MonitorServiceInstaller;

        public ProcessInstaller() {
            _serviceProcessInstaller = new ServiceProcessInstaller();
            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            Installers.Add(_serviceProcessInstaller);

            _MonitorServiceInstaller = new ServiceInstaller();
            _MonitorServiceInstaller.StartType = ServiceStartMode.Automatic;
            _MonitorServiceInstaller.ServiceName = AgentService.Name;
            _MonitorServiceInstaller.DisplayName = AgentService.DisplayName;
            _MonitorServiceInstaller.Description = AgentService.Description;
            Installers.Add(_MonitorServiceInstaller);
        }

        protected override void OnAfterInstall(System.Collections.IDictionary savedState) {
            base.OnAfterInstall(savedState);
            ServiceController ecm = new ServiceController(AgentService.Name);
            ecm.Start();
        }
    }
}
