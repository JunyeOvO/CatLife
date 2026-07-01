# CatLife Unity Art Import Manifest

Date: 2026-07-02

## Imported local runtime assets

| Asset | Unity path | Source path | Size | SHA256 prefix | Git policy |
|---|---|---|---:|---|---|
| Town latest no-skybox model | `Assets/Art/Town/Source/catlife_v2_island_grass_style_no_skybox_20260630.fbx` | `03-3d-models/catlife-town/current/catlife_v2_island_grass_style_no_skybox_20260630.fbx` | 42,809,740 bytes | `78494D2385C77EE9` | Local binary, ignored |
| Cat rig/model source | `Assets/Art/Cat/Animations/CatLife_cat_10_actions_final_state.fbx` | `06-deliverables/cat-animation-final-package-20260629/CatLife_cat_10_actions_final_state.fbx` | 20,387,692 bytes | `9A97A3589EFD9FD1` | Local binary, ignored |
| Cat action manifest | `Assets/Art/Cat/Animations/cat_actions_manifest.json` | `06-deliverables/cat-animation-final-package-20260629/cat_actions_manifest.json` | 2,862 bytes | `27412D41A5FD2CDE` | Tracked |

## Extracted Unity animation clips

The source FBX contains extra draft takes and a zero-frame `CL_CAT_SRC_BasePose` take. For Unity runtime use, the 10 final manifest actions were extracted into independent `.anim` assets under:

`Assets/Art/Cat/Animations/Clips/`

Extracted clips:

- `CL_CAT_IdleBreath_v06_headsync_loop_108f`
- `CL_CAT_AlertLook_v01_loop_120f`
- `CL_CAT_PawWave_v01_loop_96f`
- `CL_CAT_TailWagHappy_v01_loop_96f`
- `CL_CAT_CuriousSniff_v02_loop_112f`
- `CL_CAT_HeadTiltListen_v01_loop_96f`
- `CL_CAT_LookBack_v02_loop_112f`
- `CL_CAT_StretchYawn_v03_slow_loop_264f`
- `CL_CAT_EarTwitchAlert_v02_loop_120f`
- `CL_CAT_HeadShakeNo_v01_loop_108f`

Unity verification showed all 10 extracted clips have `loopTime=true`.

## Town source note

The current authoritative town source is:

`03-3d-models/catlife-town/current/catlife_v2_island_grass_style_no_skybox_20260630.fbx`

Unity verification loaded this FBX as `catlife_v2_island_grass_style_no_skybox_20260630` with 173 transforms and 167 renderers. The older `Assets/Art/Town/Source/catlife_v2.fbx` seed copy was removed from the Unity workspace to avoid ambiguous scene assembly.
