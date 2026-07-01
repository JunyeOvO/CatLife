# CatLife Project File Map

Last updated: 2026-07-01

This file records the canonical location for each project file category after local cleanup.

## Root

| Path | Purpose |
|---|---|
| `README.md` | Repository entry and current project status |
| `AGENTS.md` | Project-local Codex/agent workflow rules |
| `.gitignore`, `.gitattributes` | Git behavior and large-file policy |

Root should not contain planning documents, member role documents, Unity handoff archives, or generated asset packages.

## Canonical Folders

| Category | Canonical location |
|---|---|
| Official competition documents | `01-competition-docs/` |
| Asset specs and previews | `02-asset-specs/` |
| Blender and 3D source assets | `03-3d-models/` |
| Reference images | `04-reference-images/` |
| Textures | `05-textures/` |
| Renders and visual outputs | `05-renders/` |
| Competition deliverables and handoff packages | `06-deliverables/` |
| Technical specs and JSON schemas | `07-tech-specs/` |
| Handoff docs, planning, and role plans | `08-handoff-docs/` |
| Defense and script material | `09-defense/` |
| Art guide and Blender landing plan | `10-art-guide/` |
| Legacy PDF handoff package | `11-handoff-package/` |
| Consolidated docs package copy | `12-docs-package/` |
| Local workflow notes | `work/` |
| Local tools | `tools/` |

## Handoff and Planning

| File group | Canonical location |
|---|---|
| Official application-track semifinal meeting PDF | `01-competition-docs/应用赛道复赛交流会-0528.pdf` |
| Official innovation sharing PDF | `01-competition-docs/创新创意经验分享.pdf` |
| Competition development rules and quality gates PDF | `01-competition-docs/02-开发规约与质量门禁.pdf` |
| Competition review checklist PDF | `01-competition-docs/03-评审对照检查表.pdf` |
| Competition official information integration PDF | `01-competition-docs/04-官方信息完全整合.pdf` |
| Current sprint plan | `08-handoff-docs/planning/CatLife_复赛冲刺计划.md` |
| Development guide | `08-handoff-docs/planning/CatLife_DEV-GUIDE_复赛开发规约.md` |
| Review checklist | `08-handoff-docs/planning/CatLife_复赛评审对照检查表.md` |
| Competition meeting integration report | `08-handoff-docs/planning/复赛交流会_信息整合报告.md` |
| Member role plans | `08-handoff-docs/role-plans/` |
| Current project content and development thinking overview | `08-handoff-docs/planning/CatLife_当前项目内容与开发思路总览_20260701.md` |
| Unity handoff document and scripts | `06-deliverables/unity-handoff-20260629/` |
| Collaborator Unity reference export | `06-deliverables/unity-collaborator-reference-20260629/` |
| CatLife UI assembly kit with previews and Unity scripts | `06-deliverables/catlife-ui-assembly-kit-20260629/` |
| GitHub collaborator upload location and summary | `08-handoff-docs/planning/CatLife_GitHub协作者上传内容定位与摘要_20260629.md` |
| Unity import validation checklist | `06-deliverables/unity-handoff-20260629/UNITY_IMPORT_VALIDATION.md` |
| Pre-APK visual demo preview package | `06-deliverables/demo-preview-20260629/` |
| Non-Unity preparation master plan | `07-tech-specs/CatLife_MVP_非Unity准备工作总方案.md` |
| Mobile 3D performance budget | `07-tech-specs/CatLife_移动端3D性能预算与优化方案.md` |
| Town scene Unity landing plan | `07-tech-specs/CatLife_猫咪小镇场景Unity落地方案.md` |
| APK and demo video execution plan | `08-handoff-docs/planning/CatLife_MVP_从当前状态到APK与演示视频执行计划.md` |
| Clean rebuild from latest local scene plan | `08-handoff-docs/planning/CatLife_从最新本地场景从零重建App流程.md` |
| Human-team complete development workflow research | `08-handoff-docs/planning/CatLife_人类团队完整开发流程调研_20260701.md` |
| Official competition calibration record | `08-handoff-docs/planning/CatLife_复赛官方材料校准记录_20260629.md` |
| Android build and device QA plan | `07-tech-specs/CatLife_Android打包与真机QA方案.md` |
| LLM code package and privacy fallback plan | `07-tech-specs/CatLife_大模型代码包与隐私降级方案.md` |
| Complete CatLife UI specification | `07-tech-specs/CatLife_UI完整设计与交互规格_20260629.md` |
| Use case library, metrics, and flow table | `07-tech-specs/CatLife_用例库指标流程表_20260629.md` |
| Unity implementation concepts and simple scripts | `07-tech-specs/CatLife_Unity实现技术与脚本概念_20260629.md` |
| User scenarios and use case coverage validation | `08-handoff-docs/planning/CatLife_用户场景流程与用例覆盖验证_20260629.md` |
| UI competition assembly guide with preview assets | `08-handoff-docs/planning/CatLife_UI顶格比赛提交装配说明_20260629.md` |
| Homepage target breakdown and fixed-height camera controls | `08-handoff-docs/planning/CatLife_主页目标稿拆解与摄像机控制_20260629.md` |
| Cat chat bubble UI module implementation | `08-handoff-docs/planning/CatLife_猫咪聊天气泡模块实现说明_20260630.md` |
| Demo video script and shot list | `08-handoff-docs/planning/CatLife_演示视频脚本与镜头表.md` |
| Final submission checklist | `08-handoff-docs/planning/CatLife_最终提交包检查表.md` |
| 10-page PPT refinement script | `08-handoff-docs/planning/CatLife_作品介绍PPT_10页精修脚本.md` |
| Poster copy and layout plan | `08-handoff-docs/planning/CatLife_海报文案与版式方案.md` |
| User validation interview and survey template | `08-handoff-docs/planning/CatLife_用户验证访谈与问卷模板.md` |

