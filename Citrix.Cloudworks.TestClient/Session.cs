using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudStack.SDK;

namespace Citrix.Cloudworks.TestClient {

    public class Session {

        private Client client;

        public Session(string cloudStackUrl, string apiKey, string secretKey) {
            client = new Client(new Uri(cloudStackUrl), apiKey, secretKey);        
        }

        public string CreateVirtualMachine(string name, string networkId, string zoneId, string serviceOfferingId, string templateId, string userData) {
            try {             
                DeployVirtualMachineRequest request = new DeployVirtualMachineRequest() {
                    DisplayName = name,
                    TemplateId = templateId,
                    ZoneId = zoneId,
                    ServiceOfferingId = serviceOfferingId,
                    UserData = userData
                };
                request.Parameters["name"] = name;
                request.WithNetworkIds(networkId);

                return client.DeployVirtualMachine(request);
               
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }

       
    }
}
