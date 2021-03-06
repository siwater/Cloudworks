
<#
    
Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
    The scipt controls auto-logom the the server after boot. 

.DESCRIPTION
    

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param(
    [Parameter(ParameterSetName='Enable')]
    [switch]$Enable,
    [Parameter(ParameterSetName='Disable')]
    [switch]$Disable,
    [Parameter(ParameterSetName="Enable", Mandatory=$true)]
    [string]$UserName,     
    [Parameter(ParameterSetName="Enable", Mandatory=$true)]    
    [string]$Password,
    [Parameter(ParameterSetName="Enable")]    
    [string]$DomainName 
)
$ErrorActionPreference = "Stop"

$regroot = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WinLogon"

if ($Enable) {
    Write-Output "Enabling auto-login for $UserName"    
    $data = @{
        "DefaultUserName"=$UserName;
        "AltDefaultUserName"=$UserName;
        "DefaultPassword"=$Password;
        "AutoAdminLogon"="1"
    } 
    foreach($key in $data.keys) {
        Set-ItemProperty -Path $regroot -Name $key -Value $data[$key]
        if ($DomainName -ne $null) {
            Set-ItemProperty -Path $regroot -Name "CachePrimaryDomain" -Value $DomainName
        }
    }
} elseif ($Disable) {
    Write-Output "Disabling auto-login"    
    Set-ItemProperty -Path $regroot -Name "AutoAdminLogon" -Value "0"
    Remove-ItemProperty -Path $regroot -Name "DefaultPassword"
    Remove-ItemProperty -Path $regroot -Name "DefaultUserName"
    Remove-ItemProperty -Path $regroot -Name "AltDefaultUserName"
}




