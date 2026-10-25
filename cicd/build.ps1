<#
.SYNOPSIS
    Baut PaymentBackend.Functions und erzeugt ein deploybares Zip fuer Azure Functions.

.DESCRIPTION
    Fuehrt 'dotnet publish' fuer das Function-Projekt aus und packt das Ergebnis
    nach cicd/build/PaymentBackend.Functions.zip. Das Zip kann direkt per Zip-Deploy
    hochgeladen werden, z.B.:
        az functionapp deployment source config-zip -g <rg> -n <app> --src cicd/build/PaymentBackend.Functions.zip

.PARAMETER Configuration
    Build-Konfiguration (Standard: Release).
#>
[CmdletBinding()]
param(
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$repoRoot   = Split-Path -Parent $PSScriptRoot
$project    = Join-Path $repoRoot 'PaymentBackend.Functions/PaymentBackend.Functions.csproj'
$buildDir   = Join-Path $PSScriptRoot 'build'
$publishDir = Join-Path $buildDir 'publish'
$zipPath    = Join-Path $buildDir 'PaymentBackend.Functions.zip'

Write-Host "==> Bereinige $buildDir"
if (Test-Path $buildDir) {
    Remove-Item $buildDir -Recurse -Force
}
New-Item -ItemType Directory -Path $publishDir -Force | Out-Null

Write-Host "==> dotnet publish ($Configuration)"
dotnet publish $project --configuration $Configuration --output $publishDir
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish fehlgeschlagen (ExitCode $LASTEXITCODE)."
}

# ZipFile statt Compress-Archive: Compress-Archive ueberspringt versteckte Dateien
# und schreibt unter Windows PowerShell Backslashes in die Pfade, was auf Linux-Hosts bricht.
Write-Host "==> Erzeuge $zipPath"
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $publishDir,
    $zipPath,
    [System.IO.Compression.CompressionLevel]::Optimal,
    $false)

$sizeMb = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
Write-Host "==> Fertig: $zipPath ($sizeMb MB)" -ForegroundColor Green
