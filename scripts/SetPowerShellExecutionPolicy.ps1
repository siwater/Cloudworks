<#
    Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
This script will set the PowerShell execution policy for the local machine

.DESCRIPTION

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param ( 
    [Parameter(Mandatory=$true)]
    [string]$Policy
)

$ErrorActionPreference = "Stop"

Set-ExecutionPolicy -ExecutionPolicy $Policy -Force