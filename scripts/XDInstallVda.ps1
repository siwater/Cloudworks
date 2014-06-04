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
    [string]$InstallerPath = "E:\x64\XenDesktop Setup",
    [string]$LogPath = (Join-Path -Path $ENV:windir -ChildPath "Temp\Citrix\XenDesktop Installer"),
    [string]$Controller,
    [switch]$Reboot,
    [switch]$NoContinue
)

#
# Main
#
$ErrorActionPreference = "Stop"
Import-Module CloudworksTools
Start-Logging

$runOnceKey = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce"
if (-not (Test-Path $LogPath)) {
    New-Item -Path $LogPath -ItemType Directory
}

try {
    Write-Log "Starting VDA install"
    $installargs = "/controllers ""$Controller"" /quiet /components vda,plugins /enable_hdx_ports /optimize /masterimage /baseimage /enable_remote_assistance /noreboot /logpath '$LogPath'"
    $installer = Join-Path -Path $InstallerPath -ChildPath "XenDesktopVdaSetup.exe"
    Start-ProcessAndWait $installer $installargs
    Write-Log "VDA install completed"
    
    if ($NoContinue) {
        try {
            Remove-ItemProperty -Path $runOnceKey -Name "XenDesktopSetup"
        } catch {
            Write-Log "Error removing RunOnce key"
            $error[0]
        }
    }
    
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