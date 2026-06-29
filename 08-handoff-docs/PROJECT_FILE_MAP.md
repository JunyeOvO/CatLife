# CatLife Project File Map

Last updated: 2026-06-29

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
| Current sprint plan | `08-handoff-docs/planning/CatLife_复赛冲刺计划.md` |
| Development guide | `08-handoff-docs/planning/CatLife_DEV-GUIDE_复赛开发规约.md` |
| Review checklist | `08-handoff-docs/planning/CatLife_复赛评审对照检查表.md` |
| Competition meeting integration report | `08-handoff-docs/planning/复赛交流会_信息整合报告.md` |
| Member role plans | `08-handoff-docs/role-plans/` |
| Unity handoff document and scripts | `06-deliverables/unity-handoff-20260629/` |

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

## Git Policy

Large binary working files such as `.blend`, `.fbx`, `.glb`, `.usdz`, videos, and archives stay local or are tracked only when already intentionally committed as small handoff artifacts. New large source assets should be kept under their canonical folders and left ignored unless an explicit release/storage decision is made.
