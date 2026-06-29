param(
    [Parameter(Mandatory = $true)]
    [string]$UnityProjectPath
)

$ErrorActionPreference = "Stop"
$KitRoot = Resolve-Path (Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "..")
$AssetsRoot = Join-Path $UnityProjectPath "Assets"

if (-not (Test-Path $AssetsRoot)) {
    throw "UnityProjectPath must point to a Unity project folder containing Assets/: $UnityProjectPath"
}

$TargetRoot = Join-Path $AssetsRoot "CatLife\UIAssemblyKit"
New-Item -ItemType Directory -Force -Path $TargetRoot | Out-Null

Copy-Item -LiteralPath (Join-Path $KitRoot "assets") -Destination $TargetRoot -Recurse -Force
Copy-Item -LiteralPath (Join-Path $KitRoot "layout") -Destination $TargetRoot -Recurse -Force
Copy-Item -LiteralPath (Join-Path $KitRoot "unity-scripts") -Destination $TargetRoot -Recurse -Force

Write-Host "Installed CatLife UI kit to $TargetRoot"
Write-Host "Next: in Unity, import assets, create Canvas 1080x1920, then follow README.md."
