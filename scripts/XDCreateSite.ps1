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
	[string[]]$Administrators,
    [string]$LicenseServer,
    [int]$LicenseServerPort = 27000
)

function Set-LicenseServer {
    Param (
        [string]$LSAddress,
        [int]$LSPort 
    )
    Set-XDLicensing -LicenseServerAddress $LSAddress -LicenseServerPort $LSPort
    try  {
        $location = Get-LicLocation  -AddressType 'WSL' -LicenseServerAddress $LSAddress -LicenseServerPort $LSPort
        $certificate = Get-LicCertificate  -AdminAddress $location
        Set-ConfigSiteMetadata -Name 'CertificateHash' -Value $certificate.CertHash
    } catch {
        Write-Error "Failed to set license server"
        Write-Error $error[0]
    }
}

Add-PsSnapIn -Name Citrix.*
Import-Module Citrix.XenDesktop.Admin

New-XDDatabase -AllDefaultDatabases -DatabaseServer $DatabaseServer -SiteName $SiteName  

New-XDSite -AllDefaultDatabases -DatabaseServer $DatabaseServer -SiteName $SiteName

Set-LicenseServer $LicenseServer $LicenseServerPort

if ($Administrators -ne $null) {
    foreach ($Administrator in $Administrators) {
        $admin = Get-AdminAdministrator | Where-Object {$_.Name.ToLower() -eq $Administrator.ToLower()}
        if ($admin -eq $null) {
            try {
                $admin = New-AdminAdministrator -Name $Administrator
                Add-AdminRight -Administrator $Administrator -All -Role 'Full Administrator'
            } catch {
                Write-Error "Failed to add administrator $Administrator"
                Write-Error $error[0]
            }
        }
    }
}