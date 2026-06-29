# Unity C# 编译检查脚本 v5
# 1. 强制 Unity 重新编译项目
# 2. 等待编译完成
# 3. 解析 Editor.log 输出结果
# 用法: powershell.exe -File build_check.ps1

param(
    [string]$ProjectPath = "C:\Users\35068\My project (2)"
)

$ErrorActionPreference = "Continue"

Write-Host "=== Unity C# 编译检查 ===" -ForegroundColor Cyan
Write-Host "项目: $ProjectPath"
Write-Host ""

$unityExe = "C:\Program Files\Unity 6000.2.5f1\Editor\Unity.exe"
$editorLog = "C:\Users\35068\AppData\Local\Unity\Editor\Editor.log"

if (-not (Test-Path $unityExe)) {
    Write-Host "[ERROR] 找不到 Unity.exe: $unityExe" -ForegroundColor Red
    exit 1
}

# 先清空或备份旧 Editor.log（让新错误更清晰）
if (Test-Path $editorLog) {
    Copy-Item $editorLog "$editorLog.prev" -Force
    # Unity 追加模式，所以我们可以标记"从这里开始是新编译"
}

Write-Host "[1/2] 触发编译（batchmode，无窗口）..." -ForegroundColor Yellow
$compileOut = "$env:TEMP\unity_compile_$PID.log"

$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = $unityExe
$psi.Arguments = @(
    "-batchmode",
    "-projectPath", "`"$ProjectPath`"",
    "-quit",
    "-silentCrashes"
) -join " "
$psi.UseShellExecute = $false
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.CreateNoWindow = $true

$proc = [System.Diagnostics.Process]::Start($psi)
$stdout = $proc.StandardOutput.ReadToEnd()
$stderr = $proc.StandardError.ReadToEnd()
$proc.WaitForExit()

$exitCode = $proc.ExitCode

# 显示 Unity 原始错误
$unityOutput = $stdout + "`n" + $stderr
if ($unityOutput -match "compilation error|error CS|Scripts have compiler errors") {
    Write-Host "--- Unity 编译输出 ---" -ForegroundColor Red
    $unityOutput -split "`n" | Where-Object { $_ -match "error|warning|compilation|Scripts" } |
        Select-Object -First 20 | ForEach-Object { Write-Host $_ -ForegroundColor $(if($_ -match "error"){'Red'}else{'Yellow'}) }
    Write-Host ""
}

Write-Host "[2/2] 解析 Editor.log..." -ForegroundColor Yellow
Write-Host ""

if (-not (Test-Path $editorLog)) {
    Write-Host "[ERROR] Editor.log 未生成: $editorLog" -ForegroundColor Red
    exit 1
}

# 读取上次编译后的新内容（对比 .prev 文件之后的行）
$allLines = Get-Content $editorLog -Encoding UTF8
$prevLines = @()
if (Test-Path "$editorLog.prev") {
    $prevLines = Get-Content "$editorLog.prev" -Encoding UTF8
}

# 找到新行（上次编译之后的内容）
$newLines = @()
if ($prevLines.Count -gt 0) {
    $lastPrevHash = ($prevLines[-1]).GetHashCode()
    $foundNew = $false
    foreach ($line in $allLines) {
        if ($foundNew -or $line.GetHashCode() -eq $lastPrevHash) {
            $foundNew = $true
            $newLines += $line
        }
    }
} else {
    $newLines = $allLines
}

# 只看最后 3000 行（最新编译）
if ($newLines.Count -gt 3000) {
    $newLines = $newLines[-3000..-1]
}

# 提取错误和警告
$errors = $newLines | Where-Object { $_ -match "error CS" }
$warnings = $newLines | Where-Object { $_ -match "warning CS" }

function Normalize-Path($line) {
    $idx = $line.IndexOf("Assets")
    if ($idx -ge 0) { return $line.Substring($idx) }
    return $line
}

if ($errors.Count -gt 0) {
    Write-Host "=== 编译错误 ($($errors.Count) 个) ===" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host (Normalize-Path $_) -ForegroundColor Red }
    Write-Host ""
}

if ($warnings.Count -gt 0) {
    Write-Host "=== 编译警告 ($($warnings.Count) 个，显示前 20) ===" -ForegroundColor Yellow
    $warnings | Select-Object -First 20 | ForEach-Object { Write-Host (Normalize-Path $_) -ForegroundColor Yellow }
    if ($warnings.Count -gt 20) {
        Write-Host "... 还有 $($warnings.Count - 20) 个警告" -ForegroundColor Yellow
    }
    Write-Host ""
}

Write-Host "=== 汇总 ===" -ForegroundColor Cyan
if ($errors.Count -eq 0 -and $exitCode -eq 0) {
    Write-Host "✓ 编译成功，无错误" -ForegroundColor Green
} else {
    Write-Host "✗ 编译失败: $($errors.Count) 个错误, $($warnings.Count) 个警告 (exit=$exitCode)" -ForegroundColor Red
}

Write-Host ""
Write-Host "日志: $editorLog" -ForegroundColor Cyan

# 清理
Remove-Item "$env:TEMP\unity_compile_$PID.log" -ErrorAction SilentlyContinue

exit $(if ($errors.Count -gt 0 -or $exitCode -ne 0) { 1 } else { 0 })