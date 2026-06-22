# CatLife 非模型画面效果设计方案

生成日期：2026-06-22
适用范围：Unity/Android 实时画面、PPT/海报/视频渲染、Blender Render 分支参考

> 当前权威版：`10-art-guide/CatLife_非模型画面效果深化方案.md`。
> 本文件保留为初稿，后续执行以深化方案为准。

## 1. 设计结论

CatLife 当前模型资产已经具备低多边形猫咪小镇的核心辨识度。下一阶段最影响画面观感的不是继续加模型，而是补齐“天空、光照、后处理、粒子、状态反馈、镜头构图”这六层非模型内容。

主视觉方向定为：**晴朗午后专注花园**。

关键词：浅蓝天空、奶油暖光、柔绿草地、橙白猫、低强度漂浮粒子、状态化光效、轻微景深感但不使用重景深。

不采用：赛博霓虹、暗夜强发光、写实 HDRI、浓雾、强眩光、过多闪烁粒子。原因是这些会与“陪伴式专注辅助”冲突，把产品误导成宠物游戏或奇幻游戏。

## 2. 现有画面诊断

参考 `06-deliverables/catlife_preview_cam45.png`：

| 层级 | 当前问题 | 设计方向 |
|---|---|---|
| 背景 | 灰色空背景，像 Blender 工作台预览 | 换成浅蓝到奶白的卡通天空盒 |
| 光照 | 模型可见，但缺少品牌化时间氛围 | 使用暖向主光 + 冷色天光 + 低强度补光 |
| 阴影 | 暗部偏硬，局部容易显脏 | 控制阴影距离，使用柔和 blob/contact shadow |
| 画面空气感 | 前中后层次弱 | 远景用轻微雾色融合天空 |
| 互动反馈 | 画面静态，缺少状态表达 | 用低密度粒子绑定普通/过渡/专注/奖励四状态 |
| 移动端风险 | Runtime 仍有 875k triangles，不能再重度堆 GPU 效果 | 粒子、后处理、阴影都按移动端预算设计 |

## 3. 天空盒设计

### 3.1 主天空盒：`SKY_DayFocus_Garden`

建议使用 **procedural gradient skybox 或 6-sided low-poly skybox**，避免写实 HDRI。

| 项 | 设定 |
|---|---|
| 顶部色 | `#A8DDF5` 浅晴空蓝 |
| 中段色 | `#DFF3F7` 低饱和蓝白 |
| 地平线色 | `#FFF0D0` 奶油暖白 |
| 太阳方向 | 左前上方 35-45 度 |
| 云 | 3-5 组低多边形软云剪影，不做真实体积云 |
| 地平线 | 极浅色远山/树线，可用 skybox 贴图画死 |

Unity 落地：

1. `Window > Rendering > Lighting` 中设置全局 Skybox Material。
2. Skybox 与 Fog 色保持一致，避免地平线突兀。
3. 如果运行时切换天空盒，需要同步更新环境光探针。

### 3.2 状态天空变体

| 状态 | 天空变化 | 目的 |
|---|---|---|
| 普通 | 标准浅蓝天 | 保持轻快 |
| 过渡 | 饱和度降低 5%，云层稍慢移动 | 暗示从活跃转入安静 |
| 专注 | 顶部蓝更淡，地平线更奶白 | 降低刺激源 |
| 奖励 | 地平线暖黄提高，短时星点/爪印粒子 | 完成反馈 |

注意：状态变化只做 5-10% 的色温和粒子变化，不做大幅昼夜切换。

## 4. 光照方案

### 4.1 Runtime 移动端光照

| 光源 | 类型 | 设定 |
|---|---|---|
| `LGT_Sun_Key` | Directional Light | 暖白 `#FFE2B0`，Intensity 0.9-1.1 |
| 环境光 | Skybox/Ambient | 浅蓝灰 `#CFEAF2`，Intensity 0.6-0.8 |
| 局部补光 | 尽量不用实时点光 | 路灯/窗户用 Emission 材质替代 |
| 阴影 | 主光阴影 | 只让关键建筑/猫/大树投影 |

