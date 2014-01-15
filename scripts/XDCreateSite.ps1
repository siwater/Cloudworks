<#
    Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
    This script will create a XenDesktop database and site.

.DESCRIPTION
    Once you have installed XenDesktop, you need to create a database and site using this script.

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param (
    [string]$SiteName = "CloudSite",
    [string]$DatabaseServer = ".\SQLEXPRESS",
	[string]$Administrator,
    [string]$LicenseServer,
    [int]$LicenseServerPort = 27000
)

Add-PsSnapIn Citrix.DelegatedAdmin.Admin.V1
Import-Module Citrix.XenDesktop.Admin

New-XDDatabase -AllDefaultDatabases -DatabaseServer $DatabaseServer -SiteName $SiteName  

New-XDSite -AllDefaultDatabases -DatabaseServer $DatabaseServer -SiteName $SiteName

Set-XDLicensing -LicenseServerAddress $LicenseServer -LicenseServerPort $LicenseServerPort

if ($Administrator -ne $null) {
    $admin = Get-AdminAdministrator | Where-Object {$_.Name.ToLower() -eq $Administrator.ToLower()}
    if ($admin -eq $null) {
        $admin = New-AdminAdministrator -Name $Administrator
        Add-AdminRight -Administrator $Administrator -All -Role 'Full Administrator'
    }
}