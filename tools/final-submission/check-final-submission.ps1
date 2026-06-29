param(
    [string]$ProjectRoot = (Resolve-Path "$PSScriptRoot\..\..").Path,
    [string]$OutputName = "CatLife_submission_check_20260705.md"
)

$ErrorActionPreference = "Stop"

$finalDir = Join-Path $ProjectRoot "06-deliverables\final-submission"
$llmTemplateDir = Join-Path $ProjectRoot "06-deliverables\llm-code-package-template"
$outputPath = Join-Path $finalDir $OutputName

function New-Result {
    param(
        [string]$Item,
        [string]$Expected,
        [bool]$Passed,
        [string]$Evidence,
        [string]$NextAction
    )

    [pscustomobject]@{
        Item = $Item
        Expected = $Expected
        Passed = $Passed
        Evidence = $Evidence
        NextAction = $NextAction
    }
}

function Find-FirstFile {
    param([string[]]$Patterns)

    foreach ($pattern in $Patterns) {
        $match = Get-ChildItem -LiteralPath $finalDir -File -Filter $pattern -ErrorAction SilentlyContinue |
            Sort-Object LastWriteTime -Descending |
            Select-Object -First 1
        if ($match) {
            return $match
        }
    }

    return $null
}

function Get-Sha256OrBlank {
    param($File)
    if (-not $File) {
        return ""
    }
    return (Get-FileHash -Algorithm SHA256 -LiteralPath $File.FullName).Hash
}

New-Item -ItemType Directory -Force -Path $finalDir | Out-Null

$checks = New-Object System.Collections.Generic.List[object]

$ppt = Find-FirstFile @("CatLife_作品介绍PPT*.pptx", "*.pptx")
$video = Find-FirstFile @("CatLife_作品演示视频*.mp4", "*.mp4")
$poster = Find-FirstFile @("CatLife_作品海报*.png", "*.png", "*.jpg", "*.jpeg")
$apk = Find-FirstFile @("CatLife_MVP_Android*.apk", "*.apk")
$codePackage = Find-FirstFile @("CatLife_大模型调用代码包*.zip", "*代码包*.zip")

$checks.Add((New-Result "PPT" "PPT exists and includes real product screenshots" ([bool]$ppt) ($(if($ppt){$ppt.Name}else{"missing"})) "Add CatLife_presentation_v1.pptx"))
$checks.Add((New-Result "Video" "MP4, target <=3min, hard max <=5min, shows final product/name/UI/features" ([bool]$video) ($(if($video){$video.Name}else{"missing"})) "Add CatLife_demo_video_v1.mp4"))
$checks.Add((New-Result "Poster" "Portrait 70cm x 150cm poster, jpg/jpeg/png, includes title/slogan/visual" ([bool]$poster) ($(if($poster){$poster.Name}else{"missing"})) "Add CatLife_poster_v1.png"))
$checks.Add((New-Result "APK" "Runnable Android APK, installable and launchable on device" ([bool]$apk) ($(if($apk){$apk.Name}else{"missing"})) "Add CatLife_MVP_Android_v0.1.0.apk and adb install evidence"))
$checks.Add((New-Result "Code package" "Large-model code package zip, API call marked, no secrets" ([bool]$codePackage) ($(if($codePackage){$codePackage.Name}else{"missing"})) "Package 06-deliverables/llm-code-package-template after real integration notes"))

$llmTemplateExists = Test-Path -LiteralPath $llmTemplateDir
$checks.Add((New-Result "LLM template" "Large-model code package template exists" $llmTemplateExists ($(if($llmTemplateExists){$llmTemplateDir}else{"missing"})) "Keep template or package it as final code bundle"))

$secretPatterns = @(
    "sk-[A-Za-z0-9]",
    "api[_-]key\s*[:=]",
    "API_KEY\s*[:=]",
    "secret\s*[:=]",
    "password\s*[:=]",
    "token\s*[:=]",
    "Bearer\s+[A-Za-z0-9_\-]{20,}"
)

