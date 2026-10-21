# Starts the ABC Pharmacy API and React client together (Windows).
# Usage: double-click run.cmd    Press Ctrl+C to stop both.
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { Write-Host 'Please install the .NET 10 SDK first.'; exit 1 }
if (-not (Get-Command npm -ErrorAction SilentlyContinue)) { Write-Host 'Please install Node.js first.'; exit 1 }

$api = Start-Process dotnet -ArgumentList 'run', '--project', 'PharmacyApi', '--launch-profile', 'http' -NoNewWindow -PassThru
try {
    Write-Host 'Waiting for the API to start...'
    while ($true) {
        try { Invoke-WebRequest http://localhost:5270/api/medicines -UseBasicParsing | Out-Null; break } catch { Start-Sleep 1 }
    }

    Set-Location pharmacy-client
    if (-not (Test-Path node_modules)) { npm install }
    npm run dev -- --open
}
finally {
    # /T also stops the API process that 'dotnet run' spawns.
    taskkill /PID $api.Id /T /F | Out-Null
}
