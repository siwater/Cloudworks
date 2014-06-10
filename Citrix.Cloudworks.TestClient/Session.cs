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
using System.Linq;
using System.Text;

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
