<#
    
Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
This script will create a domain controller for a specified domain

.DESCRIPTION
The script will create an unattended.txt file containing the options in unattended-template.txt merged with the options specified 
on the command line, and run dcpromo to create a domain controller. If an OnBoot parameter has been specified a RunOnce task will be 
created to execute that script block after the next reboot.

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>

Param (
    [string]$DomainName = "stackmate.local",
    [string]$SafeModePassword = "C1tr1xCl0ud", 
    [scriptblock]$OnBoot,    
    [switch]$Reboot = $true 
)

# Get Computer domain name
function Get-DomainName () {
    $computer = Get-WmiObject Win32_ComputerSystem
    return $computer.Domain
}

# Generate the answer file for dcpromo (by adding required parameters to base template)
function Generate-Answer-File ($domain, $safePassword) { 
    $templateFile = "unattend-template.txt" 
    $outFile = "unattend.txt"     
    if (Test-Path -path $templateFile) {           
        if (Test-Path -path $outFile) {
            Remove-Item $outFile
        }
        $lines = (Get-Content $templateFile)      
        foreach ($line in $lines) {          
            Write-Output $line | Out-File $outFile -Append
        }     
        Write-Output "NewDomainDNSName=$domain" | Out-File $outFile -Append
        Write-Output "SafeModeAdminPassword=$safePassword" | Out-File $outFile -Append
    } else {
        $dir = Get-Location
        throw "Cannot locate $templateFile in $dir"
    }
    return $outFile
}

# Use dcpromo to create an Active Directory domain
function Create-Domain ($domain, $safePassword) {
    $current = Get-DomainName 
    if ($domain -ne $current) {
        Write-Output "Creating new domain $domain" 
        $answer = Generate-Answer-File $domain $safePassword      
        dcpromo /unattend:$answer
    }
}

#
# Main.
#
$ErrorActionPreference = "Stop"
$dir = Split-Path -parent $MyInvocation.MyCommand.Path
. "$dir\Utilities.ps1"
Start-Logging
try {
    if ($OnBoot) {
        Schedule-RunOnce $OnBoot
    }
    Create-Domain $DomainName $SafeModePassword
    Write-Output "Created domain $DomainName"
}
finally {
    Stop-Logging
}
if ($Reboot) {
    Restart-Computer
}
 



