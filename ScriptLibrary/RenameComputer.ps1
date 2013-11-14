<#
    
Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
This script will rename the computer

.DESCRIPTION

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>

Param (
    [Parameter(Mandatory=$true)]
    [string]$ComputerName,
    [scriptblock]$OnBoot,
    [switch]$Reboot = $true
)
$ErrorActionPreference = "Stop"
$dir = Split-Path -parent $MyInvocation.MyCommand.Path
. "$dir\Utilities.ps1"
Start-Logging
try {
    if ($OnBoot) {
        Schedule-RunOnce $OnBoot
    }
    $computer = Get-WmiObject Win32_ComputerSystem
    if ($computer.Name.ToLower() -ne $ComputerName.ToLower()) {
        $computer.rename($ComputerName)
        Write-Output "Renamed computer $ComputerName"
     } else {
        Write-Output "Computer is already named $ComputerName (continuing)"
     }
}
finally {
    Stop-Logging
}
if ($Reboot) {
    Restart-Computer
}



