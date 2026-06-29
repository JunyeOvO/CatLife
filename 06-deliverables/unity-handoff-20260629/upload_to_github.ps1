# =============================================
# CatLife Unity 项目上传脚本
# 运行方式：PowerShell 中执行
#   .\06-deliverables\unity-handoff-20260629\upload_to_github.ps1
# =============================================
# 前提：
#   1. 已安装 Git for Windows
#   2. 拥有 GitHub Token（有 repo 权限）
#   3. 网络能访问 github.com
# =============================================

param(
    [string]$Token = "",
    [string]$RepoUrl = "https://github.com/JunyeOvO/CatLife.git",
    [string]$Branch = "main",
    [string]$ProjectPath = "C:\Users\35068\My project (2)"
)

# ---- 配置检查 ----
if (-not $Token) {
    $Token = Read-Host "请输入 GitHub Personal Access Token（不会显示）" -AsSecureString
    $Token = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($Token))
}

# ---- 克隆仓库（浅克隆，减少下载量）----
$TempClone = "$env:TEMP\CatLife_Unity_Upload"
Write-Host "[1/6] 克隆仓库到 $TempClone ..."

if (Test-Path $TempClone) {
    Remove-Item $TempClone -Recurse -Force
}

git clone --depth 1 --filter=blob:none --sparse $RepoUrl $TempClone

if ($LASTEXITCODE -ne 0) { Write-Error "克隆失败"; exit 1 }

# ---- 稀疏克隆，只取已有内容 ----
Set-Location $TempClone
git sparse-checkout set --no-cone
git sparse-checkout add "*"

Write-Host "[2/6] 拉取仓库已有文件 ..."
git pull origin $Branch
if ($LASTEXITCODE -ne 0) { Write-Error "拉取失败"; exit 1 }

# ---- 初始化 Git LFS ----
Write-Host "[3/6] 初始化 Git LFS ..."
git lfs install 2>$null
git add .gitattributes
git lfs track "*.glb" "*.fbx" "*.png" "*.jpg" "*.wav" "*.mp3"

# ---- 复制 Unity 项目源码（排除缓存）----
Write-Host "[4/6] 复制 Unity 项目文件 ..."

# 复制必要目录
$ItemsToCopy = @(
    "Assets",
    "ProjectSettings",
    "Packages",
    "UserSettings",
    ".gitignore",
    ".gitattributes",
    "build_check.ps1"
)

foreach ($item in $ItemsToCopy) {
    $src = Join-Path $ProjectPath $item
    $dst = Join-Path $TempClone $item
    if (Test-Path $src) {
        Copy-Item -Path $src -Destination $dst -Recurse -Force
        Write-Host "  复制: $item"
    }
}

# ---- 设置远程 + Token 认证 ----
git remote set-url origin https://$Token@github.com/JunyeOvO/CatLife.git

# ---- 提交 ----
Write-Host "[5/6] 提交更改 ..."
git add -A
git status

$CommitMsg = @"
feat: 上传 CatLife Unity 完整源码

- 脚本: Cat/Core, Core/, LLM/, UI/ 全部
- 配置: ProjectSettings/, Packages/
- 美术: Art/Cat, Art/Animations, Art/Materials, Art/Models
- 字体: MSYH.TTC 系列

由 Hermes Agent 生成 · $(Get-Date -Format "yyyy-MM-dd HH:mm")
"@

git commit -m $CommitMsg
if ($LASTEXITCODE -ne 0) { Write-Host "没有新内容提交（可能文件未变）"; exit 0 }

# ---- 推送 ----
Write-Host "[6/6] 推送到 GitHub ..."
git push origin $Branch

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ 上传成功！访问 https://github.com/JunyeOvO/CatLife"
} else {
    Write-Error "推送失败，请检查 Token 权限和网络"
}

# ---- 清理 ----
# Remove-Item $TempClone -Recurse -Force
Write-Host "临时目录保留在 $TempClone（确认无误后可手动删除）"
