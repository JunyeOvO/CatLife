# CatLife 非模型画面效果深化方案

> 整理说明：本文由 GPT 深度研究结果清洗入库，已去除不可复用的内部 citation 标记，并作为 CatLife 非模型画面效果当前深化版方案。
>
> 优先级：本文件优先于 `CatLife_非模型画面效果设计方案.md` 初稿。
## 结论摘要

对 CatLife 这类“低多边形、治愈、低负担、Android 友好”的专注陪伴应用，我的最终推荐不是继续堆模型，而是把画面主策略定为**一套非常克制的 URP 前向渲染基线**：一张干净的天空、单主光方向光、少量或零实时补光、轻量 Volume 后处理、低密度粒子、明确但不吵闹的四状态视觉反馈。这样做最符合移动端约束，因为 Unity 与 Android 官方都明确建议移动项目优先使用 URP、尽量依赖烘焙与探针、避免大量动态灯光与高带宽后处理；而 CatLife 当前 Runtime 模型已经偏重，更不适合再把“氛围感”建立在点光阴影、复杂透明烟雾和重后处理上。

在视觉方向上，**“晴朗午后专注花园”仍然是最优方向**，但应从“明亮可爱”微调为**“奶油蓝天、暖阳、低对比、低刺激的静态午后”**。原因很简单：午后方向可以主要依赖一盏 Directional Light 加环境光完成体积关系；相反，黄昏、夜景或高戏剧化天气会天然推高局部灯、发光材质、阴影和氛围特效的依赖，而这些正是移动端最容易出成本的位置。Android 官方移动灯光指南直接指出：前向渲染下实时灯光很贵，动态灯光总体上应尽量避免，方向光是最便宜的实时灯光，而点光尤其昂贵。

技术上，我建议 CatLife 采用**三档质量预设**：低端机保底流畅、默认档用于正式 APK、中高质量档用于录屏与比赛展示。三档都应以 **Forward Renderer** 为核心，而不是 Deferred；因为 Android 官方对移动游戏的建议明确指出，Deferred 在移动 GPU 上往往受带宽限制，URP 更适合作为移动项目的默认路线。

如果只能记住一句话，那就是：**把 CatLife 的画面深化建立在“天空、主光、调色、轻粒子、轻状态变化”上，而不是建立在“更多灯、更多透明、更多模型、更多动效”上。** 这条原则既贴合项目气质，也贴合 Unity/Android 的移动端性能规律。

## 视觉方向校准

### 最终推荐方向

我建议把主视觉正式定为：

**奶油蓝天的静态午后专注花园**

它其实是“晴朗午后专注花园”的优化版：保留明快、治愈、可展示的优势，但压低光比、压低饱和、压低镜头刺激，避免把 CatLife 做成“宠物游戏式热闹场景”或“黄昏滤镜式情绪短片”。这条方向最适合 CatLife 的产品本质——它是一个**专注陪伴工具**，不是一个以兴奋和刺激为目标的休闲游戏。结合技术约束看，这条方向也最省：一盏主方向光就够用，大部分氛围可以由 Skybox、环境光、轻 Bloom、轻 Vignette 和少量空气粒子承担。

### 备选方向对比

| 方向 | 天空 | 光照与色温 | 粒子气质 | UI 融合 | 复赛展示效果 | 移动端风险 |
|---|---|---|---|---|---|---|
| **奶油蓝天静态午后** | 浅蓝到奶油白渐变，少云 | 暖主光 + 冷天光，低对比 | 空气尘点、轻叶片、少量星点 | 最容易和浅色卡片 UI 融合 | 稳定、温暖、截图成功率高 | **最低** |
| 薄云晨光学习镇 | 更白、更淡、更安静 | 对比更低，偏中性暖 | 颗粒更少，几乎无闪点 | 最不打扰 UI | 很适合产品演示 | 稍显“淡”，海报冲击力弱 |
| 金色傍晚奖励庭院 | 地平线暖橙更重 | 需要更强局部高光和更长阴影 | 奖励粒子更显眼 | UI 更易被背景抢戏 | 海报好看 | **较高**，更容易诱发补光、点灯、重 Bloom |

**为什么不选黄昏/傍晚为主方向**：黄昏一旦想做得“好看”，通常会自然引入更强的局部灯、发光窗户、补光点灯、暖冷对撞和更明显 Bloom，而移动前向渲染对实时灯光特别敏感。Android 官方指南明确指出，实时灯很贵，方向光虽便宜，但点光成本高、阴影更贵；这与 CatLife 当前模型基数偏重的现状叠加后，风险会明显增大。

### 推荐色板

下面这组色值不是 Unity 默认值，而是针对 CatLife 的**项目建议值**，目标是把“暖橙、奶白、浅绿、星星金”收敛到同一低刺激体系中：

| 用途 | 建议色值 |
|---|---|
| 天空顶部 | `#A8D6FF` |
| 天空中段 | `#CFE8FF` |
| 地平线奶油白 | `#F7F2E7` |
| 云层高光 | `#FFF9F0` |
| 环境天光冷色 | `#D7E7F4` |
| 主光暖色 | `#FFE2B8` |
| 草地主色 | `#BFD7A8` |
| 奶白墙体参考 | `#F4EBDD` |
| 屋顶暖橙 | `#E8A45E` |
| 星点奖励金 | `#F6D67A` |

这套色板的关键不是“更鲜艳”，而是**天空更干净、太阳更奶、草地更柔、奖励更亮但不发荧光**。落地时应优先通过 Skybox、主光色温、Color Adjustments、White Balance 去统一，而不是再新加一批模型材质。Unity 的 Skybox、环境光、色温与 Volume 系统都支持这种做法。

