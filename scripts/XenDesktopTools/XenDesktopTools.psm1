<#
    Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
    XenDesktop Inventory Item query tools

.DESCRIPTION
    

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>

function Get-Networks {
    Param(
        [string]$ConnectionName
    )
    return Get-InventoryItem $ConnectionName "Network"
}

function Get-AvailabilityZones {
    Param(
        [string]$ConnectionName
    )
    return Get-InventoryItem $ConnectionName "AvailabilityZone"
}

function Get-Templates {
    Param(
        [string]$ConnectionName
    )
    return Get-InventoryItem $ConnectionName "Template"
}

function Get-ServiceOfferings {
    Param(
        [string]$ConnectionName
    )
    return Get-InventoryItem $ConnectionName "ServiceOffering"
}

function Get-InventoryItem {
    Param(
        [string]$ConnectionName,
        [string]$ItemType
    )
    $path = "xdhyp:\Connections\${ConnectionName}"
    Get-ChildItem -Path $path -Recurse | Where-Object {$_.ObjectType -eq $ItemType}
}


Add-PSSnapin -Name Citrix.Host.Admin.v2
Export-ModuleMember -Function Get-Networks, Get-AvailabilityZones, Get-Templates, Get-ServiceOfferings, Get-InventoryItem