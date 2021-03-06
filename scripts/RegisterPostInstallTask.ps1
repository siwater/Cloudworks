<#
    Copyright © 2014 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
 This script will install a XenDesktop meta-installer post processing task

.DESCRIPTION
    ArgList should be comma separated for mutiple arguments

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param (
    [string]$Script
)
$ErrorActionPreference = "Stop"

$regPath = "HKLM:\Software\Citrix\Citrix Desktop Delivery Controller"
if (-not (Test-Path -Path $regPath)) {
    New-Item -Path $regPath | Out-Null
}

Set-ItemProperty -Path $regpath -Name "PostInstallTask" -Value """$Script"""

Write-Output "Installed PostInstallTask $Script"
    
    