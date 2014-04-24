<#
    Copyright © 2014 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
This script will mount the specified ISO and also install a script to run at boot time to re-mount the ISO. 

.DESCRIPTION

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param ( 
    [Parameter(Mandatory=$true)]
    [string]$IsoPath
)

$ErrorActionPreference = "Stop"

$command = 'C:\Program Files (x86)\Elaborate Bytes\VirtualCloneDrive\VCDMount.exe' 
 
& $command $IsoPath
schtasks.exe /create /tn 'IsoMount' /sc onstart /tr "$command $IsoPath" /ru SYSTEM