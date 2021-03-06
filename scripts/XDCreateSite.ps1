<#
    Copyright © 2013-2014 Citrix Systems, Inc. All rights reserved.
	
	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the
	'Software'), to deal in the Software without restriction, including
	without limitation the rights to use, copy, modify, merge, publish,
	distribute, sublicense, and/or sell copies of the Software, and to
	permit persons to whom the Software is furnished to do so, subject to
	the following conditions:
  
	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.
  
	THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
	TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
	SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


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