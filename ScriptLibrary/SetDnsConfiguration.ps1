<#
    Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
This script will 

.DESCRIPTION

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/stackmate
#>
Param ( 
    [Parameter(Mandatory=$true)]
    [string[]]$DnsServers,   
    [string]$MacAddress
)

$this = $MyInvocation.MyCommand.Path
$dir = Split-Path -parent $this
$ErrorActionPreference = "Stop"
. "$dir\Utilities.ps1"

$filter = "IPEnabled=true"
if (-not [string]::IsNullOrEmpty($MacAddress)) {
    $filter += " and MacAddress='$MacAddress'"
}

$adapters = Get-WmiObject -Class Win32_NetworkAdapterConfiguration -Filter $filter 

foreach ($adapter in $adapters) {
    $result = $adapter.SetDNSServerSearchOrder($DnsServers)
}