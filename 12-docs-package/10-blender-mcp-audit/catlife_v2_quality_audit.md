# catlife_v2.blend Quality Audit

Source file: `C:\Users\fujunye\Desktop\catlife_v2.blend`

## Verdict

This Blender file is suitable as a visual/marketing render source after camera and lighting setup, but it is not ready as a Unity/Android runtime asset. The largest blockers are excessive polygon count, import-style naming, unapplied transforms, missing camera setup, and fragmented materials.

## Scene Summary

| Metric | Value |
|---|---:|
| Objects | 167 |
| Meshes | 165 |
| Materials | 161 |
| Total polygons | 3,532,846 |
| Total vertices | 2,202,584 |
| Polygon median per mesh | 20,228 |
| Polygon average per mesh | 21,411.2 |
| Polygon max per mesh | 50,272 |
| Cameras | 0 |
| Lights | 1 |

## Issue Counts

- `auto_import_name`: 164
- `high_poly_gt_10k`: 133
- `nonzero_rotation_transform`: 72
- `non_applied_scale`: 159
- `medium_poly_gt_2k`: 27
- `very_high_poly_gt_50k`: 3
- `empty_material_slot`: 1
- `oversized_dimension_gt_20m`: 1

## Quality Gates

| Gate | Status | Reason |
|---|---|---|
| Mobile runtime readiness | FAIL | 3.53M polygons is far beyond a lightweight Android scene target. |
| Unity import readiness | FAIL | 159 meshes have unapplied scale and 72 have non-zero rotation transforms. |
| Asset maintainability | FAIL | 164 meshes use auto-import names such as `node_0.xxx` / `Mesh_0.xxx`. |
| Render readiness | PARTIAL | Geometry and materials exist, but there is no camera and only one point light. |
| Material consistency | PARTIAL | 161 materials for 165 meshes suggests fragmented generated materials. |
| Scene layout completeness | PASS WITH RISK | Scene bounds are coherent, but scale and grouping need cleanup. |

## Top Problem Objects

