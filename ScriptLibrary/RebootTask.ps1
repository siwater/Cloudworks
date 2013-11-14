<#

Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
Internal reboot processing script to remove a scheduled task and execute the
[script file containing the] script block specified.

.DESCRIPTION

.NOTES
    KEYWORDS: PowerShell
    REQUIRES: PowerShell Version 2.0

.LINK
     http://community.citrix.com/
#>
Param (
    [string]$Script,
    [string]$Task
)
$ErrorActionPreference = "Continue"
$dir = Split-Path -parent $MyInvocation.MyCommand.Path
. "$dir\Utilities.ps1"
Start-Logging
try {
    Write-Output "Deleting RunOnce task $Task"
    Delete-Task $Task
    Set-Location $dir
    Write-Output "Invoking script $Script"
    Invoke-Expression "$Script" 
    Write-Output "Script $Script completed "
}
catch {
     Write-Output "Caught exception"
     $error[0]
}
finally {
    Write-Output "End of reboot task"
    Stop-Logging
}