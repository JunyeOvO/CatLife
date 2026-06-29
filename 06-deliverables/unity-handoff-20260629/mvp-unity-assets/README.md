# CatLife MVP Unity Assets

Date: 2026-06-29

This directory contains the text-based Unity assets promoted from the local MCP validation project.

## Copy Target

Copy the `Assets/` directory in this folder into the Unity project root, preserving paths and `.meta` files.

## External Large Asset Required

The FBX itself is intentionally not stored here because `*.fbx` is treated as a large local artifact.

Before opening `mainscene`, copy:

```text
06-deliverables/cat-animation-final-package-20260629/CatLife_cat_10_actions_final_state.fbx
```

to:

```text
Assets/Art/Cat/Animations/CatLife_cat_10_actions_final_state.fbx
```

Keep the paired `.fbx.meta` from this directory:

```text
Assets/Art/Cat/Animations/CatLife_cat_10_actions_final_state.fbx.meta
```

That `.meta` preserves the Unity GUID used by `mainscene.unity` and `CatLife_10Actions_Validation.controller`, and it records the loop settings validated through MCP.

## Included Assets

- `Assets/Scenes/mainscene.unity`
- `Assets/Art/Cat/Animations/CatLife_10Actions_Validation.controller`
- `Assets/Art/Cat/Animations/CatLife_cat_10_actions_final_state.fbx.meta`
- `Assets/Art/Cat/Animations/CatLife_OrangeCat_MVP.mat`
- `Assets/脚本/Cat/CatController.cs`
- `Assets/脚本/Cat/CatAnimationMvpDemo.cs`
- `Assets/脚本/MainSceneManager.cs`

## Validated Runtime Behavior

In the validation project:

- `mainscene` contains `CatModel_AnimatedMVP`.
- The original static `CatModel` is preserved as disabled `CatModel_LegacyStatic`.
- `CatLifeRuntime` drives `Normal -> Transition -> Focus -> Reward`.
- `CatController` cross-fades to real Animator states.
- `MainSceneController` no longer calls `Animator.Play` with clip names.
- Play Mode reached `Reward` with Unity Console reporting 0 runtime errors.
- Game View visual evidence is stored at:

```text
06-deliverables/unity-handoff-20260629/qa-screenshots/catlife-mvp-main-gameview.png
```
