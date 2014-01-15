<#
    Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
    This script will create a XenDesktop hosting unit connected to a cloud.

.DESCRIPTION
    Create a hosting unit connected to a cloud. Requires XenDesktop snapins Citrix.* to be installed
    (so run it on a XenDesktop controller)

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param (
    [string]$ConnectionName = "CloudConnection",
    [string]$HostingUnitName = "CloudResources",
    [string]$CloudStackUrl,
    [string]$ApiKey,
    [string]$SecretKey,
    [string]$ZoneId,
    [string]$NetworkId,
    [string]$ConnectionType = "CloudPlatform"
)
$ErrorActionPreference = "Stop"
Add-PSSnapin -Name Citrix.*
Import-Module XenDesktopTools

$connection = Get-Item -Path "xdhyp:\Connections\*" | Where-Object {$_.PSChildName -eq $ConnectionName}
if ($connection -eq $null) {
  
   $connection = New-Item -Path "xdhyp:\Connections" -Name $ConnectionName -ConnectionType $ConnectionType -HypervisorAddress $CloudStackUrl -UserName $ApiKey -Password $Secretkey -Persist
    
   New-BrokerHypervisorConnection -HypHypervisorConnectionUid $connection.HypervisorConnectionUid
   
   $zone = Get-AvailabilityZones -ConnectionName $ConnectionName | Where-Object {$_.Id -eq $ZoneId}
   
   $network = Get-Networks -ConnectionName $ConnectionName | Where-Object {$_.Id -eq $NetworkId}
   
   $hupath = "xdhyp:\HostingUnits\$HostingUnitName"
   $rootpath = "xdhyp:\Connections\$ConnectionName"
   
    New-Item -Path $hupath -HypervisorConnectionName $ConnectionName -AvailabilityZonePath $zone.FullPath -NetworkPath $network.FullPath -RootPath $rootpath
   
   
} else {
    "Connection $ConnectionName already exists"
}