## URP 移动端渲染基线

### 渲染路径与总体原则

CatLife 的 Runtime 基线应使用 **URP + Forward Renderer**。Android 官方明确建议移动游戏优先使用 URP；同时指出 Deferred 在移动 GPU 上通常不理想，因为带宽较低。CatLife 不是“多实时灯场景”，因此没有必要为了极少数局部补光把整套项目推向 Forward+ 或 Deferred。

渲染材质层面，静态场景优先使用 **Baked Lit 或 Simple Lit**，动态对象如猫与少量交互物体使用 **Simple Lit**，不要在 APK 基线里使用 Complex Lit。Unity 官方写得很直接：在低端移动平台上，静态对象优先 Baked Lit，动态对象优先 Simple Lit，Complex Lit 成本更高。

同时，应确保 **SRP Batcher 开启**，并把“新增视觉效果”限制在尽量少的 shader 变体里。Unity 官方指出，SRP Batcher 的最佳实践是尽量减少 shader variants；同一个 shader 可有多个材质，但变体越少越利于 CPU 侧渲染提交。

### 三档推荐配置

下面这张表是 CatLife 更适合的**项目建议值**，不是官方硬性阈值；它是结合 Unity/Android 的移动渲染原则，针对你们“875k triangles 模型偏重、非模型氛围必须克制”的前提做出的工程目标。

| 项目 | 低端机保底档 | 默认 APK 档 | 录屏展示档 |
|---|---:|---:|---:|
| Render Path | Forward | Forward | Forward |
| Render Scale | 0.80 | 0.90 | 1.00 |
| Upscaling Filter | Bilinear / Automatic | Bilinear / Automatic | Automatic |
| HDR | Off | On | On |
| HDR Precision | — | 32-bit | 32-bit |
| MSAA | Off | 2x | 4x |
| Camera AA | FXAA | None 或 FXAA | SMAA Low / Medium |
| Opaque Texture | Off | Off | Off |
| Depth Texture | Off | Off | Off，除非特定镜头/效果必须 |
| Store Actions | Auto / Discard | Auto / Discard | Auto |
| Main Light | On | On | On |
| Additional Lights | Disabled 或 Per Vertex 1-2 | Per Vertex 2 | Per Pixel 2-4 |
| Additional Light Shadows | Off | Off | Off |
| Shadow Distance | 20–24 m | 28–32 m | 35–40 m |
| Cascades | 1 | 2 | 4 |
| Soft Shadows | Off | Low | Medium |
| Post Processing | Color + Vignette | Light Bloom + Color + Vignette + WB | Bloom + Color + Vignette + WB |

这些设置的依据很明确。URP Asset 允许你为不同硬件准备多套 Asset 并按质量级切换；Render Scale 只影响游戏渲染而不影响 UI 原始分辨率；HDR 会增加亮度范围和 Bloom 可用性，但 Unity 同时明确说明，针对低端硬件可以关闭 HDR 以跳过相关计算；Soft Shadows 的性能影响高，而 Low 档是 Unity 专门标注为“适合移动平台的质量/成本折中”；FXAA 则是 Unity 现在仍推荐给移动平台的抗锯齿方案。

### 必须避免的运行时依赖

有三件事，不建议进 CatLife 的默认 APK 基线。

第一，**不要把 UI 背景毛玻璃或屏幕折射作为核心语言**。URP 的 Opaque Texture 本质上是透明物体绘制前的一次场景拷贝，Unity 与 Android 官方都指出它会增加额外拷贝和 GPU 带宽成本。更糟的是，Unity 还说明在部分不支持特定 store action 的移动平台上，若开启 Opaque Texture，MSAA 可能在运行时被忽略。CatLife 这种轻 UI 工具，不值得为一点“玻璃感”换掉清晰度和带宽预算。

第二，**不要把景深、Motion Blur、重 Lens Distortion、Film Grain 当成基线**。URP 旧版移动文档确实把 Bloom、Color Grading、Vignette、Chromatic Aberration、Lens Distortion 列为较“mobile-friendly”的默认效果，但这只是技术上“相对能跑”；从 CatLife 的产品性格看，Chromatic Aberration、Lens Distortion、Grain 这类“镜头存在感很强”的效果会伤害干净、安静、低负担的观感。CatLife 需要的是“轻后期”，不是“摄影机滤镜存在感”。

第三，**不要依赖 VFX Graph 做主线粒子系统**。Unity 官方到现在仍强调，移动端 compute 支持差异大，只有部分高端移动设备能被官方支持；对于大多数移动应用，推荐直接用内置 Particle System。对要跑 Android 广覆盖的比赛 APK，这条结论非常重要。

## 天空、灯光与后处理

### 天空盒方案

CatLife 的**最终推荐 Runtime Skybox** 是：

**Procedural Skybox 作为可见天空**
**Environment Lighting 采用 Gradient 作为环境光源**
**不在四状态之间频繁切换 Skybox 材质本身**

之所以这么定，是因为 Unity 官方说明：URP/Built-in 支持 Procedural、Panoramic、6 Sided 等 Skybox；Procedural 不需要输入纹理，最省资源，也与 Sun Source 配合最好。与此同时，Lighting 面板的 Environment Lighting 允许使用 **Skybox、Gradient、Color** 三种来源；对 CatLife 这种风格化低模场景来说，用 Gradient 作为真实环境光源，能更稳定地控制“天空冷、地面暖、地平线奶油白”的关系，也避免频繁改 Skybox 后还要更新环境 cubemap。

