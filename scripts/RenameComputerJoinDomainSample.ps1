<#
    
Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
Sample Cloudworks script to join an instance to an existing domain.

.DESCRIPTION
This is a sample script to illustrate how to use the Cloudworks PowerShell tools to change the IP configuration of a server
to use the new Domain Controller as Dns Server, then rename the computer and join it to the specified domain.

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param (
    [ValidatePattern('^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$')]
    [Parameter(Mandatory=$true)]
    [string]$DcIpAddress,
    [string]$ComputerName = "DomainMember01"
)
.\SetDnsConfiguration.ps1 -DnsServers $DcIpAddress
.\RenameComputer.ps1 -ComputerName $ComputerName  -OnBoot {
    .\JoinDomain.ps1 -DomainName "stackmate.local" -UserName Administrator -Password "1pass@word1" -OnBoot {
        Write-Output "Domain join complete" 
        Write-Output "Now signal operation complete with cfn-signal" 
    }
}
