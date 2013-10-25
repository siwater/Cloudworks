using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citrix.Cloudworks.TestClient {
    
    class TestCloudworks {

        private const string TemplateIdKeyPrefix = "template-id-";
        private const string PowerShellPrefix = "powershell-";
        private const string ScriptPrefix = "script-";

        private string cloudStackUrl;

        private string apiKey;
        private string secretKey;

        private string networkId;
        private string zoneId;
        private string serviceOfferingId;

        private Session session;

        public TestCloudworks() {
            cloudStackUrl = ConfigurationManager.AppSettings["cloudstack-url"];

            apiKey = ConfigurationManager.AppSettings["cloudstack-api-key"];
            secretKey = ConfigurationManager.AppSettings["cloudstack-secret-key"];

    
            networkId = ConfigurationManager.AppSettings["network-id"];
            zoneId = ConfigurationManager.AppSettings["zone-id"];
            serviceOfferingId = ConfigurationManager.AppSettings["service-offering-id"];

            session = new Session(cloudStackUrl, apiKey, secretKey);
        }

        public void RunTests() {
            foreach (KeyValuePair<string, string> template in GetAppSettings("template")) {
                //RunTest(template, "script");
                RunTest(template, "powershell");
            }
        }

        private void RunTest(KeyValuePair<string, string> template, string prefix) {

            foreach (KeyValuePair<string, XElement> script in GetUserData(prefix)) {
                string name = template.Key + "-" + script.Key;
                try {
                    string id = session.CreateVirtualMachine(name, networkId, zoneId, serviceOfferingId, template.Value, script.Value.ToString());
                    Console.WriteLine("Created test vm {0} with id {1}", name, id);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }             
            }
        }

        private IEnumerable<KeyValuePair<string, XElement>> GetUserData(string prefix) {
            return GetAppSettings(prefix).Select(keyPair => {
                XElement e = new XElement(prefix, File.ReadAllText(keyPair.Value));
                return new KeyValuePair<string, XElement>(keyPair.Key, e);
            });     
        }

        private IEnumerable<KeyValuePair<string, string>> GetAppSettings(string prefix) {
            return GetAppSettingKeys(prefix).Select(key => 
                new KeyValuePair<string, string>(key, ConfigurationManager.AppSettings[key]));
        }

        /// <summary>
        /// Get an ordered list of the AppSettings keys with the specified prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private IEnumerable<string> GetAppSettingKeys(string prefix) {
            return ConfigurationManager.AppSettings.AllKeys.
                Where(key => key.StartsWith(prefix)).
                OrderBy(key => key);
        }
    }
}