如果你真的需要让状态影响天空本身，也不要做“换整张天空”的突变。因为 Unity 的 `DynamicGI.UpdateEnvironment()` 会调度环境 cubemap 更新；文档明确说如果设备不支持异步 GPU 回读，这会阻塞线程，而即使支持也可能落后一帧或多帧。对移动端而言，这种状态切换不值得每次都触发。CatLife 的四状态更适合变化**光线、后处理和粒子**，而不是每次都重建环境。

Runtime 可见天空建议如下：

| 项目 | 建议 |
|---|---|
| 类型 | Procedural Skybox |
| Sun Disk | Simple |
| Atmosphere Thickness | 0.75–0.95 |
| Sky Tint | 接近 `#A8D6FF` 的低饱和浅蓝 |
| Ground Color | 接近 `#E9DFC9` 的浅米奶油 |
| Sun Source | 主 Directional Light |
| 云层 | 不在 Skybox 内做复杂云；如需要，只加极少量低模远云或很淡的远景云带 |
| 远山/远树 | 可选；总三角控制在 300–800 以内，仅作地平线剪影 |

如果比赛海报或视频需要更“插画感”的云层，则建议单独做一个 **Panoramic 天空材质**，只在录屏/海报质量档启用，不进默认 APK。Unity 的 Skybox 文档说明，Panoramic Skybox 使用一张球形包裹的 2D 贴图；它更适合做手绘云，但比 Procedural 多了纹理资源和风格维护成本。

### 灯光方案

CatLife 的 Runtime 灯光建议采用五件套：

**一个主 Directional Light + Gradient Ambient + Light Probes + 一个 baked/global reflection 方案 + 少量 Emission 材质。**

这是最符合移动端成本结构的组合。Android 官方明确建议移动端优先使用 baked lights、Light Probes 与 fake lighting，而不是动态灯；Light Probes 的主要用途就是给运动物体提供高质量间接光，而且成本低于实时灯。

#### Runtime 版灯光参数

| 项目 | 建议值 |
|---|---|
| 主光类型 | Directional Light |
| 颜色 | `#FFE2B8` 到 `#FFD9AE` |
| 强度 | 0.95–1.15 |
| Rotation | 俯角 38°–48°；方位朝广场斜打 |
| Shadow Type | Hard 或 Soft Low |
| Main Shadow Resolution | 1024 默认；低端 512–1024 |
| Shadow Distance | 28–32m |
| Cascades | 2 |
| Additional Lights | 默认禁阴影，仅必要时 1–2 盏 Per Vertex |
| Additional Light Shadows | Off |
| Ambient Source | Gradient |
| Sky Color | `#D7E7F4` |
| Equator Color | `#F7F2E7` |
| Ground Color | `#D8CDBB` |
| Light Probes | 广场、道路、室外坐凳、猫活动区域布一圈 |
| Reflection | 1 个 baked global probe，64–128 分辨率，低强度 |

为什么只有一盏真实主光：Android 官方说得非常明确，方向光是最便宜的实时灯，点光在全方向投光且阴影最贵；URP 还进一步说明，Directional Light 的阴影按 cascade 产生 shadow map，而 Point Light 一盏就要六张 shadow maps。CatLife 只要不是夜景，就没有理由在 Android 上开点光阴影。

#### 哪些对象应投影，哪些不应投影

**应投影**：主建筑体块、番茄钟楼、猫主角、大树主干与树冠、奖励树主轮廓、围墙/高栅栏、大型标志物。
**不应投影**：花丛、草片、小路装饰边缘、小摊零碎物、远景云、绝大多数粒子、绝大多数奖励闪点。

原因不是“它们不重要”，而是**细碎物件的阴影往往贡献很小，却会扩大 shadow caster 数量和阴影图复杂度**。Android 官方甚至在移动示例中展示了对动态角色关闭 Cast/Receive Shadows、改用 blob shadow 的实践。对于 CatLife，这种取舍不是退步，而是合适的风格化简化。

#### 实时 APK 与海报/视频的灯光分离

Runtime 保持“单主光 + 探针 + 少量 Emission”即可。海报/视频则可以单独使用一套**Capture 质量档**：

- 主光阴影提高到 2048，Cascades 4，Soft Shadows Medium。
- 允许增加 1–2 盏**不投影**的 Spot 作为边缘补光或角色轮廓光。
- 必要时切换 Environment Lighting Source 为 Skybox，以便让可见天空与环境反射更一致。
- Reflection Probe 允许提高到 128–256，但坚持 baked 或 On Awake，不要持续实时刷新。

### 后处理方案

CatLife 的 Volume 方案应该是**一个全局基础 Volume + 四状态参数扰动**，而不是一堆局部体积来回叠。URP 的 Volume 系统支持全局和局部体积，并会对生效体积进行插值；但从 CatLife 的需求看，状态变化几乎是全局性的，所以一套 Base + State 方案最稳。

#### 最终推荐的基线后处理

| 效果 | 默认 APK 建议 |
|---|---|
| Bloom | 开，但很轻 |
| Bloom Intensity | 0.12–0.22 |
| Bloom Threshold | 1.0–1.2 |
| Bloom Clamp | 2–4 |
| Bloom Filter | **Dual** |
| Bloom Downscale | **Quarter** |
| Bloom Max Iterations | 4 |
| HQ Filtering | Off |
| Color Adjustments Post Exposure | 0 到 +0.05 |
| Contrast | -2 到 +4 |
| Saturation | -4 到 +3 |
| Color Filter | 默认无，状态时轻微偏暖/偏奶 |
| White Balance Temperature | +6 到 +12 |
| White Balance Tint | -2 到 +2 |
| Vignette Intensity | 0.06–0.12 |
| Vignette Smoothness | 0.35–0.45 |
| TAA | 不推荐默认开 |
| Motion Blur | Off |
| Depth of Field | Off |
| Chromatic Aberration | Off |
| Lens Distortion | Off |
| Film Grain | Off |

