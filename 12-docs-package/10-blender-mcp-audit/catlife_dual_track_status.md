# CatLife Dual-Track Blender Status

Source scene:

```text
C:\Users\fujunye\Desktop\catlife_v2.blend
```

Generated branches:

```text
03-3d-models/blender-work/CatLife_master.blend
03-3d-models/blender-work/CatLife_runtime.blend
03-3d-models/blender-work/CatLife_render.blend
03-3d-models/blender-work/exports/CatLife_runtime.fbx
```

## What Was Done

- Created three physical branch files from the original scene.
- Kept `CatLife_master.blend` as the non-destructive master copy.
- Created `CatLife_runtime.blend` for Unity/Android runtime work.
- Created `CatLife_render.blend` for PPT/poster/video rendering work.
- In `CatLife_runtime.blend`:
  - Baked active Decimate modifiers into real mesh data.
  - Cleared all mesh modifiers.
  - Applied rotation and scale into mesh data while preserving scene dimensions.
  - Added deterministic runtime naming such as `CL_ENV_BLD_001`, `CL_ENV_ROAD_001`, `CL_ENV_DETAIL_001`.
  - Added `source_name` custom properties for traceability back to original imported names.
  - Added `usage = runtime` custom properties.
  - Linked runtime meshes into `EXPORT_RT`.
- In `CatLife_render.blend`:
  - Added cameras: `CAM_Front`, `CAM_Top`, `CAM_45`, `CAM_Poster_Vertical`, `CAM_Cat_Closeup`.
  - Added lights: `LGT_Sun_Key`, `LGT_Area_Fill`, `LGT_Area_SoftTop`.
  - Set `CAM_45` as active camera.
  - Set render resolution to 1920 x 1080.
- Exported Unity-facing FBX:
  - `03-3d-models/blender-work/exports/CatLife_runtime.fbx`

## Runtime Branch Stats

`CatLife_runtime.blend` after modifier bake:

| Metric | Value |
|---|---:|
| Meshes | 165 |
| Materials | 161 |
| Remaining modifiers | 0 |
| Polygons | 516,410 |
| Triangles | 875,306 |
| Vertices | 722,458 |
| Non-unit scale objects | 0 |
| Non-zero rotation objects | 0 |

Object classes:

| Class | Count |
|---|---:|
| `CL_ENV_BLD` | 22 |
| `CL_ENV_ROAD` | 66 |
| `CL_ENV_DETAIL` | 48 |
| `CL_ENV_MISC` | 19 |
| `CL_ENV_PROP` | 9 |
| `CL_ENV_GROUND` | 1 |

## Export

Runtime FBX:

```text
03-3d-models/blender-work/exports/CatLife_runtime.fbx
```

Export size:

```text
35,866,684 bytes
```

Axis settings:

```text
Forward = -Z
Up = Y
Apply Unit Scale = true
Use Mesh Modifiers = true
```

## Remaining Issues

- Runtime triangle count is still high for Android: 875k triangles.
- Materials are still fragmented: 161 materials for 165 meshes.
- Names are deterministic and traceable, but not fully semantic because original imported object names were mostly `node_0.xxx` / `Mesh_0.xxx`.
- `CatLife_runtime.fbx` needs Unity import verification before further Blender-side reduction.
- No Unity profiling has been run yet, so actual visible triangles, batches, SetPass calls, FPS, memory, and APK size are still unknown.

## Next Recommended Step

Import `CatLife_runtime.fbx` into Unity and capture:

- Imported triangle and vertex count.
- Unique material count.
- Main camera visible triangles.
- Batches and SetPass calls.
- FPS on target Android device.

After Unity measurements, reduce the top offending objects first rather than applying uniform decimation to the whole scene.