## Active 3D Assets

| Asset | Canonical location |
|---|---|
| Current cat animation source blend | `03-3d-models/blender-work/CatLife_cat_animation_coordinate_corrected.blend` |
| Cat animation final handoff package | `06-deliverables/cat-animation-final-package-20260629/` |
| Cat town current Blender scene | `03-3d-models/catlife-town/current/catlife_v2_view_clean_no_merge.blend` |
| Original Meshy cat source package | `03-3d-models/source-cat-models/original-meshy-quadruped/` |

## Unity Handoff Package

| File | Location |
|---|---|
| `Assets_noart.zip` | `06-deliverables/unity-handoff-20260629/Assets_noart.zip` |
| `Packages.zip` | `06-deliverables/unity-handoff-20260629/Packages.zip` |
| `ProjectSettings.zip` | `06-deliverables/unity-handoff-20260629/ProjectSettings.zip` |
| `build_check.ps1` | `06-deliverables/unity-handoff-20260629/build_check.ps1` |
| `upload_to_github.ps1` | `06-deliverables/unity-handoff-20260629/upload_to_github.ps1` |
| `交接文档_陈泓森_Unity.md` | `06-deliverables/unity-handoff-20260629/交接文档_陈泓森_Unity.md` |
| GitHub upload summary | `06-deliverables/unity-handoff-20260629/GITHUB_UPLOAD_SUMMARY.md` |
| Collaborator reference file summary | `06-deliverables/unity-collaborator-reference-20260629/FILE_SUMMARY.md` |
| `mvp-unity-assets/` | `06-deliverables/unity-handoff-20260629/mvp-unity-assets/` |
| Android batch build entrypoint | `06-deliverables/unity-handoff-20260629/mvp-unity-assets/Assets/Editor/CatLifeAndroidBuild.cs` |
| Final submission folder | `06-deliverables/final-submission/` |
| Large-model code package template | `06-deliverables/llm-code-package-template/` |
| Final submission checker | `tools/final-submission/check-final-submission.ps1` |
| LLM code package packer | `tools/final-submission/package-llm-code.ps1` |
| Final submission evidence initializer | `tools/final-submission/init-final-evidence.ps1` |
| Current gap audit | `08-handoff-docs/planning/CatLife_当前缺口审计_20260629.md` |
| Android device test record template | `08-handoff-docs/planning/CatLife_Android真机测试记录模板.md` |
| Recording and editing checklist | `08-handoff-docs/planning/CatLife_录屏剪辑执行清单.md` |
| Final release evidence runbook | `08-handoff-docs/planning/CatLife_最终发布证据包与提交运行手册.md` |

## Git Policy

Large binary working files such as `.blend`, `.fbx`, `.glb`, `.usdz`, videos, and archives stay local or are tracked only when already intentionally committed as small handoff artifacts. New large source assets should be kept under their canonical folders and left ignored unless an explicit release/storage decision is made.
