# CatLife Unity Collaborator Reference Export 2026-06-29

Purpose: archive the Unity-side work found in `work/unity-android-build-batch-20260629/` as a reference package only.

This package is not the new development baseline. The next full implementation should start from the local latest CatLife town scene/model assets, then rebuild the Unity app cleanly. Use this package to inspect prior scripts, scene structure, build settings, UI assets, and failure modes.

## Source

| Field | Value |
|---|---|
| Source workspace | `work/unity-android-build-batch-20260629/` |
| Export destination | `06-deliverables/unity-collaborator-reference-20260629/` |
| Export date | 2026-06-29 |
| Export mode | Reference-only source snapshot |
| Manifest | `manifest.csv` |
| Human-readable summary | `FILE_SUMMARY.md` |

## Included

| Area | Path | Notes |
|---|---|---|
| Runtime scripts | `unity-reference/Assets/脚本/` | State machine, focus flow, cat controller, UI, LLM mock/client wrappers |
| Editor scripts | `unity-reference/Assets/Editor/` | Scene setup helpers, Android build entrypoint, animation extraction/config tools |
| Scenes | `unity-reference/Assets/Scenes/` | `startscene`, `mainscene`, `FocusScene`, `SampleScene`, lighting side files |
| Scene build guide | `unity-reference/Assets/场景搭建指南/` | Collaborator-authored guide material if present |
| UI image assets | `unity-reference/Assets/Images/` | UI placeholders and 2D image assets |
| Screenshots | `unity-reference/Assets/Screenshots/` | Unity-side visual evidence from the prior validation workspace |
| URP/settings assets | `unity-reference/Assets/Settings/` | Render pipeline/profile assets used in that workspace |
| Packages | `unity-reference/Packages/` | `manifest.json` and `packages-lock.json` |
| Project settings | `unity-reference/ProjectSettings/` | Unity project settings snapshot |
| MCP log excerpts | `unity-reference/MCP-log-excerpts/` | Last 80 lines of MCP logs, for provenance/debug only |

## Excluded

| Excluded | Reason |
|---|---|
| `Library/`, `Temp/`, `Logs/`, `UserSettings/` | Unity-generated machine-local cache/state |
| `Builds/` | Generated output, not a source reference |
| `.sln`, `.csproj`, `.vsconfig` | IDE-generated files |
| Full `Assets/UnityMCP/` package state | Tooling/runtime noise; only compact log excerpts are kept |
| Large model/source binaries from `Assets/Art/Cat/Animations/` | The next baseline should use the latest local Blender/export assets, not this reference snapshot |

## Reference Value

Useful parts to inspect:

- `Assets/脚本/Core/StateMachine.cs`: the prior four-state flow.
- `Assets/脚本/Cat/CatController.cs`: mapping CatLife states to Animator states.
- `Assets/脚本/LLM/`: mock/client/analyzer structure for model integration.
- `Assets/Editor/CatLifeAndroidBuild.cs`: prior batch Android build entrypoint.
- `Assets/Editor/MainSceneSetup.cs` and `FocusSceneSetup.cs`: prior automated scene setup ideas.
- `ProjectSettings/EditorBuildSettings.asset`: prior scene list and order.
- `Packages/manifest.json`: prior package dependency footprint.

## Do Not Reuse Blindly

- Do not copy this package wholesale into the new project.
- Do not treat these scenes as authoritative final scenes.
- Do not inherit scene object names or serialized references without checking them against the new local town model.
- Do not assume this package proves Android runtime success.
- Do not use the prior package list as-is unless the dependency is required by the new app.

## Recommended Reuse Order

1. Read this README and `manifest.csv`.
2. Read `FILE_SUMMARY.md` to locate the useful scripts, scenes, settings, and risk areas.
3. Inspect runtime scripts for intent and API boundaries.
4. Recreate a clean Unity project from the latest local scene/model assets.
5. Reimplement only the needed contracts: state machine, cat animation binding, focus UI flow, LLM adapter.
6. Use the prior Editor scripts as reference for automation patterns, not as final scripts.
7. Build and verify with the evidence workflow in `08-handoff-docs/planning/CatLife_最终发布证据包与提交运行手册.md`.

## Current Conclusion

The collaborator Unity work is now preserved inside the project for reference. Future development should start from:

- `03-3d-models/catlife-town/current/catlife_v2_view_clean_no_merge.blend`
- `03-3d-models/blender-work/CatLife_cat_animation_coordinate_corrected.blend`
- `06-deliverables/cat-animation-final-package-20260629/`

and should not depend on `work/unity-android-build-batch-20260629/`.
