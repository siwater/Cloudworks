<#
    Copyright © 2013/2014 Citrix Systems, Inc. All rights reserved.

.SYNOPSIS
   Cloudworks tools

.DESCRIPTION
    

.NOTES
    KEYWORDS: PowerShell, Citrix
    REQUIRES: PowerShell Version 2.0 

.LINK
     http://community.citrix.com/
#>
function Start-Logging {
    $logdir = (Split-Path -parent $MyInvocation.ScriptName) + "\logs\"
    if (-not (Test-Path $logdir)) {
        $null = New-Item -Path $logdir -ItemType directory
    } 
    $logFile = $logdir + (Get-Date -Format "yyyyMMddHHmmss") + ".log"
    try {
        Start-Transcript -Path $logFile
    } catch [System.InvalidOperationException] {
       # Transcript is already running (ignore)
    }
}

function Stop-Logging {
    Stop-Transcript
}

function Write-Log {
    $time = Get-Date -Format "yyyy-MM-dd HH:mm:ss" 
    Write-Output "${time}: $args"
}

function Create-PsFileName {
    Param(
        [string]$Folder,
        [string]$NameBase
    )
    
    $base = "${Folder}\$NameBase"
    $i = 0
    do {
        $file = "$base$i.ps1"
        $i++
    } while ((Test-Path -Path $file))
    return $file      
}

function New-RunOnceTask {
   Param(
        [scriptblock]$OnBoot
    )
    $taskName = "runonce-task"
    $dir = Split-Path -parent $MyInvocation.ScriptName
    $rebootTask = "$dir\RebootTask.ps1"
    if (!(Test-Path -Path $rebootTask)) {
        throw "$rebootTask does not exist"
    }
    $script = Create-PsFileName $dir $taskName
    $OnBoot | Out-File $script
    $command = "powershell.exe -File $rebootTask -Task $taskName -Script '$script' $args"
    schtasks.exe /create /tn $taskName /sc onstart /tr $command /ru SYSTEM
    if (!$?) {
        throw "Failed to schedule reboot task"
    }
}

function Remove-Task([string]$taskName) {
    if (-not [string]::IsNullOrEmpty($taskName)) {
        schtasks /delete /tn $taskName /f
    }
}

function Start-ProcessAndWait {
    Param(
        [string]$FilePath,
        [string[]]$ArgumentList
    )
    if (Test-Path $FilePath) {
        Write-Log "Starting $FilePath $ArgList"
        $process = Start-Process -FilePath $FilePath -ArgumentList $ArgumentList -Wait -PassThru -NoNewWindow
        Write-Log "$filePath exited"
        return $process.ExitCode
    } else {
        Write-Log "Cannot locate $FilePath"
        return -1
    }
}

function Install-Feature {
   Param(
        [string]$FeatureName
    )    
    try {
       Import-Module ServerManager   
       $feature = Get-WindowsFeature | Where-Object {$_.Name -eq $FeatureName}
       if (-not $feature.Installed) {
           Add-WindowsFeature $FeatureName –IncludeAllSubFeature
       }
    } catch {
      Write-Log "Error attempting to install $FeatureName"
      throw
    }
}

function Get-OsName {
    $os = Get-WmiObject Win32_OperatingSystem
    $name = $os.name
    return $name.Substring(0,$name.IndexOf('|')).Trim()
}

Export-ModuleMember -Function Start-Logging, Stop-Logging, Write-Log, New-RunOnceTask, Remove-Task, Start-ProcessAndWait, Get-OsName, Install-Feature
