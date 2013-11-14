
function Start-Logging() {
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

function Stop-Logging() {
    Stop-Transcript
}

function Write-Log() {
    $time = Get-Date -Format "yyyy-MM-dd HH:mm:ss" 
    Write-Output "$time: $args"
}

function Get-PsFileName($folder, $nameBase) {
    $base = "$folder\$nameBase"
    $i = 0
    do {
        $file = "$base$i.ps1"
        $i++
    } while ((Test-Path -Path $file))
    return $file      
}

function Schedule-RunOnce([scriptblock]$onBoot) {
    $taskName = "runonce-task"
    $dir = Split-Path -parent $MyInvocation.ScriptName
    $rebootTask = "$dir\RebootTask.ps1"
    if (!(Test-Path -Path $rebootTask)) {
        throw "$rebootTask does not exist"
    }
    $script = Get-PsFileName $dir $taskName
    $onBoot | Out-File $script
    $command = "powershell.exe -File $rebootTask -Task $taskName -Script '$script' $args"
    schtasks.exe /create /tn $taskName /sc onstart /tr $command /ru SYSTEM
    if (!$?) {
        throw "Failed to schedule reboot task"
    }
}

function Delete-Task([string]$taskName) {
    if (-not [string]::IsNullOrEmpty($taskName)) {
        schtasks /delete /tn $taskName /f
    }
}
