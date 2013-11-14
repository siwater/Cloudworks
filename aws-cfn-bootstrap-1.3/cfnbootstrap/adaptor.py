#==============================================================================
# Copyright 2013 Citrix Systems, Inc. All Rights Reserved.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#==============================================================================
import requests
import win32com.client

class AmazonAdaptor:

    def instance_identity_document_url(self):
        return 'http://169.254.169.254/latest/dynamic/instance-identity/document'

    def instance_identity_signature_url(self):
        return 'http://169.254.169.254/latest/dynamic/instance-identity/signature'

    def instance_id_url(self):
        return 'http://169.254.169.254/latest/meta-data/instance-id'

    def iam_security_credentials_url(self, name):
        return 'http://169.254.169.254/latest/meta-data/iam/security-credentials/%s' % name

    def region_url(self, region):
        return 'https://cloudformation.%s.amazonaws.com' % region

    def region_url_regex(self):
        return r'https://cloudformation.([\w\d-]+).amazonaws.com'

class CloudPlatformAdaptor:

    def __init__(self, server):
        self.baseUrl = 'http://{0}/stackmate/'.format(server)

    def instance_identity_document_url(self):
        return self.baseUrl + 'instance-identity/document'

    def instance_identity_signature_url(self):
        return self.baseUrl + 'instance-identity/signature'

    def instance_id_url(self):
        return self.baseUrl + 'meta-data/instance-id'

    def iam_security_credentials_url(self, name):
        return self.baseUrl + 'meta-data/iam/security-credentials/%s' % name

    def region_url(self, region):
        return self.baseUrl + 'region/%s' % region

    def region_url_regex(self):
        return r'{0}region/([\w\d-]+)'.format( self.baseUrl)

def get_dhcp_servers():
    """Get a list of all the DHCP servers configured for the system"""
    result = []
    wmi = win32com.client.Dispatch("WbemScripting.SWbemLocator")
    cim = wmi.ConnectServer(".", "root\cimv2")
    adaptors = cim.ExecQuery("SELECT * from Win32_NetworkAdapterConfiguration where IPEnabled = 'true'")
    for adaptor in adaptors:
        if adaptor.DHCPServer != None:
            result.append(adaptor.DHCPServer)
    return result

def can_get(url):
    """Return true if the URL is accessible via GET."""
    try:
        resp = requests.get(url)
        return True
    except IOError:
       return False

def is_ec2():
    """Try to figure out if this is an EC2 instance by looking for the userdata/metadata server."""
    return can_get("http://169.254.169.254/latest")

Adaptor = None

##if (is_ec2()):
##    Adaptor = AmazonAdaptor()
##else:
##   for dhcp_server in get_dhcp_servers():
##        if can_get("http://%s/latest/user-data"):
##            Adaptor = CloudPlatformAdaptor(dhcp_server)

# Use test server
if Adaptor == None:
    Adaptor = CloudPlatformAdaptor("10.70.176.50")
    ##raise Exception("No UserData service found")





