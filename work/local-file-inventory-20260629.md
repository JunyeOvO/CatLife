# CatLife Local File Inventory - 2026-06-29

## Current Active Assets

- Cat animation source: `03-3d-models/blender-work/CatLife_cat_animation_coordinate_corrected.blend`
- Cat animation final package: `06-deliverables/cat-animation-final-package-20260629/`
- Cat town current Blender scene: `03-3d-models/catlife-town/current/catlife_v2_view_clean_no_merge.blend`
- Cat town original scene backup: `03-3d-models/catlife-town/archive/original/catlife_v2.blend`
- Original cat model package: `03-3d-models/source-cat-models/original-meshy-quadruped/`

## Reversible Cleanup Performed

- Moved superseded cat animation `.blend` and `.blend1` files into `03-3d-models/blender-work/archive/20260629-animation-iterations/`.
- Kept the active cat animation source file in `03-3d-models/blender-work/`.
- Moved the root-level Meshy cat model folder and ignored `.zip` package into `03-3d-models/source-cat-models/original-meshy-quadruped/`.
- Left tracked Unity handoff archives at the repository root: `Assets_noart.zip`, `Packages.zip`, and `ProjectSettings.zip`.

## Not Deleted

No local binary asset was deleted. Large Blender, GLB, FBX, USDZ, ZIP, and video-style artifacts remain local/ignored unless explicitly tracked already.

## Git Scope

The cleanup is intended to commit only lightweight documentation, manifests, and review images. Large binary assets remain governed by `.gitignore`.
