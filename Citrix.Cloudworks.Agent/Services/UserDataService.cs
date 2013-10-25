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
using System.Threading.Tasks;
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
        private CancellationTokenSource cancellationTokenSource;
        private Task initTask;

        #region Constructors

        public UserDataService() {

            userDataUrl = ConfigurationManager.AppSettings["user-data-url"];
            if (string.IsNullOrEmpty(userDataUrl)) {
                userDataUrl = DefaultUserDataUrl;
            }
            if (userDataUrl.Contains(DhcpServerPlaceholder)) {
                List<IPAddress> dhcpServers = NetworkUtilities.GetDhcpServers();
            }

            cfnFolder = ConfigurationManager.AppSettings["cfn-folder"];
            if (string.IsNullOrEmpty(cfnFolder)) {
                cfnFolder = DefaultCfnFolder;
            }
            CtxTrace.TraceVerbose("UserData Url {0}, CFN folder {1}", userDataUrl, cfnFolder);
        }

        #endregion

        #region Public Methods

        public void Start() {
            CtxTrace.TraceInformation();
            cancellationTokenSource = new CancellationTokenSource();
            initTask = Task.Factory.StartNew(() => ProcessUserDataTask(), cancellationTokenSource.Token);
        }

        public void Stop() {
            CtxTrace.TraceInformation();
            cancellationTokenSource.Cancel();
        }

        #endregion

        #region Private Tasks

        private void ProcessUserDataTask() {
            CtxTrace.TraceInformation();

            // Check sanity of configuration
            if (userDataUrl.Contains(DhcpServerPlaceholder)) {
                List<IPAddress> dhcpServers = NetworkUtilities.GetDhcpServers();
                if (dhcpServers.Count == 0) {
                    CtxTrace.TraceError("Configured to get UserData from DHCP server, but no DHCP server found");
                    UserDataState.InitialisationComplete = true;
                }
            }

            // UserData may be a little slow to be delivered
            while (!UserDataState.InitialisationComplete && !cancellationTokenSource.Token.IsCancellationRequested) {
                string userData = GetUserData();
                if (userData != null) {
                    try {
                        XDocument doc = XDocument.Parse(userData);
                        XElement script = doc.XPathSelectElement("//script");
                        if (script != null) {
                            ExecuteScript(script.Value);
                        } else {
                            script = doc.XPathSelectElement("//powershell");
                            if (script != null) {
                                ExecutePowerShellScript(script.Value);
                            }
                        }
                    } catch (Exception ex) {
                        CtxTrace.TraceError(ex);
                    }
                    UserDataState.InitialisationComplete = true;
                    break;
                }
                // Sleep a while but ensure timely response to task cancel
                DateTime waitUntil = DateTime.Now + TimeSpan.FromSeconds(5);
                while (!cancellationTokenSource.Token.IsCancellationRequested && (DateTime.Now < waitUntil)) {
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

        private string CreateTempDirectory(string baseFolder) {         
            string tempDirectory = Path.Combine(baseFolder, Guid.NewGuid().ToString());
            ScriptUtilities.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        private void ExecuteScript(string contents) {
            CtxTrace.TraceInformation();
            try {
                string dir = CreateTempDirectory(cfnFolder);
                string fileName = Path.Combine(dir, "user-data.cmd");
                File.WriteAllText(fileName, contents);
                ScriptUtilities.ExecuteProcess(fileName, null, dir);
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            }
        }

        private void ExecutePowerShellScript(string contents) {
            CtxTrace.TraceInformation();
            RegistryView view = RegistryView.Registry32;
            string oldPolicy = null;
            try {
                oldPolicy = ScriptUtilities.SetPowerShellExectionPolicy("Unrestricted", view);
                string dir = CreateTempDirectory(cfnFolder);
                string fileName = Path.Combine(dir, "user-data.ps1");
                File.WriteAllText(fileName, contents);
                ScriptUtilities.ExecuteProcess("powershell.exe", String.Format("-F {0}", fileName), dir);
            } catch (Exception e) {
                CtxTrace.TraceError(e);
            } finally {
                if (oldPolicy != null) {
                    ScriptUtilities.SetPowerShellExectionPolicy(oldPolicy, view);
                }
            }
        }

        #endregion
    }      
}