这些建议与 Unity 文档是匹配的。Bloom 在 Unity 6 文档中已经提供更适合移动的优化选项：**Dual 更快、Kawase 在低分辨率下最快、Quarter Downscale 明显便宜，降低 Max Iterations 也能减少负载**；同时 High Quality Filtering 明确更吃性能。

抗锯齿上，Unity 当前文档仍建议移动平台优先使用 **FXAA**；TAA 虽然能减轻闪烁，但官方也说明它容易出现 ghosting，并且不能与 MSAA、Camera Stacking、Dynamic Resolution 一起使用。对于低模治愈场景和 UI 叠加应用，这种交换不划算。

#### 后处理缓动策略

四状态的后处理变化必须是**慢、轻、可撤回**的，而不是“明显弹一下”。推荐的工程参数如下：

- 普通 → 过渡：`0.6–0.8s`，Ease In Out
- 过渡 → 专注：`1.2–1.8s`，Ease In Out
- 专注 → 奖励：`0.2–0.35s` 攻击 + `0.7–1.0s` 衰减
- 奖励 → 普通：`1.0–1.4s` 回落，不要瞬间切黑或切冷

URP 的 Volume 本来就是按插值工作的；如果只做少数几个全局状态，这种变化完全可以用一个简单的状态控制脚本驱动。

## 粒子、状态反馈与非模型增强

### 为什么用 Particle System 而不是 VFX Graph

CatLife 的主线粒子方案应当全部基于 **内置 Particle System**。Unity 官方对移动端的态度很明确：VFX Graph 在移动端只对一部分高端设备有官方支持，移动设备 compute 支持差异很大，而“对于大多数移动应用，请使用 Unity 内置 Particle System”。

这不仅是“兼容性问题”，也是项目策略问题。CatLife 的粒子不是视觉主角，它们只是用来补空气感、强调状态和奖励反馈；因此更应以**低 overdraw、低材质数、低系统数、可控峰值**为目标。Unity 的移动优化资料同样强调，移动端常常是 fill-rate / overdraw 受限，粒子系统最好共享材质并避免大面积透明覆盖。

### 四类粒子系统设计

#### 环境粒子

这是 CatLife 的“空气感”，不是“特效”。

| 项目 | 建议 |
|---|---|
| Prefab 名称 | `PFX_Ambient_AirDust` |
| 贴图 | 128×128 或 256×256 的软圆点 / 小椭圆 |
| 颜色 | `#FFF6E8`、`#F5F0DA`、极淡浅绿混合 |
| 生命周期 | 5–8s |
| 速度 | 0.02–0.08 |
| Shape | Box，覆盖主活动区 |
| Emission | 4–8 / sec |
| Max Particles | 24 低端 / 40 默认 / 60 录屏 |
| Noise | 很低，0.05–0.12 |
| Renderer | Billboard |
| 混合 | Alpha Blended 或 Premultiply，**不要 Additive** |

环境粒子重点是“慢”和“少”。它们绝对不应把整个屏幕罩成半透明薄雾，因为移动端 overdraw 常常比粒子数量本身更危险。

#### 过渡粒子

用来提示从普通使用进入“准备专注”的节奏切换。

| 项目 | 建议 |
|---|---|
| Prefab 名称 | `PFX_Transition_LeafDrift` |
| 贴图 | 小叶片 / 细小猫爪 / 半抽象纸屑，尽量共用一张 atlas |
| 颜色 | `#E9D7A3`、`#DCE8C5` |
| 生命周期 | 0.9–1.4s |
| 速度 | 0.18–0.35 |
| Shape | Sphere 或 Box，小范围围绕广场或猫 |
| Emission | Burst 4–8 + 短时 Rate 2–4 |
| Max Particles | 峰值 12–18 |
| Renderer | Billboard |
| 混合 | Alpha Premultiply |

它不是“庆祝”，只是轻提醒，所以不能做成大量旋转碎片或长尾拖尾。它的作用更像一阵很轻的风。

#### 专注粒子

专注态应减少噪声，因此专注粒子不是更热闹，而是**更少、更稳、更慢**。

| 项目 | 建议 |
|---|---|
| Prefab 名称 | `PFX_Focus_SoftStar` |
| 贴图 | 极简小星点 / 柔边圆点 |
| 颜色 | `#F7E7B0`、`#FFF4DD` |
| 生命周期 | 2.5–4.0s |
| 速度 | 0.03–0.06 |
| Shape | 小范围围绕猫屋或钟楼基座 |
| Emission | 1–3 / sec |
| Max Particles | 6–12 |
| Size over Lifetime | 缓入缓出 |
| Renderer | Billboard |
| 混合 | Alpha Premultiply，必要时单层 Additive 很弱 |

专注态的目标不是“视觉奖励”，而是“背景轻呼吸”。如果你把专注态做得比普通态更亮更闪，那就是跑偏了。

#### 奖励粒子

奖励是唯一允许稍微亮一点的时刻，但峰值必须短，且不能持续占满同屏透明预算。

