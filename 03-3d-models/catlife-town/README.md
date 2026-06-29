# CatLife Town Asset Index

This folder is organized as reversible local asset storage for the CatLife town scene.

## Current Working Scene

- `current/catlife_v2_view_clean_no_merge.blend`
  - Current safe Blender scene for continued town work.
  - Visual-only cleanup: no mesh merge, no topology edits, no scale application.
  - Use this first when opening the latest town scene in Blender.

## Source Assets

- `source/catlife_full_scene.glb`
  - Integrated full-scene GLB source.
- `source/individual-glb/`
  - Individual town prop/building GLB source files.
- `source/cat-models/`
  - Cat model/source references that were mixed into this folder but are not town props.

## Exports

- `exports/catlife_v2.fbx`
  - Existing FBX export kept for reference. Regenerate from the current scene before Unity handoff if the scene changes.

## Reports

- `reports/visual-artifact-fix-notes.md`
  - Notes for the black thin-line artifact investigation and current safe fix.
- `reports/qa-visual-clean/`
  - QA screenshots from the visual cleanup attempts.

## Archive

- `archive/original/catlife_v2.blend`
  - Original imported Blender scene. Keep as source recovery point.
- `archive/deprecated-merge-attempts/`
  - Deprecated mesh-merge cleanup attempt. Do not use for production because it damaged material/UV/hard-edge appearance.
- `archive/intermediate-visual-clean/`
  - Intermediate viewport-only cleanup copy retained for traceability.

## Next Pipeline Step

For the CatLife product pipeline, this scene should be treated as the Blender source branch. The next usable runtime artifact should be exported from the current working scene into a Unity-friendly runtime file, then validated in Unity/Android rather than judged from Blender render alone.