| Object | Severity | Polygons | Dimensions | Issues |
|---|---:|---:|---|---|
| `node_0.011` | 3 | 50272 | [2.386, 0.762, 0.804] | auto_import_name, very_high_poly_gt_50k, non_applied_scale |
| `node_0.019` | 3 | 50012 | [1.683, 0.769, 1.918] | auto_import_name, very_high_poly_gt_50k, non_applied_scale |
| `Mesh_0.010` | 3 | 50004 | [4.816, 0.236, 4.858] | auto_import_name, very_high_poly_gt_50k, non_applied_scale |
| `Mesh_0` | 2 | 50000 | [2.27, 1.173, 0.318] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.001` | 2 | 50000 | [5.103, 4.449, 4.933] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `Mesh_0.002` | 2 | 50000 | [1.111, 0.399, 0.534] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform |
| `Mesh_0.003` | 2 | 50000 | [2.5, 2.197, 1.879] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.009` | 2 | 50000 | [1.508, 0.882, 0.739] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.011` | 2 | 50000 | [1.509, 0.883, 0.74] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.005` | 2 | 50000 | [3.183, 2.02, 2.643] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.008` | 2 | 50000 | [6.179, 6.172, 5.302] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.009` | 2 | 50000 | [12.982, 6.494, 3.173] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.017` | 2 | 50000 | [7.508, 6.007, 5.095] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.015` | 2 | 49996 | [1.986, 0.557, 1.756] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.037` | 2 | 49980 | [11.959, 0.146, 11.969] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.047` | 2 | 49980 | [4.786, 0.146, 4.787] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.006` | 2 | 49966 | [2.239, 0.805, 0.725] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.007` | 2 | 49872 | [7.518, 5.286, 4.799] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.167` | 2 | 49868 | [1.013, 0.612, 1.04] | auto_import_name, high_poly_gt_10k |
| `node_0.168` | 2 | 49868 | [1.013, 0.612, 1.04] | auto_import_name, high_poly_gt_10k |
| `node_0.169` | 2 | 49868 | [1.022, 0.613, 1.041] | auto_import_name, high_poly_gt_10k |
| `node_0.170` | 2 | 49868 | [1.013, 0.612, 1.04] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform |
| `node_0.018` | 2 | 49736 | [3.79, 8.215, 3.84] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.004` | 2 | 40164 | [1.705, 0.739, 0.965] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.041` | 2 | 36401 | [1.905, 1.5, 2.336] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |

## Required Fix Mapping

| Problem | Target Fix | Priority |
|---|---|---|
| Meshes over 10k polygons | Decimate, retopologize, or replace with low-poly source assets. Target: props <500 tris, key buildings <3k-8k tris, whole mobile scene preferably <150k-300k tris. | P0 |
| Auto-generated names | Rename by role: `Ground_Island`, `Road_Ring_01`, `Building_TomatoClock`, `Tree_01`, etc. | P0 |
| Unapplied scale/rotation | Apply transforms before export: scale = 1, rotation = 0 where possible. | P0 |
| No camera | Add render cameras for front, top, close-up, vertical poster. | P1 |
| One point light only | Add controlled sun/area/environment lighting for render, and keep Unity runtime lighting simple. | P1 |
| Too many materials | Merge repeated color-equivalent materials into a low-poly palette. | P1 |
| Empty material slot on `??` | Remove or assign the empty material slot. | P1 |
| Oversized base object `??` | Verify world scale and split base into Unity-friendly chunks if needed. | P2 |

## Per-Model Audit

A CSV version is available at `tools/blender-mcp/catlife_v2_model_audit.csv`.

| Object | Collection | Severity | Polygons | Vertices | Dimensions | Issues |
|---|---|---:|---:|---:|---|---|
| `Mesh_0` | Collection 5 | 2 | 50000 | 30538 | [2.27, 1.173, 0.318] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.001` | Collection 5 | 2 | 50000 | 29504 | [5.103, 4.449, 4.933] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `Mesh_0.002` | Collection 5 | 2 | 50000 | 29677 | [1.111, 0.399, 0.534] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform |
| `Mesh_0.003` | Collection 5 | 2 | 50000 | 33293 | [2.5, 2.197, 1.879] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.004` | Collection 5 | 2 | 40164 | 28431 | [1.705, 0.739, 0.965] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.005` | Collection 5 | 2 | 33134 | 20792 | [2.397, 0.67, 4.24] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `Mesh_0.006` | Collection 5 | 2 | 49966 | 33634 | [2.239, 0.805, 0.725] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.007` | Collection 5 | 2 | 13511 | 8018 | [1.389, 1.296, 1.367] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `Mesh_0.008` | Collection 5 | 1 | 2962 | 2029 | [2.479, 0.854, 0.557] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.009` | 04_Stone_Ring_Road | 2 | 50000 | 24998 | [1.508, 0.882, 0.739] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.010` | Collection | 3 | 50004 | 36643 | [4.816, 0.236, 4.858] | auto_import_name, very_high_poly_gt_50k, non_applied_scale |
| `Mesh_0.011` | 04_Stone_Ring_Road | 2 | 50000 | 24998 | [1.509, 0.883, 0.74] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.014` | 04_Stone_Ring_Road | 1 | 2962 | 2029 | [2.48, 0.855, 0.543] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.015` | 04_Stone_Ring_Road | 1 | 2953 | 1999 | [1.554, 0.854, 0.302] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.016` | 04_Stone_Ring_Road | 1 | 2953 | 1999 | [1.554, 0.854, 0.302] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `Mesh_0.017` | 04_Stone_Ring_Road | 1 | 2953 | 1999 | [1.554, 0.854, 0.302] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `node_0.001` | Collection 5 | 2 | 36372 | 25776 | [1.837, 1.36, 0.899] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.002` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.005` | Collection 5 | 2 | 50000 | 30056 | [3.183, 2.02, 2.643] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.006` | Collection 5 | 2 | 19553 | 11595 | [3.141, 2.07, 0.927] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.007` | Collection 5 | 2 | 49872 | 37943 | [7.518, 5.286, 4.799] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.008` | Collection 5 | 2 | 50000 | 38501 | [6.179, 6.172, 5.302] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.009` | Collection 5 | 2 | 50000 | 35108 | [12.982, 6.494, 3.173] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.011` | Collection 5 | 3 | 50272 | 31067 | [2.386, 0.762, 0.804] | auto_import_name, very_high_poly_gt_50k, non_applied_scale |
| `node_0.012` | Collection 5 | 2 | 21962 | 12525 | [3.041, 2.261, 0.966] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.014` | Scene Collection | 2 | 20228 | 13563 | [1.194, 1.72, 0.915] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.015` | Collection 5 | 2 | 49996 | 35562 | [1.986, 0.557, 1.756] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.016` | Collection 5 | 2 | 20228 | 13563 | [0.955, 1.373, 0.732] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.017` | Collection 5 | 2 | 50000 | 32996 | [7.508, 6.007, 5.095] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.018` | Collection 5 | 2 | 49736 | 39968 | [3.79, 8.215, 3.84] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.019` | Collection 5 | 3 | 50012 | 29513 | [1.683, 0.769, 1.918] | auto_import_name, very_high_poly_gt_50k, non_applied_scale |
| `node_0.020` | Scene Collection | 2 | 20943 | 12191 | [1.996, 0.937, 0.307] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.021` | Collection 5 | 2 | 15301 | 10413 | [1.079, 0.902, 1.067] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.022` | Collection 5 | 1 | 9023 | 6177 | [0.52, 2.223, 0.528] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.023` | 04_Stone_Ring_Road | 2 | 20228 | 13563 | [1.193, 1.717, 0.915] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.024` | Scene Collection | 2 | 15301 | 10413 | [1.077, 0.896, 1.064] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.026` | 04_Stone_Ring_Road | 2 | 15301 | 10413 | [1.079, 0.903, 1.065] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.027` | 04_Stone_Ring_Road | 2 | 15301 | 10413 | [1.077, 0.896, 1.064] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.028` | Scene Collection | 2 | 20943 | 12191 | [1.996, 0.937, 0.307] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.029` | Collection 5 | 2 | 11408 | 6806 | [0.713, 1.377, 0.696] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.030` | 04_Stone_Ring_Road | 2 | 15301 | 10413 | [0.897, 0.747, 0.886] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.031` | 04_Stone_Ring_Road | 1 | 2416 | 5113 | [3.722, 2.28, 1.221] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `node_0.032` | Collection 5 | 1 | 8798 | 4995 | [1.457, 2.125, 1.435] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.033` | Collection 5 | 2 | 15599 | 9593 | [3.33, 3.118, 1.335] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.034` | 04_Stone_Ring_Road | 1 | 543 | 1049 | [1.094, 1.673, 1.082] | auto_import_name, non_applied_scale |
| `node_0.035` | 04_Stone_Ring_Road | 2 | 15599 | 9593 | [2.498, 2.304, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.036` | 04_Stone_Ring_Road | 2 | 19553 | 11595 | [5.236, 3.451, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.037` | Scene Collection | 2 | 49980 | 33058 | [11.959, 0.146, 11.969] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.038` | Collection 5 | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.041` | 04_Stone_Ring_Road | 2 | 36401 | 25826 | [1.905, 1.5, 2.336] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.046` | Collection 5 | 2 | 16467 | 9983 | [0.863, 1.854, 0.889] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.047` | 04_Stone_Ring_Road | 2 | 49980 | 33058 | [4.786, 0.146, 4.787] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.053` | 04_Stone_Ring_Road | 2 | 15301 | 10413 | [0.718, 0.597, 0.709] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.054` | 04_Stone_Ring_Road | 2 | 21962 | 12525 | [2.536, 1.885, 0.806] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.055` | 04_Stone_Ring_Road | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.056` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.057` | 04_Stone_Ring_Road | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.058` | 04_Stone_Ring_Road | 2 | 19528 | 11557 | [5.236, 2.905, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.059` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.060` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.061` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.062` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.063` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.064` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.066` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.067` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.068` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.069` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.070` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.071` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.072` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.073` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.074` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.075` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.076` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.077` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.078` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.079` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.080` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.081` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.082` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.083` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.084` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.085` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.086` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.087` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.088` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.089` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.090` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.091` | Scene Collection | 2 | 20943 | 12191 | [1.996, 0.937, 0.306] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.092` | Scene Collection | 2 | 20943 | 12191 | [1.996, 0.937, 0.307] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.094` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.095` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.096` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.097` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.098` | Scene Collection | 2 | 20943 | 12191 | [1.994, 0.941, 0.312] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.099` | Scene Collection | 2 | 20943 | 12191 | [1.996, 0.937, 0.307] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.102` | Scene Collection | 2 | 15301 | 10413 | [1.077, 0.896, 1.064] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.103` | Scene Collection | 2 | 15301 | 10413 | [1.077, 0.896, 1.064] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.104` | Scene Collection | 2 | 20228 | 13563 | [1.194, 1.72, 0.915] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.105` | Scene Collection | 2 | 15599 | 9593 | [2.498, 2.069, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.106` | Scene Collection | 1 | 2416 | 5113 | [3.724, 2.278, 1.221] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `node_0.107` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.108` | Scene Collection | 2 | 19553 | 11595 | [5.236, 3.451, 1.545] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.109` | Scene Collection | 2 | 15599 | 9593 | [3.33, 2.759, 1.335] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.110` | Scene Collection | 1 | 8798 | 4995 | [1.457, 2.125, 1.435] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.111` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.112` | Scene Collection | 2 | 15599 | 9593 | [3.33, 2.759, 1.335] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.113` | Scene Collection | 2 | 15599 | 9593 | [2.498, 2.069, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.114` | Scene Collection | 2 | 21962 | 12525 | [3.042, 2.262, 0.966] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.115` | Scene Collection | 2 | 19553 | 11595 | [5.237, 3.81, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.116` | Scene Collection | 1 | 2416 | 5113 | [3.724, 2.161, 1.221] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `node_0.118` | Scene Collection | 2 | 15599 | 9593 | [2.498, 1.573, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.119` | Scene Collection | 2 | 11408 | 6806 | [0.71, 1.374, 0.695] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.120` | Scene Collection | 2 | 19553 | 11595 | [5.237, 3.644, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.121` | Scene Collection | 1 | 8798 | 4995 | [1.457, 2.125, 1.436] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.122` | Scene Collection | 2 | 21962 | 12525 | [2.536, 1.886, 0.806] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.123` | Scene Collection | 2 | 15599 | 9593 | [3.33, 2.759, 1.329] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.124` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.125` | Scene Collection | 2 | 15301 | 10413 | [1.077, 0.901, 1.064] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.127` | Scene Collection | 1 | 9023 | 6177 | [0.52, 2.223, 0.528] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.128` | Scene Collection | 2 | 15599 | 9593 | [2.496, 2.069, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.129` | Scene Collection | 2 | 11408 | 6806 | [0.71, 1.576, 0.695] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.130` | Scene Collection | 2 | 19553 | 11595 | [5.236, 3.119, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.131` | Scene Collection | 2 | 15599 | 9593 | [2.498, 2.573, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.132` | Scene Collection | 2 | 11408 | 6806 | [0.71, 1.017, 0.694] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.133` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.134` | Scene Collection | 1 | 2416 | 5113 | [3.722, 2.011, 1.221] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `node_0.135` | Scene Collection | 2 | 19553 | 11595 | [6.283, 4.141, 1.853] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.136` | Scene Collection | 2 | 19553 | 11595 | [5.236, 3.644, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.137` | Scene Collection | 1 | 8798 | 4995 | [1.457, 2.125, 1.436] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.138` | Scene Collection | 2 | 15599 | 9593 | [2.498, 2.069, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.139` | Scene Collection | 2 | 15599 | 9593 | [3.33, 2.759, 1.335] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.140` | Scene Collection | 2 | 15301 | 10413 | [1.079, 0.902, 1.065] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.141` | Scene Collection | 2 | 19528 | 11557 | [5.236, 2.905, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.142` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.143` | Scene Collection | 2 | 21962 | 12525 | [2.535, 1.885, 0.805] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.144` | Scene Collection | 2 | 21962 | 12525 | [3.043, 2.262, 0.967] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.145` | Scene Collection | 2 | 19553 | 11595 | [5.236, 3.81, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.146` | Scene Collection | 2 | 15599 | 9593 | [2.496, 2.069, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.147` | Scene Collection | 2 | 19528 | 11557 | [5.237, 2.905, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.148` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.149` | Scene Collection | 2 | 21962 | 12525 | [2.536, 2.587, 0.806] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.150` | Scene Collection | 2 | 19553 | 11595 | [5.236, 3.451, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.151` | Scene Collection | 2 | 15599 | 9593 | [2.498, 2.304, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.152` | Scene Collection | 2 | 15599 | 9593 | [3.33, 3.118, 1.335] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.153` | Scene Collection | 1 | 8798 | 4995 | [1.457, 2.125, 1.436] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.154` | Scene Collection | 1 | 2416 | 5113 | [3.722, 2.28, 1.221] | auto_import_name, medium_poly_gt_2k, nonzero_rotation_transform, non_applied_scale |
| `node_0.155` | Scene Collection | 2 | 21962 | 12525 | [3.043, 1.795, 0.967] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.156` | Scene Collection | 2 | 15599 | 9593 | [3.33, 3.118, 1.335] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.157` | Scene Collection | 2 | 15599 | 9593 | [2.496, 2.303, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.158` | Scene Collection | 2 | 19553 | 11595 | [5.236, 2.65, 1.545] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.159` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.160` | Scene Collection | 2 | 19553 | 11595 | [5.236, 4.01, 1.544] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.161` | Scene Collection | 2 | 21962 | 12525 | [3.042, 2.262, 0.966] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform, non_applied_scale |
| `node_0.162` | Scene Collection | 2 | 21962 | 12525 | [2.536, 1.885, 0.806] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.163` | Scene Collection | 1 | 8798 | 4995 | [0.729, 1.062, 0.718] | auto_import_name, medium_poly_gt_2k, non_applied_scale |
| `node_0.164` | Scene Collection | 2 | 19528 | 11557 | [5.236, 2.905, 1.545] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.165` | Scene Collection | 2 | 15599 | 9593 | [3.33, 2.759, 1.335] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.166` | Scene Collection | 2 | 15599 | 9593 | [2.498, 2.069, 1.001] | auto_import_name, high_poly_gt_10k, non_applied_scale |
| `node_0.167` | Scene Collection | 2 | 49868 | 31104 | [1.013, 0.612, 1.04] | auto_import_name, high_poly_gt_10k |
| `node_0.168` | Scene Collection | 2 | 49868 | 31104 | [1.013, 0.612, 1.04] | auto_import_name, high_poly_gt_10k |
| `node_0.169` | Scene Collection | 2 | 49868 | 31104 | [1.022, 0.613, 1.041] | auto_import_name, high_poly_gt_10k |
| `node_0.170` | Scene Collection | 2 | 49868 | 31104 | [1.013, 0.612, 1.04] | auto_import_name, high_poly_gt_10k, nonzero_rotation_transform |
| `柱体` | Collection 5 | 2 | 290 | 384 | [40.936, 33.527, 2.206] | empty_material_slot, oversized_dimension_gt_20m |