| 项目 | 建议 |
|---|---|
| Prefab 名称 | `PFX_Reward_StarBurst` |
| 贴图 | 星点 + 猫爪 + 小圆片，共用 atlas |
| 颜色 | `#F6D67A`、`#FFE9B6`、少量 `#FFD095` |
| 生命周期 | 0.45–1.1s |
| 速度 | 0.4–0.9 |
| Shape | Sphere / Cone，围绕奖励树或猫 |
| Emission | Burst 18–28 |
| Max Particles | 单次峰值 24–36 |
| 子系统 | 可附一层 6–10 粒子的柔星闪点 |
| 混合 | 一层 Alpha，一层极弱 Additive |

奖励特效的设计重点是**短攻击、快回落、少重叠**。它应该像一口轻甜的糖，而不是烟花。

### 粒子预算

在 CatLife 这种模型已经偏重的场景里，粒子活跃预算建议如下：

| 档位 | 常驻活跃粒子 | 峰值活跃粒子 | 同屏粒子系统数 |
|---|---:|---:|---:|
| 低端机 | 30–50 | 70–90 | 3–5 |
| 默认 APK | 45–70 | 110–130 | 4–7 |
| 录屏展示 | 80–120 | 180–220 | 6–10 |

这个预算是项目建议值，不是 Unity 硬门槛。它的核心思想是：**把粒子峰值留给奖励态，平时尽量“看不出系统存在，但能感觉空气更活”**。同时，所有粒子最好共用 **2–4 个材质**，以免在当前本就材质偏碎的场景里进一步抬高 SetPass 和批次成本。Unity 关于 draw call、批处理与共享 shader variant 的说明都支持这种做法。

### 非模型画面增强手段优先级

| 手段 | 适合 CatLife | 移动端成本 | 实现建议 | 优先级 |
|---|---|---|---|---|
| Blob Shadow | 很适合 | 低 | 猫与关键动体用假阴影替代细碎实时阴影 | 很高 |
| 轻雾 / 远景空气透视 | 适合 | 低到中 | 用 RenderSettings Fog，不做体积雾 | 很高 |
| 草地边缘颜色过渡 | 适合 | 低 | 靠材质/顶点色完成，不靠额外模型 | 很高 |
| 低模远云 / 远山剪影 | 适合 | 低 | 总 tris 极低，仅补地平线层次 | 中 |
| 镜头 Idle Drift | 适合但要克制 | 极低 | 普通态轻微，专注态几乎静止 | 中 |
| Decal | 仅局部适合 | 中到高 | APK 不建议依赖 URP 内建 Decal | 低 |
| Contact Shadow / SSAO | 不推荐基线使用 | 中到高 | 可供海报/视频档尝试 | 低 |

这里尤其要强调两点。

第一，**Blob Shadow 非常适合 CatLife**。Android 官方移动灯光示例就展示了关闭 Cast/Receive Shadows、使用 blob 方法照顾动态对象的思路。对猫、奖励树动态部件和部分小道具来说，假阴影比真实阴影更稳定，也更匹配低模风格。

第二，**URP Decal 不建议作为 APK 基线**。Unity 文档明确指出，Decal Renderer Feature 需要 DepthNormal prepass，这对 tile-based GPU 更不高效。CatLife 若想做地面猫爪、奖励落点、路面细节，尽量用预烘焙材质、简单投影平面或本身就画进纹理/顶点色的方法，而不是强依赖运行时 Decal。

另外，**轻雾建议用全局 Fog，不要做体积雾**。Unity 的 Lighting Window 允许直接使用线性或指数雾，它足以提供远景空气透视和“灰棚感”修正；CatLife 不需要体积光切片、GodRay 或 raymarch 雾。

## 四状态视觉语言

CatLife 的四状态不应设计成四套“不同场景”，而应设计成**同一场景的四个呼吸层级**。这样既能避免状态壁垒太强，也能最大化复用天空、灯光、体积配置和材料体系。

### 普通状态

普通态对应高频操作、猫活跃、用户仍在任务外缘。

| 维度 | 建议 |
|---|---|
| 天空/光照 | 基线天空；主光 1.0；天光正常 |
| 后处理 | Bloom 0.14；Vignette 0.06；Saturation 0 |
| 粒子 | 环境尘点常驻；可有极少叶片漂移 |
| 场景动态 | 猫活动稍多；远景草叶轻摆 |
| UI 背景融合 | 面板透明 6–8%；不做背景模糊 |
| 禁止风险 | 不要把普通态做成“已经在庆祝” |

普通态要“活”，但不能“闹”。如果普通态就已经有明显发光、闪点和较重暗角，用户后面很难感知到专注和奖励的层级差。

### 过渡状态

过渡态是最容易被做错的一层。它不是小奖励，也不是小专注，而是**噪声下降、秩序上升**的提示。

| 维度 | 建议 |
|---|---|
| 天空/光照 | 主光强度 -3% 到 -5%；环境更均衡 |
| 后处理 | Saturation -2；Contrast +1；Temperature +2 |
| 粒子 | 轻叶片/轻猫爪 1 次短 burst |
| 场景动态 | 猫开始收拢动作，镜头 idle drift 降低 |
| UI 背景融合 | UI 底板不透明度 +2% 到 +4% |
| 禁止风险 | 不要出现“突然暗下来”或“突然整体发黄” |

过渡态的感受应该是“安静下来”，不是“切换主题”。

### 专注状态

专注态是 CatLife 最重要的一层。它必须让用户感觉更稳定、更轻、更柔和，而不是更炫。

| 维度 | 建议 |
|---|---|
| 天空/光照 | 可见天空几乎不变；主光略柔；天光稍提 |
| 后处理 | Bloom 0.10–0.14；Vignette 0.10–0.12；Saturation -4；Temperature +4 |
| 粒子 | 仅保留少量专注星点；环境粒子减量 20–35% |
| 场景动态 | 猫安静陪伴；风动和小摆件动画减速 |
| UI 背景融合 | 轻锁定 UI，底板更稳；不用屏幕模糊 |
| 禁止风险 | 不能做成“昏暗”“催眠”“孤立感很强” |

