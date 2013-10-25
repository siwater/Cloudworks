/*
 * Copyright (c) 2013 Citrix Systems, Inc. All Rights Reserved.
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
