<#
    
Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
Sample StackMate script to join an instance to an existing domain.

.DESCRIPTION
This is a sample script to illustrate how to use the StackMate tools to change the IP configuration of a server
to use a specified Dns Server, then rename the computer and join it to the specified domain.

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
Param (
     [ValidatePattern('^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$')]
     [Parameter(Mandatory=$true)]
    [string]$DcIpAddress
)
.\SetDnsConfiguration.ps1 -DnsServers $DcIpAddress
.\RenameComputer.ps1 -ComputerName Member01 -OnBoot {
    .\JoinDomain.ps1 -DomainName "stackmate.local" -UserName Administrator -Password "1pass@word1" -OnBoot {
        Write-Output "Domain join complete" 
        Write-Output "Now signal operation complete to StackMate" 
    }
}