移动端建议：

| 项 | 建议 |
|---|---|
| Main Light Shadows | 开，但 Shadow Distance 控制在 25-35 |
| Cascade Count | 1 或 2，不用 4 |
| Additional Light Shadows | 关 |
| Soft Shadows | 中高端开，低端关 |
| Light Probe | 可用于猫和动态道具 |
| Reflection Probe | 低优先级，材质不走强 PBR |

### 4.2 Render 分支灯光

用于 PPT/海报/视频的 `CatLife_render.blend` 可以更漂亮：

| 光源 | 用途 |
|---|---|
| Key Sun | 左前 45 度，暖光，塑造屋顶和石路高光 |
| Area Fill | 右前大面积低强度补光，避免猫屋暗部死黑 |
| Soft Top | 顶部柔光，让星星树和猫脸更干净 |
| Rim Light | 只在海报/猫特写启用，分离主体和背景 |

## 5. 后处理方案

Unity URP 使用 Global Volume，不使用 Post Processing Stack v2。

### 5.1 标准移动端档

| 效果 | 参数建议 | 说明 |
|---|---|---|
| Color Adjustments | Saturation +5 到 +8，Contrast +4 到 +8，Color Filter `#FFF4E0` 低强度 | 统一奶油暖调 |
| Bloom | Intensity 0.08-0.18，Threshold 1.1-1.3，High Quality Filtering 关闭 | 只给灯、星星、奖励粒子轻微发光 |
| Vignette | Intensity 0.08-0.12，Smoothness 0.55 | 聚焦中心广场，不显黑边 |
| White Balance | Temperature +3 到 +6，Tint -2 到 0 | 保持暖但不偏黄 |
| Anti-aliasing | FXAA 或 2x MSAA 二选一实测 | 避免低模边缘过锯齿 |

禁用项：

| 效果 | 原因 |
|---|---|
| Motion Blur | 专注产品不应制造视觉拖影 |
| Chromatic Aberration | 破坏干净治愈感 |
| Lens Distortion | UI/小镇边缘会变形 |
| Film Grain | 低模卡通不需要颗粒噪声 |
| Bokeh Depth of Field | 移动端成本高，且会影响看清功能 |

### 5.2 状态化后处理

| 状态 | 后处理变化 |
|---|---|
| 普通 | 标准饱和度 |
| 过渡 | Saturation -3，Vignette +0.02 |
| 专注 | Saturation -5，Bloom 仅保留灯/星点，Vignette +0.04 |
| 奖励 | Bloom 短时 +0.08，Saturation +5，0.8 秒内回落 |

后处理参数必须缓动，禁止瞬间跳变。

## 6. 粒子效果设计

整体原则：**粒子用于表达状态，不用于刷屏装饰**。

优先使用 Unity Particle System。Visual Effect Graph 更适合大量 GPU 粒子，但 CatLife 移动端不需要“百万粒子”能力，且当前美术目标是轻量、稳定、可控。

### 6.1 粒子材质规范

| 项 | 规范 |
|---|---|
| 贴图 | 1 张 `FX_CatLife_Atlas_512.png` 图集 |
| 图集内容 | 圆点、四角星、猫爪、叶片、花瓣、微光 |
| Shader | URP Unlit Transparent 或 Additive Soft |
| Sorting | 粒子分层，不穿插 UI |
| 颜色 | 只用 `#F8EFCB`、`#F5D966`、`#FFFFFF`、`#BFEA7A`、`#F7B3A0` |

### 6.2 环境粒子

| 粒子 | 位置 | 参数 | 用途 |
|---|---|---|---|
| `FX_Ambient_DustMotes` | 中心广场上方 | 20-35 粒，生命周期 4-7s，速度极慢 | 空气感 |
| `FX_LeafDrift` | 树线边缘 | 8-15 粒，随机横向风 | 自然感 |
| `FX_FlowerSpecks` | 花丛周围 | 低频弹出，非循环密集 | 提醒资产细节 |

