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
    Write-Log "Starting VDA install"
    New-Item $LogPath -ItemType Directory
    $installargs = "/controllers ""$Controller"" /quiet /components vda,plugins /enable_hdx_ports /optimize /masterimage /baseimage /enable_remote_assistance /noreboot"
    $installer = Join-Path -Path $InstallerPath -ChildPath "XenDesktopVdaSetup.exe"
    Start-ProcessAndWait $installer $installargs
    Write-Log "VDA install completed"
    
    if ($Reboot) {
        Write-Log "Initiating reboot"
        Restart-Computer -Force
    }
}
catch {
   Write-Log "Error attempting to install VDA"
   $error[0]
}
finally {
    Stop-Logging
}