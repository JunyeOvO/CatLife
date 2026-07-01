# CatLife Town GLB Export - 2026-07-02

## Source

- Blender file: `03-3d-models/catlife-town/current/catlife_v2_island_grass_style_no_skybox_20260630.blend`
- Exported file: `03-3d-models/catlife-town/current/catlife_v2_island_grass_style_no_skybox_20260702.glb`
- Export route: Blender MCP on `127.0.0.1:9876`

## Export Settings

- Format: binary glTF 2.0 (`.glb`)
- Visible objects only: yes
- Apply transforms: yes
- Y-up export: yes
- Cameras/lights: excluded
- Materials: flat-color Principled BSDF rebuilt from Blender `diffuse_color`
- Textures/images: intentionally excluded for reliable Unity flat-color import

## Verification

- GLB size: `67,319,640` bytes
- SHA256 prefix: `a3f8dc0fd90f3def`
- Scenes: `1`
- Nodes: `168`
- Meshes: `167`
- Materials: `164`
- Textures: `0`
- Images: `0`

Key material names preserved in the GLB:

- `M_Island_GrassTop`
- `M_Island_SoilSide`
- `M_Island_DarkBottom`
- `M_LowPoly_GrassPuff_A/B/C`
- `M_LowPoly_Stone_Tan_A/B`
- `M_LowPoly_Flower_Yellow_Strong`
- `M_LowPoly_Flower_Pink_Strong`

The first export still contained texture-node interference and produced default gray factors on some style materials. The final export rebuilds material nodes as flat colors before export, and direct GLB JSON inspection confirmed the key `baseColorFactor` values are present.
