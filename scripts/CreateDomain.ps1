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
    [string]$DomainName ,
    [string]$SafeModePassword, 
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

# Create an Active Directory domain
function Create-Domain ($domain, $safePassword) {
    $current = Get-DomainName 
    if ($domain -ne $current) {
        Write-Output "Creating new domain $domain ..."
        $os = Get-OsName
        if ($os.Contains("2012")) {
             Write-Output "...using AD-Domain-Servces"
             Install-WindowsFeature AD-Domain-Services -IncludeManagementTools
             $securePassword = ConvertTo-SecureString $safePassword -AsPlainText -Force 
             Install-ADDSForest -DomainName $domain -SafeModeAdministratorPassword $securePassword -Confirm:$false -NoRebootOnCompletion
        } else {
            Write-Output "...using dcpromo"
            $answer = Generate-Answer-File $domain $safePassword  
            dcpromo /unattend:$answer
        }
    }
}

#
# Main.
#
$ErrorActionPreference = "Stop"
Import-Module CloudworksTools

Start-Logging
try {
    Create-Domain $DomainName $SafeModePassword
    Write-Output "Created domain $DomainName"
    if ($OnBoot) {
        New-RunOnceTask $OnBoot
    }
}
finally {
    Stop-Logging
}
if ($Reboot) {
    Restart-Computer -Force
}
 



