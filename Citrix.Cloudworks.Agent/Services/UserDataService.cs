using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.Win32;

using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent.Services {
    
    /// <summary>
    /// This class processes &lt;script&gt; and &lt;powershell&gt; elements from user-data. 
    /// </summary>
    public class UserDataService {
   
        private const string DhcpServerPlaceholder = "{dhcp}";
        private const string DefaultUserDataUrl = "http://{dhcp}/latest/user-data";
        private const string DefaultCfnFolder = "C:\\cfn";

        private string userDataUrl;
        private string cfnFolder;
        private bool stopSignalled;
        private UserDataState stateService;

        #region Constructors

        public UserDataService() {
            
            userDataUrl = ConfigurationManager.AppSettings["user-data-url"];
            if (string.IsNullOrEmpty(userDataUrl)) {
                userDataUrl = DefaultUserDataUrl;
            }

            cfnFolder = ConfigurationManager.AppSettings["cfn-folder"];
            if (string.IsNullOrEmpty(cfnFolder)) {
                cfnFolder = DefaultCfnFolder;
            }
            stateService = new UserDataState(cfnFolder);
            CtxTrace.TraceVerbose("UserData Url {0}, CFN folder {1}", userDataUrl, cfnFolder);
        }

        #endregion

        #region Public Methods

        public void Start() {
            CtxTrace.TraceInformation();
            stopSignalled = false;
            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessUserDataTask));
        }

        public void Stop() {
            CtxTrace.TraceInformation();
            stopSignalled = true;
        }

        #endregion

        #region Private Tasks

        private void ProcessUserDataTask(object ignored) {
            CtxTrace.TraceInformation();

            // Check sanity of configuration
            if (userDataUrl.Contains(DhcpServerPlaceholder)) {
                List<IPAddress> dhcpServers = NetworkUtilities.GetDhcpServers();
                if (dhcpServers.Count == 0) {
                    CtxTrace.TraceError("Configured to get UserData from DHCP server, but no DHCP server found");
                    stateService.InitialisationComplete = true;
                }
            }

            // UserData may be a little slow to be delivered
            while (!stateService.InitialisationComplete && !stopSignalled) {
                string userData = GetUserData();
                if (userData != null) {
                    // Script may initiate a reboot, so mark the processing done once userData read.
                    stateService.InitialisationComplete = true;
                    
                    // May be multiple root elements, so wrap for Xml parser
                    string wrappedUserData = string.Format("<dummyRoot>{0}</dummyRoot>", userData);
                    try {
                        XDocument doc = XDocument.Parse(wrappedUserData);
                        XElement script = doc.XPathSelectElement("//script");
                        if (script != null) {
                            ExecuteScript(script.Value);
                        }
                        script = doc.XPathSelectElement("//powershell");
                        if (script != null) {
                            ExecutePowerShellScript(script.Value);
                        }

                    } catch (Exception ex) {
                        CtxTrace.TraceError(ex);
                    }                 
                    break;
                }
                // Sleep a while but ensure timely response to task cancel
                DateTime waitUntil = DateTime.Now + TimeSpan.FromSeconds(5);
                while (!stopSignalled && (DateTime.Now < waitUntil)) {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                }
            }
            CtxTrace.TraceInformation("Task exit");
        }

        #endregion

        #region Private Methods

        private string GetUserData() {
            if (userDataUrl.Contains(DhcpServerPlaceholder)) {
                List<IPAddress> dhcpServers = NetworkUtilities.GetDhcpServers();
                foreach (IPAddress address in dhcpServers) {
                    string userData = NetworkUtilities.HttpGet(userDataUrl.Replace(DhcpServerPlaceholder, address.ToString()));
                    if (userData != null) {
                        return userData;
                    }
                }
                return null;
            } else {
                return NetworkUtilities.HttpGet(userDataUrl);
            }
        }

        private string CreateWorkingDirectory(string baseFolder) {
            string workingDirectory = Path.Combine(baseFolder, "user-data");
            int i = 1;
            while (Directory.Exists(workingDirectory)) {
                 workingDirectory = Path.Combine(baseFolder, string.Format("user-data-{0}",i++));
            }
            ScriptUtilities.CreateDirectory(workingDirectory);
            return workingDirectory;
        }

        private void ExecuteScript(string contents) {
            CtxTrace.TraceInformation();
            try {
                string dir = CreateWorkingDirectory(cfnFolder);
                string fileName = Path.Combine(dir, "user-data.cmd");
                File.WriteAllText(fileName, contents);
                ScriptUtilities.ExecuteProcess("cmd.exe", "/c " + fileName, dir);
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
        }

        private void ExecutePowerShellScript(string contents) {
            CtxTrace.TraceInformation();
            string oldPolicy = null;
            try {
                oldPolicy = ScriptUtilities.SetPowerShellExectionPolicy("Unrestricted");
                string dir = CreateWorkingDirectory(cfnFolder);
                string fileName = Path.Combine(dir, "user-data.ps1");
                File.WriteAllText(fileName, contents);
                ScriptUtilities.ExecuteProcess("powershell.exe", String.Format("-F {0}", fileName), dir);
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            } finally {
                if (oldPolicy != null) {
                    ScriptUtilities.SetPowerShellExectionPolicy(oldPolicy);
                }
            }
        }

        #endregion
    }      
}