### 6.3 四状态粒子

| 状态 | 粒子 | 表现 |
|---|---|---|
| 普通 | `FX_Cat_Active_PawDots` | 猫附近少量爪印/圆点，速度略快 |
| 过渡 | `FX_Transition_SoftRing` | 中心广场出现 1 圈浅金扩散环，透明度低 |
| 专注 | `FX_Focus_BreathingStars` | 猫/专注小屋附近 6-12 个慢速呼吸星点 |
| 奖励 | `FX_Reward_StarPawBurst` | 0.8-1.2s 小爆发，星星和猫爪向上飘，不超过 45 粒 |

### 6.4 粒子预算

| 档位 | 同屏粒子数 | 用途 |
|---|---:|---|
| Low | 25-40 | 低端 Android |
| Default | 60-90 | 默认目标 |
| High | 100-140 | 录屏/高端机 |

硬性限制：

1. 不使用粒子碰撞。
2. 不使用大面积半透明烟雾。
3. 单个粒子屏幕尺寸不超过画面高度 4%。
4. Reward 爆发粒子持续时间不超过 1.2 秒。
5. 粒子系统总数优先合并，避免每个道具挂一个循环粒子。

## 7. 画面层次与非模型补充

### 7.1 地面与边缘

| 内容 | 做法 |
|---|---|
| 草地边缘 | 用浅绿色渐变 decal 或顶点色过渡，避免纯色大平面 |
| 小镇边界 | 在岛边加浅雾/柔和描边，不新增复杂模型 |
| 石路 | 保留奶白石路，后处理提高亮部，不做写实粗糙贴图 |

### 7.2 阴影与接触感

移动端优先用“假阴影”补接触感：

| 物体 | 阴影方式 |
|---|---|
| 猫 | 椭圆 blob shadow，跟随状态缩放 |
| 道具 | 静态浅色 contact decal |
| 大建筑/树 | 主光实时或烘焙阴影 |
| 小花草 | 不投影 |

### 7.3 UI 与场景关系

专注 UI 出现时，场景不应变暗到游戏暂停感。建议：

| UI 状态 | 场景处理 |
|---|---|
| 轻锁定 | 背景饱和度 -5，中心猫/计时器保留亮度 |
| 专注计时 | 粒子从活跃变慢，星点呼吸周期 3-4s |
| 退出提示 | 不做红色警告，使用奶白面板 + 暖橙确认 |

## 8. 镜头与构图

### 8.1 实时主镜头

| 项 | 建议 |
|---|---|
| 类型 | Orthographic 或低透视 Perspective |
| 角度 | 35-45 度俯视 |
| 中心 | 猫/圆形广场/专注小屋三者形成三角 |
| 镜头运动 | 只做轻微 idle drift，不做频繁旋转 |
| UI 安全区 | 底部和右侧预留 18-22% |

### 8.2 渲染镜头

| 镜头 | 用途 |
|---|---|
| `CAM_45` | PPT 主图、官网式展示 |
| `CAM_Poster_Vertical` | 海报竖版主视觉 |
| `CAM_Cat_Closeup` | 答辩封面、视频片头 |
| `CAM_Top` | 功能流程解释、路线图 |

当前 `catlife_preview_cam45.png` 左右边缘有建筑裁切。PPT 主图可接受，海报和 App 首屏需要重新取景，避免主体被切。

## 9. Unity 落地清单

### 9.1 资产命名

```text
Assets/CatLife/Art/FX/
  FX_CatLife_Atlas_512.png
  MAT_FX_SoftAdditive.mat
  MAT_FX_AlphaBlend.mat
  PS_Ambient_DustMotes.prefab
  PS_LeafDrift.prefab
  PS_Focus_BreathingStars.prefab
  PS_Reward_StarPawBurst.prefab

Assets/CatLife/Art/Sky/
  MAT_SKY_DayFocus_Garden.mat
  TEX_SKY_DayFocus_6Sided/

Assets/CatLife/Rendering/
  URP_CatLife_Mobile.asset
  VOL_CatLife_GlobalProfile.asset
  RENDERSET_CatLife_Runtime.md
```

