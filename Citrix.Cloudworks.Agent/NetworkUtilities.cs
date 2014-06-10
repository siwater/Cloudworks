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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

using Citrix.Diagnostics;

namespace Citrix.Cloudworks.Agent {
       
    public static class NetworkUtilities {

        /// <summary>
        /// Simple "GET" on the specified Url returing the contents as a string. If an error occurs
        /// it is logged and the method returns null
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url) {
            CtxTrace.TraceVerbose("url={0}", url);
            try {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
                    using (Stream responseStream = response.GetResponseStream()) {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8)) {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            } catch (Exception e) {
                CtxTrace.TraceError(e);
                return null;
            }
        }

        /// <summary>
        /// Get a list of all the DHCP servers configured for this machine. An empty list indicates no
        /// DHCP servers found 
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetDhcpServers() {
            List<IPAddress> result = new List<IPAddress>();
            foreach (NetworkInterface i in NetworkInterface.GetAllNetworkInterfaces()) {
                IPInterfaceProperties properties = i.GetIPProperties();
                IPAddressCollection dhcpServers = properties.DhcpServerAddresses;
                if (dhcpServers != null) {
                    result.AddRange(dhcpServers);
                }
            }
            return result;
        }
    }
}