$secretHits = New-Object System.Collections.Generic.List[string]
$scanRoots = @($finalDir, $llmTemplateDir) | Where-Object { Test-Path -LiteralPath $_ }
foreach ($root in $scanRoots) {
    Get-ChildItem -LiteralPath $root -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.Length -lt 5MB } |
        ForEach-Object {
            foreach ($pattern in $secretPatterns) {
                $hits = Select-String -LiteralPath $_.FullName -Pattern $pattern -CaseSensitive:$false -ErrorAction SilentlyContinue
                foreach ($hit in $hits) {
                    $relative = Resolve-Path -LiteralPath $hit.Path -Relative
                    $secretHits.Add("${relative}:$($hit.LineNumber):$($hit.Line.Trim())")
                }
            }
        }
}

$checks.Add((New-Result "Secret scan" "final-submission and LLM template contain no common secret patterns" ($secretHits.Count -eq 0) ("hits=" + $secretHits.Count) "Review and remove matched text"))

$deviceEvidence = Find-FirstFile @("*logcat*.txt", "*android-runtime*.txt", "*install*.txt")
$checks.Add((New-Result "Android evidence" "Install/runtime/logcat evidence exists" ([bool]$deviceEvidence) ($(if($deviceEvidence){$deviceEvidence.Name}else{"missing"})) "Save adb install and logcat evidence after device test"))

$hashRows = @()
foreach ($file in @($ppt, $video, $poster, $apk, $codePackage) | Where-Object { $_ }) {
    $hashRows += [pscustomobject]@{
        File = $file.Name
        SizeBytes = $file.Length
        SHA256 = Get-Sha256OrBlank $file
    }
}

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("# CatLife Submission Check")
$lines.Add("")
$lines.Add("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$lines.Add("Directory: " + $finalDir)
$lines.Add("")
$lines.Add("## 1. Check Results")
$lines.Add("")
$lines.Add("| Item | Expected | Status | Evidence | Next action |")
$lines.Add("|---|---|---|---|---|")
foreach ($check in $checks) {
    $status = if ($check.Passed) { "PASS" } else { "MISSING" }
    $lines.Add("| $($check.Item) | $($check.Expected) | $status | $($check.Evidence) | $($check.NextAction) |")
}

$lines.Add("")
$lines.Add("## 2. File Hashes")
$lines.Add("")
$lines.Add("| File | Size(bytes) | SHA256 |")
$lines.Add("|---|---:|---|")
if ($hashRows.Count -eq 0) {
    $lines.Add("| No final deliverable files found | 0 |  |")
} else {
    foreach ($row in $hashRows) {
        $lines.Add("| $($row.File) | $($row.SizeBytes) | $($row.SHA256) |")
    }
}

$lines.Add("")
$lines.Add("## 3. Secret Scan")
$lines.Add("")
if ($secretHits.Count -eq 0) {
    $lines.Add("No common secret patterns matched.")
} else {
    $lines.Add("Potential sensitive text found. Review manually:")
    foreach ($hit in $secretHits) {
        $lines.Add("- " + $hit)
    }
}

$lines.Add("")
$lines.Add("## 4. Conclusion")
$lines.Add("")
if (($checks | Where-Object { -not $_.Passed }).Count -eq 0) {
    $lines.Add("All automated checks passed. Manual review is still required for PPT/video/poster readability, APK device flow, and platform upload constraints.")
} else {
    $lines.Add("Missing items remain. The final submission package is not complete.")
}

Set-Content -LiteralPath $outputPath -Value $lines -Encoding UTF8

Write-Host "Wrote $outputPath"
foreach ($check in $checks) {
    $status = if ($check.Passed) { "PASS" } else { "MISSING" }
    Write-Host "$status`t$($check.Item)`t$($check.Evidence)"
}

if (($checks | Where-Object { -not $_.Passed }).Count -gt 0) {
    exit 2
}
