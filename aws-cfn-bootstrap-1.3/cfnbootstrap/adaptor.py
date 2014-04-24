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

import logging
import os
import requests
import sys
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
        self.server = server
        self.baseUrl = 'http://{0}/latest/meta-data/'.format(server)

    # Content used for CFN_V1 authentication header to CFN server. This impl is temporary
    def instance_identity_document_url(self):
        return self.baseUrl + 'instance-id'

    # Content used for CFN_V1 authentication header to CFN server. This impl is temporary.
    def instance_identity_signature_url(self):
        return self.baseUrl + 'instance-id'

    def instance_id_url(self):
        return self.baseUrl + 'instance-id'

    # Unused ?
    def iam_security_credentials_url(self, name):
        return self.baseUrl + 'meta-data/iam/security-credentials/%s' % name

    # CFN Server address: a DescribeStackResource will be made to this Url.
    # The Url should be provided in the environment
    def region_url(self, region):
        var = 'StackMateApiUrl'
        try: 
            return os.environ[var]
        except KeyError:
            print >> sys.stderr, "%s not set" % var
            return self.baseUrl + 'region/%s' % region

    def region_url_regex(self):
        return r'{0}region/([\w\d-]+)'.format( self.baseUrl)

# Adaptor for local testing (requires StackMate emulator)
class TestAdaptor:

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
    if sys.platform == "linux" or sys.platform == "linux2":
        return get_dhcp_servers_Linux()
    elif sys.platform == "win32":
        return get_dhcp_servers_Windows()
    raise Exception("Unrecognised sys.platform: %" % sys.platform)

def get_dhcp_servers_Windows():
    """Windows specific code to get a list of all the DHCP servers configured for the system"""
    result = []
    wmi = win32com.client.Dispatch("WbemScripting.SWbemLocator")
    cim = wmi.ConnectServer(".", "root\cimv2")
    adaptors = cim.ExecQuery("SELECT * from Win32_NetworkAdapterConfiguration where IPEnabled = 'true'")
    for adaptor in adaptors:
        if adaptor.DHCPServer != None:
            result.append(adaptor.DHCPServer)
    return result

def get_dhcp_servers_Linux():
    """Linux specific code to get a list of all the DHCP servers configured for the system"""
    result = []

    p=subprocess.Popen("cat /var/lib/dhcp/dhclient.eth0.leases | grep dhcp-server-identifier",stderr=subprocess.PIPE,stdout=subprocess.PIPE,shell=True)
    out, err = p.communicate()

    if out == '':
        p=subprocess.Popen("cat /var/lib/dhclient/dhclient-eth0.leases | grep dhcp-server-identifier",stderr=subprocess.PIPE,stdout=subprocess.PIPE,shell=True)
        out, err = p.communicate()

    adaptors=out.split()
    for adaptor in adaptors:
        m=re.match(".*\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$",adaptor.rstrip(';'))
        if m is not None:
            result.append(m.group())
    return result



def can_get(url):
    """Return true if the URL is accessible via GET."""
    try:
        resp = requests.get(url)
        return True
    except IOError:
       return False


Adaptor = None

for dhcp_server in get_dhcp_servers():
   if can_get("http://%s/latest/meta-data" % dhcp_server):
       Adaptor = CloudPlatformAdaptor(dhcp_server)
       break

# Use test adaptor if not in CS environment
if Adaptor == None:
    Adaptor = TestAdaptor("10.70.176.50")
    ##raise Exception("No UserData service found")





