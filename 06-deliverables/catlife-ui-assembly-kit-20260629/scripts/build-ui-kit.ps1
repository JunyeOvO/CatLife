param(
    [string]$Python = "C:\Users\fujunye\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe"
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$Generator = Join-Path $ScriptDir "generate-ui-kit.py"

if (-not (Test-Path $Python)) {
    $Python = "python"
}

& $Python $Generator
if ($LASTEXITCODE -ne 0) {
    throw "UI kit generation failed."
}

Write-Host "CatLife UI assembly kit generated."