这里我特别建议**专注态减少而不是增加粒子**。很多项目会犯一个错误：为了强调“状态变化”，在专注态反而增加了围绕主体的视觉符号。对专注软件来说，这是反逻辑的。专注态需要的是**视觉噪声最低点**。

### 奖励状态

奖励态是唯一允许明显抬升视觉强度的时刻，但也必须短促、轻巧。

| 维度 | 建议 |
|---|---|
| 天空/光照 | 主光可短时 +5% 到 +8%；不必换天空 |
| 后处理 | Bloom 0.20–0.28；Post Exposure +0.05；Vignette 稍降 |
| 粒子 | StarBurst + 猫爪/星星 burst；0.8–1.2s 内回落 |
| 场景动态 | 猫小庆祝；奖励树/钟楼可有轻动画 |
| UI 背景融合 | 任务完成卡片可加一圈极淡辉光 |
| 禁止风险 | 不要烟花化、不要强闪白、不要长期持续 |

奖励态最常见的误区是“以为比赛展示需要很夸张”。实际上，CatLife 的奖励如果太兴奋，就会推翻前面“轻陪伴、低负担”的品牌承诺。它应该像温柔的认可，而不是爆闪成就动画。

### 建议的状态控制脚本结构

下面这段不是必须原样使用，而是给 Unity 开发的落地骨架。它的设计目标是：**全局状态驱动 Volume、灯光、粒子，不做大规模场景切换。**

```csharp
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum VisualState
{
    Normal,
    Transition,
    Focus,
    Reward
}

public sealed class CatLifeVisualStateController : MonoBehaviour
{
    [Header("References")]
    public Volume globalVolume;
    public Light mainDirectionalLight;

    public ParticleSystem ambientFx;
    public ParticleSystem transitionFx;
    public ParticleSystem focusFx;
    public ParticleSystem rewardFx;

    [Header("Timings")]
    public float normalToTransition = 0.7f;
    public float transitionToFocus = 1.4f;
    public float rewardAttack = 0.25f;
    public float rewardDecay = 0.9f;

    [Header("Directional Light")]
    public float baseLightIntensity = 1.0f;

    private ColorAdjustments colorAdjustments;
    private WhiteBalance whiteBalance;
    private Bloom bloom;
    private Vignette vignette;

    private VisualState currentState;

    private void Awake()
    {
        if (globalVolume == null || globalVolume.profile == null) return;

        globalVolume.profile.TryGet(out colorAdjustments);
        globalVolume.profile.TryGet(out whiteBalance);
        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out vignette);
    }

    public void SetState(VisualState next)
    {
        StopAllCoroutines();
        currentState = next;
        StartCoroutine(BlendToState(next));
    }

    private System.Collections.IEnumerator BlendToState(VisualState state)
    {
        float duration = state switch
        {
            VisualState.Transition => normalToTransition,
            VisualState.Focus => transitionToFocus,
            VisualState.Reward => rewardAttack,
            _ => 0.8f
        };

        // 这里建议把目标参数做成 ScriptableObject 配置表
        float targetBloom = 0.14f;
        float targetVignette = 0.06f;
        float targetPostExposure = 0f;
        float targetSaturation = 0f;
        float targetTemperature = 8f;
        float targetLightIntensity = baseLightIntensity;

        switch (state)
        {
            case VisualState.Normal:
                targetBloom = 0.14f;
                targetVignette = 0.06f;
                targetSaturation = 0f;
                targetTemperature = 8f;
                targetLightIntensity = baseLightIntensity;
                PlayOnly(ambientFx);
                break;

            case VisualState.Transition:
                targetBloom = 0.12f;
                targetVignette = 0.08f;
                targetSaturation = -2f;
                targetTemperature = 10f;
                targetLightIntensity = baseLightIntensity * 0.97f;
                PlayOnly(ambientFx, transitionFx);
                break;

            case VisualState.Focus:
                targetBloom = 0.11f;
                targetVignette = 0.11f;
                targetSaturation = -4f;
                targetTemperature = 12f;
                targetLightIntensity = baseLightIntensity * 0.95f;
                PlayOnly(ambientFx, focusFx);
                break;

            case VisualState.Reward:
                targetBloom = 0.24f;
                targetVignette = 0.05f;
                targetPostExposure = 0.05f;
                targetSaturation = 2f;
                targetTemperature = 14f;
                targetLightIntensity = baseLightIntensity * 1.06f;
                PlayOnly(ambientFx, rewardFx);
                break;
        }

        float t = 0f;

        var startBloom = bloom.intensity.value;
        var startVignette = vignette.intensity.value;
        var startPostExposure = colorAdjustments.postExposure.value;
        var startSaturation = colorAdjustments.saturation.value;
        var startTemperature = whiteBalance.temperature.value;
        var startLight = mainDirectionalLight.intensity;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);

            bloom.intensity.value = Mathf.Lerp(startBloom, targetBloom, k);
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, k);
            colorAdjustments.postExposure.value = Mathf.Lerp(startPostExposure, targetPostExposure, k);
            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, k);
            whiteBalance.temperature.value = Mathf.Lerp(startTemperature, targetTemperature, k);
            mainDirectionalLight.intensity = Mathf.Lerp(startLight, targetLightIntensity, k);

            yield return null;
        }

        if (state == VisualState.Reward)
        {
            yield return new WaitForSeconds(0.2f);
            SetState(VisualState.Normal);
        }
    }

    private void PlayOnly(params ParticleSystem[] active)
    {
        ParticleSystem[] all = { ambientFx, transitionFx, focusFx, rewardFx };
        foreach (var fx in all)
        {
            if (fx == null) continue;
            bool shouldBeActive = System.Array.IndexOf(active, fx) >= 0;
            if (shouldBeActive && !fx.isPlaying) fx.Play();
            if (!shouldBeActive && fx.isPlaying) fx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
```