### 9.2 Scene 组件

```text
SceneRoot
  Rendering
    Directional Light: LGT_Sun_Key
    Global Volume: VOL_CatLife_Global
    Reflection Probe: optional, low resolution
  FX
    FX_Ambient
    FX_State
  Camera
    Main Camera
```

### 9.3 状态驱动接口

建议 Unity 状态机暴露一个视觉接口：

```csharp
public enum FocusVisualState
{
    Normal,
    Transition,
    Focus,
    Reward
}

public interface IFocusVisualDriver
{
    void SetVisualState(FocusVisualState state, float transitionSeconds = 0.8f);
}
```

由 `FocusVisualDriver` 统一控制：

- 后处理参数缓动。
- 粒子 emission rate。
- 猫/主场景 blob shadow 强度。
- 天空色温微调。

## 10. 移动端验收指标

| 项 | 目标 |
|---|---:|
| FPS | 中端 Android 稳定 30fps，目标 45fps+ |
| 同屏粒子 | 默认不超过 90 |
| 后处理 | Bloom + Color Adjustments + Vignette 三项以内 |
| 主光阴影距离 | 25-35 |
| Additional Light Shadows | 0 |
| 粒子贴图 | 1 张 512 图集 |
| 透明 overdraw | Scene 视图 Overdraw 模式下不能覆盖大半屏 |
| 视觉状态切换 | 0.6-1.0s 缓动，无闪烁 |

## 11. P0-P3 执行优先级

### P0：立刻补齐

1. 替换灰背景为 `SKY_DayFocus_Garden`。
2. 配置 `LGT_Sun_Key` 与 Global Volume。
3. 加 `FX_Ambient_DustMotes` 和 `FX_Focus_BreathingStars` 两个最小粒子。
4. 用 `catlife_preview_cam45.png` 同构图重新输出一张“有天空、有后处理、有粒子”的对比图。

### P1：影响复赛展示

1. 做四状态视觉变化。
2. 输出竖版海报镜头主图。
3. 为视频录制准备 Reward 粒子爆发。
4. 做移动端低/中/高三档 FX 开关。

### P2：增强质感

1. 增加低模云和远景树线。
2. 增加草地边缘 decal。
3. 增加猫脚下 blob shadow。

### P3：最终打磨

1. 镜头 idle drift。
2. 小镇边缘轻雾。
3. 奖励状态短时色温变化。

## 12. 调研依据

- Unity Skybox 可设置为全局 Render Settings，也可针对 Camera 覆盖；项目应优先使用全局 Skybox，保证所有镜头一致。参考：<https://docs.unity3d.com/462/Documentation/Manual/class-Skybox.html>
- Unity `RenderSettings.skybox` 运行时切换后需要更新环境探针；若后续按状态换天空盒，需要注意这一点。参考：<https://docs.unity3d.com/6000.4/Documentation/ScriptReference/RenderSettings-skybox.html>
- URP 移动端后处理推荐优先使用 Bloom、Color Grading/Color Adjustments、Vignette 等移动端相对友好的效果，并控制成本。参考：<https://docs.unity3d.com/6000.4/Documentation/Manual/urp/integration-with-post-processing.html>
- URP 阴影性能主要受可见投影物体数量、Shadow Distance、Shadow Cascade 和投影光数量影响；CatLife 应减少实时投影和额外光阴影。参考：<https://docs.unity3d.com/6000.0/Documentation/Manual/shadows-optimization.html>
- Unity 移动端图形优化强调 fillrate/overdraw，粒子和透明层需要特别克制。参考：<https://docs.unity3d.com/2019.4/Documentation/Manual/MobileOptimisation.html>
- Unity Particle System 的模块化系统适合本项目低密度、状态化粒子；Color/Size/Trails 等模块可完成需要的反馈，不必默认上 VFX Graph。参考：<https://docs.unity3d.com/6000.4/Documentation/Manual/ParticleSystemModules.html>
