# CatLife 猫咪小镇场景 Unity 落地方案

日期：2026-06-29
目标：把 Blender 猫咪小镇从当前安全源文件落到 Unity MVP 主场景，并保证可运行、可录制、可优化。

## 1. 当前资产选择

生产源文件只使用：

```text
03-3d-models/catlife-town/current/catlife_v2_view_clean_no_merge.blend
```

可参考导出：

```text
03-3d-models/catlife-town/exports/catlife_v2.fbx
03-3d-models/catlife-town/source/catlife_full_scene.glb
03-3d-models/catlife-town/source/individual-glb/
```

禁止作为生产文件：

```text
03-3d-models/catlife-town/archive/deprecated-merge-attempts/
```

原因：mesh merge 尝试会破坏材质、UV 和硬边表现，用户已肉眼确认合并后视觉错误。

## 2. 导入策略

优先策略：模块化导入，而不是一次性把完整小镇重资产塞入最终工程。

| 阶段 | 导入内容 | 目标 |
|---|---|---|
| A | 地面 + 中心广场 | 校准尺度、坐标、摄像机 |
| B | 入口门头 + 三个主建筑 | 建立第一眼视觉 |
| C | 树木、栅栏、花草 | 检查 Draw Call 和重复物体优化 |
| D | 小道具、灯、桌椅、标牌 | 补足镜头细节 |
| E | 动画猫 | 确认猫和小镇空间关系 |

如果时间不足，MVP 只需要 A+B+E 加少量 C，就能支撑演示视频。

## 3. 坐标、尺度与摆位

| 项 | 规则 |
|---|---|
| Blender 源约定 | 保持当前 no-merge 文件，不在 Blender 再全局应用 scale |
| Unity 场景 | 以 Unity `Y` 为 up，导入器处理坐标转换 |
| 猫咪方向 | 沿用动画验证结论：头部方向应在 Unity 场景中符合镜头和交互需要，不再直接改骨骼源 |
| 尺度 | 以猫身高约 1.2m 的 MVP 验证结果为视觉基准，小镇门、长椅、桌椅围绕猫重新校准 |
| 地面 | 地面 y=0，猫脚底贴地，不允许默认飞起或陷入地面 |

## 4. 黑色细线毛刺处理

已验证的安全处理在 Blender 侧是“视口/材质显示修正”，不是拓扑合并：

- 关闭 viewport outline、cavity、wire overlay；
- 修正 opaque 材质的 alpha、transparent backface、transparent shadow；
- 保留原始拓扑、UV、法线和对象 transform；
- 不合并顶点，不批量应用 scale。

Unity 侧如果仍出现黑线，按以下顺序排查：

1. 透明/半透明材质：把不需要透明的材质改为 Opaque。
2. Z-fighting：查找重叠地面、重复装饰片、双层面片。
3. 法线/背面：检查 Backface Culling、双面材质和反向法线。
4. 阴影：先关闭小物件阴影投射，再逐项恢复。
5. 贴图 mipmap：远距离闪烁或黑边优先检查 mipmap、filter mode 和 texture padding。

## 5. Unity 目标目录

建议落位：

```text
Assets/Art/Town/Models/
Assets/Art/Town/Materials/
Assets/Art/Town/Textures/
Assets/Art/Town/Prefabs/
Assets/Scenes/mainscene.unity
```

交付包同步位置：

```text
06-deliverables/unity-handoff-20260629/mvp-unity-assets/Assets/Art/Town/
```

注意：大体积 `.fbx/.glb/.blend` 默认不进 Git，除非明确切换 Git LFS 或压缩交付包路线。

## 6. Prefab 结构建议

```text
CatLifeTownRoot
├── Ground
├── Plaza
├── Buildings
│   ├── Gate
│   ├── TomatoClockTower
│   ├── CatHouse
│   └── FishSnackShop
├── Props
│   ├── Benches
│   ├── Lamps
│   ├── Tables
│   └── Signs
├── Vegetation
│   ├── Trees
│   ├── Bushes
│   └── Flowers
└── RuntimeAnchors
    ├── CatSpawn
    ├── CameraOverview
    ├── CameraCatCloseup
    └── FocusInteractionPoint
```

## 7. 首轮验收镜头

| 镜头 | 内容 | 目的 |
|---|---|---|
| 全景 | 小镇、入口、中心广场、主建筑 | 证明场景已落地 |
| 猫特写 | 动画猫站在广场或猫窝前 | 证明角色和场景融合 |
| 状态变化 | 普通、过渡、专注、奖励 | 证明 MVP 逻辑闭环 |
| 性能截图 | Unity Stats 或 Profiler | 证明不是只做静态展示 |

## 8. 完成定义

小镇落地完成必须同时满足：

- `mainscene` 打开后能看到小镇和动画猫；
- 猫大小、方向、贴地正确；
- 黑线毛刺不影响最终 Game View；
- 首轮性能数字被记录；
- 录屏镜头可直接用于演示视频；
- 所用源文件、导出文件、交付包位置在文档中同步。
