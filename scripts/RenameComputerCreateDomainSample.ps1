<#
    
Copyright © 2013 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
Sample Cloudworks script to create a domain controller.

.DESCRIPTION
This is a sample script to illustrate how to use the Cloudworks PowerShell tools to rename a newly created 
CloudPlatform instance and promote it to be a domain controller.

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
.\RenameComputer.ps1 -ComputerName DC01 -OnBoot {
    .\CreateDomain.ps1 -DomainName "cloudworks.local" -OnBoot {
        Write-Output "Domain creation complete" 
        Write-Output "Now notify operation complete with cfn-signal" 
    }
}
