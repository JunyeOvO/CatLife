# CatLife 移动端 3D 性能预算与优化方案

日期：2026-06-29
目标：为猫咪小镇、动画猫、粒子/渲染效果和 Android 包设定可执行预算，避免“能在 Blender 看见”但“手机跑不动”。

## 1. 目标设备与帧率

| 档位 | 目标 | 说明 |
|---|---|---|
| MVP 保底 | 30 FPS 稳定运行 3 分钟 | 复赛演示和真机验证的最低门槛 |
| 推荐体验 | 45-60 FPS | 中高端 Android 设备，发热可控 |
| 录屏模式 | 30 FPS 锁帧也可接受 | 优先稳定画面和音画同步 |

项目是专注陪伴软件，不是高强度动作游戏。画面应优先稳定、清晰、低发热，而不是追求最高模型精度。

## 2. 预算建议

| 指标 | MVP 预算 | 说明 |
|---|---:|---|
| 屏幕内可见三角形 | 100k-250k 优先；超过 500k 需明确性能证据 | 162 万面不能直接进入移动端主场景 |
| Draw Calls / Batches | 100-150 以内优先 | 小镇重复树木、栅栏、花草必须合批或实例化 |
| 材质数量 | 主场景 20-40 个以内 | 低多边形风格适合材质复用和调色板 |
| 主贴图尺寸 | 大物体 1024，普通道具 512，小道具 256 | 先压缩，再按镜头需要提高 |
| 运行时贴图内存 | MVP 128 MB 内优先 | 大量未压缩贴图会先打爆带宽和内存 |
| 实时光源 | 1 个主方向光 + 少量无阴影补光 | 移动端少用实时阴影 |
| 粒子系统 | 同屏 1-3 个轻量效果 | 避免透明粒子 overdraw |
| 骨骼动画角色 | 主猫 1 只，其他猫静态或低频动画 | 主猫才需要完整 Animator |

## 3. 162 万面资产判断

当前“12+150=162w 面”的整体量级不适合直接作为移动端运行基线。行业项目中单角色高面数成立的前提通常包括：

- 明确目标机型和性能档位；
- 角色是屏幕核心，场景和背景有严格预算；
- 有 LOD、烘焙、遮挡剔除、贴图压缩、骨骼/材质预算；
- 持续用真机 Profiler 验证。

CatLife MVP 的风险正好相反：猫咪、小镇、UI、状态机、录屏和 Android 包都要同时稳定。因此建议先做“可运行低预算版”，再逐步提高视觉质量。

## 4. 优化路线

### 4.1 模型与导入

- Unity 模型导入默认关闭 `Read/Write`，除非脚本必须读 Mesh 数据。Unity 官方说明关闭后会移除 CPU 侧 Mesh 数据，节省运行时内存。
- 开启 `Optimize Mesh`，保留默认顶点/索引优化。
- 静态小镇物体优先拆为模块：地面、中心广场、建筑、树木、栅栏、花草、小道具。
- 不再使用破坏外观的 Blender mesh merge 文件；合批应在 Unity 静态批处理或材质复用层完成。
- 给远景树、栅栏和建筑配置 LOD。小镇全景镜头下，远处小物件可以降到极低模型或直接隐藏。

### 4.2 材质与贴图

- Android 优先使用 ASTC；如需要兼容更老设备，再准备 ETC2 方案。
- 所有 3D 贴图开启 mipmap，减少远距离闪烁和带宽压力。
- 低多边形风格优先使用少量纯色/渐变材质，少用复杂 PBR。
- 透明材质只保留必要对象。黑线毛刺、半透明 hashed/dithered 材质和透明阴影需要逐项排查。

### 4.3 渲染设置

- 目标使用 URP 移动端配置或等价轻量渲染设置。
- 使用单方向光，静态场景光照尽量烘焙。
- 阴影分辨率低档起步，远距离阴影关闭。
- 如真机 GPU 压力大，开启分辨率缩放或 Dynamic Resolution。
- Android Player Settings 中优先开启 Multithreaded Rendering、Static Batching、GPU Skinning，按真机表现调整。

### 4.4 场景组织

- 小镇导入后先按区域分组：入口、中心广场、番茄钟楼、猫窝、鱼干超市、许愿树、树木栅栏圈。
- 摄像机主视角只看到需要的区域，远处和背面用剔除距离控制。
- 碰撞体只给交互区域、地面和必要建筑添加简化 Collider，不给所有装饰物生成 Mesh Collider。

## 5. 性能验证清单

| 检查 | 工具 | 通过标准 |
|---|---|---|
| FPS | Unity Stats / Profiler / 真机录屏观察 | MVP 30 FPS 可持续 |
| Batches | Unity Stats | 主场景 100-150 优先 |
| Triangles | Unity Stats | 首版低于 250k 优先 |
| SetPass Calls | Unity Stats | 越低越好，异常高说明材质太散 |
| 内存 | Profiler Memory | 无持续上涨；贴图内存可解释 |
| GC Alloc | Profiler CPU | 常驻流程接近 0 或低频 |
| 发热 | 真机连续运行 | 3 分钟演示不明显降频 |

## 6. 粒子/渲染效果落地

| 效果 | 方式 | 预算 |
|---|---|---|
| 专注开始柔和提示 | UI 淡入 + 轻粒子 | 1 个粒子系统，低发射率 |
| 奖励反馈 | 猫尾动画 + 少量星星/花瓣 | 生命周期短，贴图小 |
| 小镇氛围 | 静态花草、烘焙阴影、轻风摆动可选 | 不做大量透明粒子 |
| 状态变化 | 颜色/光照轻变化 + 猫动作切换 | 避免全屏后处理堆叠 |

## 7. 官方依据

- Unity 图形优化：先定位 CPU/GPU 瓶颈，再处理 Draw Call、Overdraw、贴图带宽、顶点处理和 LOD。https://docs.unity3d.com/Manual/OptimizingGraphicsPerformance.html
- Unity 模型导入：`Read/Write` 关闭可节省运行时内存，`Optimize Mesh`、Mesh Compression、LOD、Weld Vertices 属于导入侧优化。https://docs.unity3d.com/Manual/FBXImporter-Model.html
- Unity Android：Resolution Scaling、Static Batching、GPU Skinning、Texture Compression、IL2CPP/ARM64 属于 Android Player Settings 关键项。https://docs.unity3d.com/Manual/class-PlayerSettingsAndroid.html
- Android 游戏优化：真机性能应通过 Unity on Android、Frame Pacing、Performance Tuner 或等价工具验证。https://developer.android.com/games/optimize
