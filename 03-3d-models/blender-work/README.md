# CatLife Blender Work Index

This folder is the local Blender work area for CatLife runtime/render assets.

## Current Cat Animation Source

- `CatLife_cat_animation_coordinate_corrected.blend`
  - Current accepted cat animation source.
  - Coordinate basis: `+Z` up, cat head faces `-Y`, tail points `+Y`, `X` is left/right.
  - Contains the accepted rig objects `CL_CAT_CORRECTED_Armature` and `CL_CAT_CORRECTED_Mesh`.

## Animation Notes and QA

- `cat-animation-production-notes.md`
  - Production log for the 10-action cat animation batch.
- `qa-frames/`
  - Contact sheets and view checks used for action review.

## Exports

- `exports/cat-animation-final-20260629/`
  - Final animation export package source.
- `exports/CatLife_runtime.fbx`
  - Earlier runtime scene export retained for Unity reference.

## Archive

- `archive/20260629-animation-iterations/`
  - Superseded `.blend` and `.blend1` files from baseline, axis validation, earlier action attempts, and scene runtime/render branches.
  - These are retained only for recovery and audit. Use the current source file above for continued animation work.

## Related Deliverable

- `../../06-deliverables/cat-animation-final-package-20260629/`
  - Final handoff package copied from this work area.