URP 的 Volume 与后处理确实就是围绕 Volume Profile 和 Override 工作，因此这种“状态参数表 + 平滑插值”的控制方式会比频繁启停大量 Renderer Feature 更稳，也更适合专注类应用。

## 性能预算、测试与最终落地

### Android 目标性能预算

CatLife 应把运行目标分成两层：

**体验目标**：默认设备尽量稳定 60fps；低端设备允许 30fps 质量档。
**兜底红线**：绝不能长期落入 Android vitals 的 slow session 风险区。Google 明确说明，slow session 以 20fps / 30fps 两条线监控，超过 25% 慢帧就会被判为慢会话；而仅仅“没触发 slow session”并不等于体验优秀。

下面是我建议作为 CatLife 内部验收的**项目阈值**：

| 指标 | 通过 | 警告 | 失败 |
|---|---:|---:|---:|
| 目标帧率 | 中高端 60，低端 30 | 低于目标 10% 内 | 持续低于目标 15%+ |
| 可见 triangles | 默认镜头 ≤ 350k | 350k–500k | > 500k |
| Batches | ≤ 140 | 141–220 | > 220 |
| SetPass Calls | ≤ 45 | 46–70 | > 70 |
| 透明覆盖 | 大面积不超过 2x–3x | 局部 4x | 全屏持续高 overdraw |
| 阴影投射体 | ≤ 25 | 26–45 | > 45 |
| 实时阴影光源 | 仅主方向光 1 盏 | 主光 + 个别实验光 | 多盏影灯并存 |
| 粒子活跃数 | 默认 ≤ 130 峰值 | 131–180 | > 180 |
| 粒子材质数 | ≤ 4 | 5–6 | > 6 |
| 后处理 | 4 个轻效果以内 | 有额外特效 | 出现重景深/重模糊/重失真 |
| 场景材质 | 新增效果材质 ≤ 6 | 7–10 | > 10 |
| APK 安装包体 | ≤ 150MB 更稳妥 | 150–250MB | > 250MB |

这些数值不是 Unity 官方硬上限，而是针对你们当前场景复杂度、表现诉求和 Android 实机交付风险做的项目预算。Unity 官方只说明如何优化 draw calls、材质变体、包体、反射与后处理；真正的阈值仍要回到你们设备池测试。

### 测试方法

#### 编辑器内快速检查

先用 **Game View 的 Stats** 看渲染统计，再用 **Frame Debugger** 看有没有多余 pass、重复透明和意外阴影图。Unity 官方说明，Game View 的 Stats 会显示渲染统计；Frame Debugger 可以冻结一帧并逐个查看 draw events。

#### 真机性能采集

必须做 **Development Build + Autoconnect Profiler** 真机采样。Unity 官方很明确：要在 Wi‑Fi 与 USB 连接条件下把 Android 设备连到 Editor，打开 Development Build 和 Autoconnect Profiler，再 Build & Run；只有真机才能看到移动端真实的 CPU/GPU 行为。

#### GPU 深挖

如果需要确认 overdraw、带宽、纹理复制和阴影 pass，使用：

- **RenderDoc**：Unity 编辑器支持集成抓帧，也支持在 standalone player 上正常使用。
- **Android GPU Inspector**：Google 官方工具，可看 GPU、CPU、内存、电量与 GPU counters，支持 Adreno、Mali、PowerVR。

#### 包体与内存

- 用 Unity 的 **Memory Profiler** 查纹理、lightmap、粒子材质和反射探针占用。
- 包体则查看 Unity 的 build size 优化手段，必要时按架构拆分 APK 或直接上 AAB。Unity 官方提供了 Android 发行体积优化文档。

### 直接交给 Unity 开发的任务拆解

下面是按优先级整理的落地清单。这里我保留了 P0/P1/P2/P3 标识，但不把它们放进标题里，方便你们直接抄到任务面板。

#### 必做项

| 优先级 | 文件/资源名 | 实现要点 | 验收标准 |
|---|---|---|---|
| P0 | `URP_Q_Default.asset` | Forward、Render Scale 0.9、HDR On、Opaque/Depth Off、Main Light Shadows On、Additional Shadows Off | 真机默认档稳定运行 |
| P0 | `Renderer_Fwd_Default.asset` | 不加 Decal、不加 SSAO、不加特殊 Renderer Feature | Frame Debugger 无多余 depth/normal prepass |
| P0 | `MAT_Sky_Procedural_CatLife.mat` | Procedural Skybox，奶油蓝天梯度 | 场景无“灰棚感” |
| P0 | `LGT_Sun_Main` | 单 Directional 主光 | 主要体块关系清晰，无遮挡噪 |
| P0 | `VOL_Global_Base.asset` | Bloom/Color/WB/Vignette 基线 | 无重后期痕迹 |
| P0 | `PFX_Ambient_AirDust.prefab` | 常驻低密度空气感 | 同屏无明显 overdraw 区 |

#### 应尽快补齐

