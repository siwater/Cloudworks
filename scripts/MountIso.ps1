<#
    Copyright © 2014 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
 This script will mount the specified ISO

.DESCRIPTION

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param ( 
    [string]$IsoPath,
    [switch]$Eject = $false
)
$ErrorActionPreference = "Stop"
$args = ""
if ($Eject) {
    $args = "/u"
}
&'C:\Program Files (x86)\Elaborate Bytes\VirtualCloneDrive\VCDMount.exe' $args $IsoPath


 