<#
    
Copyright © 2014 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
This script will upload CA certificates to the Netscaler and enable the required features for Access Gateway.
It assumes the NS has been allocated an NSIP and the initial (web) configuration wizard has been run (i.e. SNIP, DNS, 
host name, licensing have been taken care of). 

.DESCRIPTION

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param (
    [Parameter(Mandatory=$true)]
    [string]$NsIp,
    [string]$NsUser = "nsroot",
    [string]$NsPassword = "nsroot"
)

function Install-PuTTY {   

    $file = "putty-0.63-installer.exe"
    $Url = "https://s3.amazonaws.com/citrix-cloudworks/$file"
    $installer = "$Env:Temp\$file"

    try {

        (New-Object System.Net.WebClient).DownloadFile($Url, $installer)
    } catch {
        Write-Error $error[0]
    }
    Start-ProcessAndWait -FilePath $installer -ArgumentList "/silent"

    Remove-Item -Path $installer
 }
 
$ErrorActionPreference = "Stop"
Import-Module CloudworksTools

$putty = "C:\Program Files (x86)\PuTTY"
if (-not (Test-Path $putty)) {
    Install-PuTTY
}  
$here = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$nshost = "${NsUser}@${NsIp}"

$script = @"
#
# Netscaler configuration script for one time setup of the VPX
#
# Enable required features
enable ns feature CMP SSL SSLVPN

# Install and link the Citrix certficate chain
add ssl certKey CITRITEIssuingCA01 -cert "/nsconfig/ssl/CITRITEIssuingCA01.cer"
add ssl certKey CITRITEPolicyCA -cert "/nsconfig/ssl/CITRITEPolicyCA.cer"
add ssl certKey CITRIXRoot -cert "/nsconfig/ssl/CITRIXRootCA.cer"

link ssl certKey CITRITEIssuingCA01 CITRITEPolicyCA
link ssl certKey CITRITEPolicyCA CITRIXRoot

set vpn parameter -defaultAuthorizationAction ALLOW -proxy BROWSER -forceCleanup none -clientOptions all -clientConfiguration all -SSO ON -icaProxy ON -wiPortalMode NORMAL -clientlessVpnMode ON -clientlessPersistentCookie ALLOW -UITHEME GREENBUBBLE

"@

$scriptFile = [IO.Path]::GetTempFileName()
$script | Out-File -Encoding ASCII $scriptFile


# Accept server certificate
echo y | &$putty\plink -pw $NsPassword $nshost exit | Out-Null

&$putty\pscp -pw $NsPassword convert-eol.sh "${nshost}:/root"
&$putty\plink -batch -pw $NsPassword $nshost shell chmod a+x convert-eol.sh

&$putty\pscp -pw $NsPassword $scriptFile "${nshost}:/root/one-time-setup.nscmd"
&$putty\plink -batch -pw $NsPassword $nshost shell ./convert-eol.sh one-time-setup.nscmd
&$putty\plink -batch -pw $NsPassword $nshost source one-time-setup.nscmd

Remove-Item -Path $scriptFile