| 优先级 | 文件/资源名 | 实现要点 | 验收标准 |
|---|---|---|---|
| P1 | `VOL_State_Normal` 等四套状态配置 | 通过脚本驱动渐变，不切场景 | 状态变化可感知但不刺眼 |
| P1 | `PFX_Transition_LeafDrift` | 过渡态轻提示 | 不打断 UI 使用 |
| P1 | `PFX_Focus_SoftStar` | 专注态极轻背景呼吸 | 专注态更安静而非更热闹 |
| P1 | `PFX_Reward_StarBurst` | 0.8–1.2s 回落型奖励 burst | 奖励明确但不烟花化 |
| P1 | `PROBE_Light_Main` | Light Probe Group 覆盖猫主要活动区 | 动态猫融入场景光照 |
| P1 | `SHD_Blob_Cat` / `MAT_BlobShadow_Cat` | 猫使用假阴影兜底 | 主光影压力可控 |

#### 优化提升项

| 优先级 | 文件/资源名 | 实现要点 | 验收标准 |
|---|---|---|---|
| P2 | `URP_Q_Low.asset` | 0.8 scale、HDR Off、FXAA、1 cascade | 低端机保底 30fps |
| P2 | `URP_Q_Capture.asset` | 4x MSAA、2048 shadow、4 cascades | 录屏画质明显提升 |
| P2 | `Reflection_Global_Baked` | 64–128 分辨率 baked probe | 反射可见但不昂贵 |
| P2 | `Fog_Controller` | 轻线性雾或指数雾 | 远景层次更好 |
| P2 | `CatLifeVisualStateController.cs` | 全局状态控制器 | 状态切换稳定、可配置 |

#### 比赛展示增强项

| 优先级 | 文件/资源名 | 实现要点 | 验收标准 |
|---|---|---|---|
| P3 | `Recorder_Poster_4K` | Unity Recorder / Image Sequence | 海报帧干净可选 |
| P3 | `Recorder_Trailer_1080p60` | 录屏展示档 + 路径镜头 | 视频输出稳定 |
| P3 | `MAT_Sky_Panoramic_Showcase.mat` | 仅展示档用的手绘天空 | 海报更有氛围，但不进 APK |
| P3 | `LGT_Rim_Showcase_A/B` | 录屏专用补光，不投影 | 宣传表现更精致 |

### 一周内可完成的最小闭环版本

如果按比赛节奏，要在一周内做出一个“既能进 APK，又能出 PPT/视频”的闭环，我建议这样拆：

| 时间 | 目标 |
|---|---|
| 第一天 | 完成 `URP_Q_Default`、`Renderer_Fwd_Default`、`MAT_Sky_Procedural_CatLife`、`LGT_Sun_Main` |
| 第二天 | 完成 `VOL_Global_Base`，先把 Bloom / WB / Vignette 调到位 |
| 第三天 | 完成 `PFX_Ambient_AirDust` 与 `PFX_Reward_StarBurst` 两个最关键粒子 |
| 第四天 | 接入四状态控制脚本，打通 Normal / Focus / Reward 切换 |
| 第五天 | 加 Light Probe 与 blob shadow，关掉不必要阴影与额外灯 |
| 第六天 | 真机 Profiling，盯 batches / SetPass / FPS / overdraw / memory |
| 第七天 | 补录屏档与海报档，导出比赛素材 |

### 只给 PPT、海报、视频用，不进 APK 的高质量渲染项

这一部分必须和 Runtime 明确切开：

- 更高分辨率天空贴图或 Panoramic 手绘天空。
- 4x MSAA + SMAA，必要时更高 Shadow Distance 与 4 Cascades。
- Bloom High Quality Filtering 与更高 Max Iterations，但只在录屏档开启。
- 一到两盏不投影的展示补光。
- 更多奖励粒子、更高粒子峰值。
- Recorder 输出 MP4 或图像序列；Unity Recorder 是 Editor-only 工具，适合录制比赛展示，不该成为运行时依赖。
- Poster 截图可用 `ScreenCapture.CaptureScreenshot(..., superSize)` 或 Recorder 图像序列，Unity 官方说明 `superSize` 可以提升截图分辨率。

最终建议可以再浓缩成一句执行口令：

**APK 用“干净天空 + 单主光 + 轻调色 + 轻粒子 + 轻状态变化”，展示素材再单独开高质量档。**
只要坚持这条边界，CatLife 的画面会同时更治愈、更统一，也更有把握在 Android 上稳住。
## 项目内参考链接

- 初稿方案：`10-art-guide/CatLife_非模型画面效果设计方案.md`
- 色彩与材质参考：`02-asset-specs/CatLife_Color_Material_Reference.md`
- 美术规范：`10-art-guide/CatLife_美术资产与低多边形风格规范.md`
- Blender 双轨状态：`tools/blender-mcp/catlife_dual_track_status.md`
- 预览图：`06-deliverables/catlife_preview_cam45.png`

## 外部技术参考链接

- Unity Skybox：<https://docs.unity3d.com/462/Documentation/Manual/class-Skybox.html>
- Unity RenderSettings.skybox：<https://docs.unity3d.com/6000.4/Documentation/ScriptReference/RenderSettings-skybox.html>
- Unity URP Post Processing：<https://docs.unity3d.com/6000.4/Documentation/Manual/urp/integration-with-post-processing.html>
- Unity Shadow Optimization：<https://docs.unity3d.com/6000.0/Documentation/Manual/shadows-optimization.html>
- Unity Mobile Optimization：<https://docs.unity3d.com/2019.4/Documentation/Manual/MobileOptimisation.html>
- Unity Particle System Modules：<https://docs.unity3d.com/6000.4/Documentation/Manual/ParticleSystemModules.html>