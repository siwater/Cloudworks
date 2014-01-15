<#
    
Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
This script will install a XenDesktop VDA on the local machine

.DESCRIPTION
Installs the XenDesktop VDA and Citrix Receiver on the local server. Requires access to the XenDeskop DVD image.

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>

Param (
    [string]$InstallerPath = "D:\x64\XenDesktop Setup",
    [string]$Controller,
    [switch]$Reboot 
)

#
# Main
#
$ErrorActionPreference = "Stop"
Import-Module CloudworksTools
Start-Logging
try { 
    
    
    $installargs = "/controllers ""$Controller"" /quiet /components vda,plugins /enable_hdx_ports /optimize /masterimage /baseimage /enable_remote_assistance /noreboot"
    $installer = Join-Path -Path $InstallerPath -ChildPath "XenDesktopVdaSetup.exe"
    Start-ProcessAndWait $installer $installargs
    
    if ($Reboot) {
        Restart-Computer -Force
    }
} 
finally {
    Stop-Logging
}