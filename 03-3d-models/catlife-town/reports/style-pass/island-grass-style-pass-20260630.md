# CatLife 小镇空岛底座与草地风格 Pass

日期：2026-06-30

## 目标

按最新两张 CatLife 主页参考图，把 Blender 内猫咪小镇空岛底座调整为更接近低多边形童话风：

- 空岛上表面更明亮的黄绿色草地；
- 空岛侧面保持暖棕土层；
- 增加少量低模草团、暖色小石头和彩色花点；
- 不再生成漂浮、跑出空岛或纯白的草/石头。

## 当前文件

```text
03-3d-models/catlife-town/current/catlife_v2_island_grass_style_20260630.blend
```

操作前检查点：

```text
03-3d-models/catlife-town/archive/style-pass-checkpoints/catlife_v2_before_island_grass_style_20260630.blend
```

最新预览：

```text
03-3d-models/catlife-town/reports/style-pass/catlife_v2_island_grass_style_preview_grass_puffs_20260630.png
```

## 重要修正

第一版错误使用外接椭圆和固定高度估算空岛表面，导致草/石头看起来跑到空岛外或悬浮。已改为从底座对象 `柱体` 的真实 `M_Island_GrassTop` 顶面面片采样。

最终生成对象：

| 对象 | 内容 | 面数 | 材质 |
|---|---|---:|---|
| `CL_StylePass_LowPoly_GrassPuffs_OnIsland` | 绿色低模草团/小灌木块 | 806 | `M_LowPoly_GrassPuff_A/B/C` |
| `CL_StylePass_LowPoly_TanStones_OnIsland` | 暖灰/米色低模小石头 | 84 | `M_LowPoly_Stone_Tan_A/B` |
| `CL_StylePass_LowPoly_ColorFlowers_OnIsland` | 黄/粉色小花点 | 35 | `M_LowPoly_Flower_Yellow_Strong`、`M_LowPoly_Flower_Pink_Strong` |

采样高度范围：

```text
surface_z_minmax = -0.0334 ~ 0.1654
```

这与 `柱体` 草坪顶面高度一致，不再使用错误的 `z≈0.398`。

## 材质调整

| 材质 | 目标 |
|---|---|
| `M_Island_GrassTop` | 更亮的黄绿色草坪 |
| `M_Island_SoilSide` | 暖棕色侧壁土层 |
| `M_Island_DarkBottom` | 深棕底部 |
| `M_LowPoly_GrassPuff_*` | 可见绿色草团，避免灰白细碎草叶 |
| `M_LowPoly_Stone_Tan_*` | 暖灰/米色小石头，避免纯白 |

所有新增对象都设置了 `object.color` 和材质 `diffuse_color`，便于 Blender 实体视图和材质预览中识别颜色。

## 参考图风格还需要的 Blender 工作

| 项 | 是否建议在 Blender 做 | 说明 |
|---|---|---|
| 草地颜色和空岛侧壁颜色 | 是 | 当前已做，后续可以继续按真机截图微调饱和度和亮度 |
| 低模草团/花点/石头 | 是 | 当前已做，建议保持低面数，避免移动端负担 |
| 软光和低反光材质 | 是 | 建筑、招牌、屋顶可继续降低高光，统一玩具感 |
| 天空蓝和云 | 只做预览，不建议烘死到模型 | Unity 中更适合用天空盒/后处理控制；Blender 可保留 `CL_StylePass_Sky_*` 预览对象 |
| 背景虚化/景深 | 主要在 Unity 相机做 | Blender 里只用于出图验证，最终 App 以 Unity 摄像机和 UI 叠加为准 |
| 黑色边线/毛刺 | 需要继续检查 | 优先检查重叠面、法线、材质背面和 z-fighting；不要再合并全场景网格 |
| 移动端性能 | Blender 侧先控制新增面数 | 本次新增约 925 面，影响很小；真正性能要在 Unity/Android Profiler 验证 |

## 后续验收

| 编号 | 标准 |
|---|---|
| STYLE-01 | 关闭 UI 后，小镇底座上表面为明亮低模草地，不再是过暗纯绿 |
| STYLE-02 | 新增草/石头/花都在空岛顶面，不漂浮、不跑到岛外 |
| STYLE-03 | 新增对象无纯白材质；石头为暖灰/米色，草为绿色 |
| STYLE-04 | Unity 导出前可按 `CL_StylePass_` 前缀统一选择、隐藏或导出 |
| STYLE-05 | 正式提交截图应使用 Unity/Android Game View，而不是 Blender 工作视口 |
