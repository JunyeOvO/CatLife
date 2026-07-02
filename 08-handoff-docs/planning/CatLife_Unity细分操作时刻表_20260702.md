# CatLife Unity 细分操作时刻表

日期：2026-07-02
用途：给当前 Unity 主工程建立“下一步怎么做”的开发时刻表。本文不是开发行为规范，而是按操作顺序拆分相机、截图验证、10 个猫动画、无识别播放、连续行走、可视范围约束、模块接入和预览图效果达成路径。

## 0. 当前基线

| 项 | 当前状态 |
|---|---|
| Unity 主工程 | `work/CatLife_Unity_Main/` |
| 主场景 | `Assets/Scenes/MainScene.unity` |
| Main Camera | position `(0, 4.9, -43.2)`；rotation 约 `(1.5, 0, 0)`；FOV `34` |
| 小镇 | `CatLifeTownRoot`；167 renderers；bounds center `(0.323, 3.273, 0.588)`；bounds size `(42.757, 10.234, 33.586)` |
| 猫 | `CatCompanionModel`；position `(0, 0.03, -8.5)`；scale `0.055`；已挂 `Animator` + `CatTownWalker` |
| 当前 Animator | `Assets/Art/Cat/Animator/CatLife_TownWalker.controller`；当前只有 Idle + Walk + `IsWalking` |
| 已有动作 clip | `Assets/Art/Cat/Animations/Clips/` 下已有 10 个状态动作 `.anim` + 1 个 walk `.anim` |
| 当前行走 | 随机点移动，基础转向；还没有基于相机视野约束，也没有连续路径/平滑拐弯 |
| 当前验证能力 | 可通过 Unity MCP `manage_camera` 抓 Game View / Scene View 截图，可通过 `execute_code` 读对象位置和 Animator 状态 |

结论：下一步不是继续导入大 FBX，而是先建立“相机画面可验证基准”，再把已有 10 个动作接入无识别播放，随后升级行走和可视范围约束。

## 1. 里程碑顺序

| 里程碑 | 目标 | 完成后能看到什么 |
|---|---|---|
| M1 镜头与截图验证闭环 | Game View、Scene View、Main Camera 对齐，可用多模态截图判断画面 | Play Mode 里能稳定截到猫和小镇，后续每步可视觉验收 |
| M2 无识别猫动画播放 | 10 个动作接入 Animator，未接行为识别时也能自然播放 | 猫不只 idle/walk，会在等待或主界面自然做抬头、摇尾、伸懒腰等动作 |
| M3 连续行走与平滑拐弯 | 行走路径更连续，目标点串联，转弯不突兀 | 猫在小镇中持续游走，拐弯有提前转向，不像瞬间换方向 |
| M4 镜头内活动约束 | 猫的目标点由相机可见地面范围生成 | 猫不会走出 Game View 画面主要可见区 |
| M5 首页预览构图 | 相机、猫、背景、UI Overlay 形成目标预览画面 | 能稳定产出接近宣传预览图的竖屏截图 |
| M6 行为识别接入 | mock 行为事件和真实 Android 事件分阶段进入 Unity | 猫动画和 UI 状态能被 Normal/Transition/Focus/Reward 驱动 |
| M7 Android/最终证据 | APK 构建、真机验证、录屏和截图证据落地 | 可进入最终提交包制作 |

## 2. 开发时刻总表

Day 0 以 2026-07-02 当前 Unity 状态为起点。时间是操作批次，不是日历承诺；如果 Unity 编译、资产导入或视觉 QA 失败，当前批次顺延，不能跳阶段。

| 时刻 | 操作 ID | 具体操作 | 主要文件/对象 | 验证方式 | 进入下一步条件 |
|---|---|---|---|---|---|
| Day 0 / 0.1 | U-001 | 建立 Main Camera / Game View / Scene View 对齐操作。新增 Editor 菜单：`CatLife/Camera/Align Main Camera To Scene View`、`CatLife/Camera/Frame Town Home Shot`、`CatLife/Camera/Capture Game And Scene Views`。 | `Assets/Editor/CatLifeCameraShotSetup.cs`；`Main Camera` | 用 `manage_camera` 分别抓 `game_view` 和 `scene_view`，确认两张图看到同一只猫和同一块小镇区域。 | 两张截图的猫、小镇主建筑和地面区域一致；Console 0 error。 |
| Day 0 / 0.2 | U-002 | 固定 Home Shot v1：调整 Main Camera 位置、角度、FOV，使 Play Mode 时猫和小镇中心在 9:16 Game View 中可见。 | `Main Camera`；`MainScene.unity` | Play Mode 截图；对象查询记录 camera position/rotation/FOV。 | 截图中猫完整可见，背景有小镇主体，UI 预留区不遮挡猫。 |
| Day 0 / 0.3 | U-003 | 建立截图验收目录策略：临时截图仍放 `Assets/Screenshots/`，最终证据截图放 `06-deliverables/final-submission/evidence/03-screenshots/`。 | 文档和后续截图文件 | `git status` 确认临时截图不被误提交。 | 截图命名清楚；临时和证据分开。 |
| Day 0 / 0.4 | U-004 | 检查 10 个动作 clip 是否都能被 Unity 读取，并生成 clip 清单。不要重复导入 FBX。 | `Assets/Art/Cat/Animations/Clips/*.anim`；`cat_actions_manifest.json` | Unity `execute_code` 读取 10 个 clip，检查 `loopTime`、长度、binding root。 | 10 个动作 clip 全部存在且无 missing binding。 |
| Day 0 / 0.5 | U-005 | 设计并生成无识别播放 Animator Controller v1：Idle、Walk、10 个 ambient 动作；参数先用 `IsWalking`、`AmbientAction`、`PlayAmbient`。 | `Assets/Art/Cat/Animator/CatLife_CatRuntime.controller` | Unity Animator 状态数检查；Play Mode 手动触发 3 个动作。 | 不接行为识别也能播放 idle/walk/ambient。 |
| Day 1 / 1.1 | U-006 | 新增 `CatAmbientAnimationPlayer`：在无用户行为识别时，按等待时间随机播放 10 个动作；行走时只播 Walk，停下时才播 ambient。 | `Assets/Scripts/Cat/CatAmbientAnimationPlayer.cs`；`CatCompanionModel` | Play Mode 30 秒观察；记录触发动作名；Console 0 error。 | 猫静止时能随机做动作，行走时不和 walk 抢状态。 |
| Day 1 / 1.2 | U-007 | 升级 `CatTownWalker` 为连续路径模式：目标队列、最小目标距离、提前转向、到点不停顿或短暂停顿可配置。 | `Assets/Scripts/Cat/CatTownWalker.cs` | Play Mode 60 秒记录位置序列；观察拐弯和速度。 | 猫不会频繁原地抖动，转弯平滑，路径连续。 |
| Day 1 / 1.3 | U-008 | 建立相机可见地面范围：从 Main Camera 视锥投射到 `groundY=0.03` 平面，生成猫可活动矩形或多边形。 | `CatTownWalker` 或新增 `CatCameraVisibleWalkArea.cs` | Gizmo 画出可活动范围；Play Mode 查询目标点 viewport 坐标。 | 随机目标点的 viewport x/y 在安全区内，例如 `0.08..0.92`、`0.12..0.82`。 |
| Day 1 / 1.4 | U-009 | 新增可视范围自动验证：每 0.5 秒采样猫位置，转换到 camera viewport；越界时记录 warning 或停止测试。 | `Assets/Scripts/Cat/CatCameraVisibilityProbe.cs`；Editor test helper | Play Mode 60 秒无越界；截图确认猫始终可见。 | 60 秒无 viewport 越界；Console 0 error。 |
| Day 1 / 1.5 | U-010 | 多模态截图验收第 1 轮：Game View 截图、Scene View 截图、猫路径 gizmo 截图。 | 临时截图 | 人工看图：猫比例、背景、小镇、活动范围是否合理。 | 视觉基准通过后再接 UI。 |
| Day 2 / 2.1 | U-011 | 首页 UI Overlay 接入 v1：顶部 CatLife/时间，右侧功能按钮，底部 CTA；先做静态 Canvas，不接真实功能。 | `Assets/Scripts/UI/` 或现有 Canvas；`MainScene.unity` | 9:16 Game View 截图，检查文字和按钮不遮挡猫。 | UI 布局接近目标图，猫脸/身体/CTA 不重叠。 |
| Day 2 / 2.2 | U-012 | Home Shot v2：根据 UI 重新微调 camera、cat 起始点、walk area，让猫默认处在视觉中心下方。 | `Main Camera`；`CatCompanionModel`；walk area | Play Mode 截图；多分辨率 9:16/16:9 截图。 | 9:16 截图达到“可做 PPT/视频素材”的首页视觉。 |
| Day 2 / 2.3 | U-013 | 状态机 mock 接入：不接 Android 行为识别，先用 Editor/键盘/按钮模拟 Normal、Transition、Focus、Reward。 | `Assets/Scripts/State/`；`cat_reaction_state_table.json` | Play Mode 切状态，观察猫动作和 UI 文案变化。 | 四状态可手动切换，猫动作可响应。 |
| Day 2 / 2.4 | U-014 | 将 10 个动作映射到状态表：Normal 偏 idle/curious，Transition 偏 alert/look back，Focus 偏 calm idle，Reward 偏 paw wave/tail wag。 | Animator Controller；状态驱动脚本 | 逐状态截图/短录屏。 | 每个状态至少有 1 个明确猫反馈动作。 |
| Day 3 / 3.1 | U-015 | 行为识别模块 Unity 入口接入：读取 mock `behavior_event_schema.json` 数据流，输出 state + score。 | `Assets/Scripts/Behavior/`；`07-tech-specs/behavior_event_schema.json` | 输入样例事件，Console 或 UI 显示 state/score。 | 不依赖 Android 也能本地跑行为识别模拟。 |
| Day 3 / 3.2 | U-016 | Android 行为事件桥接预留：Unity 侧暴露 `ReceiveBehaviorEventJson(string json)`，Android 后续只需传 JSON。 | `Assets/Scripts/Bridge/AndroidBehaviorEventBridge.cs` | Editor 内直接调用 JSON；异常 JSON 不崩溃。 | Unity 侧接口稳定，Android 可接入。 |
| Day 3 / 3.3 | U-017 | LLM 降级文案/猫气泡预留：先接 mock 文案和本地规则输出，不在 Unity 中硬编码密钥。 | `Assets/Scripts/LLM/`；气泡 UI | 点击或状态切换时出现短句。 | 无密钥，断网可演示。 |
| Day 4 / 4.1 | U-018 | Android 构建前 Unity 清理：Build Settings、Scenes In Build、URP、Input、分辨率和性能预算检查。 | `ProjectSettings/`；`Packages/`；主场景 | Unity Console、build dry run、性能统计。 | 无 Editor-only runtime 脚本进入 Android。 |
| Day 4 / 4.2 | U-019 | APK 构建和真机验证接力。 | `06-deliverables/final-submission/` | ADB install、启动、logcat、screenrecord。 | 主流程可在真机或云真机跑通。 |
| Day 4 / 4.3 | U-020 | 最终预览素材冻结：输出首页、专注、奖励、记录页截图和演示视频素材。 | `final-submission/evidence/` | 检查截图来自同一版 APK 或同一版 Unity 运行画面。 | 可进入 PPT/视频/海报最终制作。 |

## 3. 相机与截图验证细分表

这是最先做的操作，因为后续“猫是否可见”“是否接近预览图”都依赖它。

| 操作 | Unity 内动作 | MCP/验证动作 | 产出 |
|---|---|---|---|
| 1. Scene View 选定构图 | 在 Scene View 中框住猫、小镇主建筑、前景道路 | `manage_camera(capture_source="scene_view", include_image=true)` | Scene View 参考截图 |
| 2. Main Camera 对齐 Scene View | Editor 菜单把 Main Camera 设置到当前 Scene View 位置和旋转 | 读取 Main Camera transform | camera 数值 |
| 3. Game View 验证 | 切 Play Mode，用 Main Camera 截图 | `manage_camera(capture_source="game_view", camera="Main Camera", include_image=true)` | Game View 截图 |
| 4. 画面对比 | 比较 Scene View 和 Game View 中猫/建筑相对位置 | 多模态看图 | 是否需要调 FOV/高度/俯仰 |
| 5. 保存 Home Shot | 固化 camera transform；写入操作记录 | `git diff MainScene.unity` | Home Shot v1 |

判定标准：

| 项 | 标准 |
|---|---|
| 猫 | 至少 90% 身体在画面内，头部不被 UI 预留区域遮挡 |
| 小镇 | 至少看到主建筑/猫窝/道路中的两个主视觉元素 |
| 地面 | 猫脚下有可行走区域，不能只悬在边缘 |
| UI 预留 | 顶部、右侧、底部 CTA 留出空间 |
| 截图方式 | Game View 和 Scene View 都可由 MCP 抓到，便于我用多模态检查 |

## 4. 10 个动画接入时刻表

当前 10 个状态动作已经是 Unity `.anim`，不要作为下一步重复导入 FBX。下一步是“确认、建 controller、建无识别播放机制”。

| 动作 clip | 初始用途 | 无识别阶段播放时机 | 行为识别接入后用途 |
|---|---|---|---|
| `CL_CAT_IdleBreath_v06_headsync_loop_108f` | 默认 idle | 常驻基础状态 | Normal / Focus 基础态 |
| `CL_CAT_AlertLook_v01_loop_120f` | 注意/警觉 | 偶发看向周围 | Transition |
| `CL_CAT_PawWave_v01_loop_96f` | 奖励/互动 | 偶发短动作 | Reward |
| `CL_CAT_TailWagHappy_v01_loop_96f` | 开心/奖励 | 偶发短动作 | Reward / Normal |
| `CL_CAT_CuriousSniff_v02_loop_112f` | 好奇 | 停下后随机 | Normal |
| `CL_CAT_HeadTiltListen_v01_loop_96f` | 聆听 | 停下后随机 | Transition / LLM 气泡 |
| `CL_CAT_LookBack_v02_loop_112f` | 回头 | 拐弯或停下后随机 | Transition |
| `CL_CAT_StretchYawn_v03_slow_loop_264f` | 伸懒腰 | 长时间 idle 后低频播放 | Normal / Focus 前 |
| `CL_CAT_EarTwitchAlert_v02_loop_120f` | 耳朵警觉 | 偶发短动作 | Transition |
| `CL_CAT_HeadShakeNo_v01_loop_108f` | 摇头否定 | 低频播放 | 分心提醒 / 轻锁定反馈 |

Animator 先按两层逻辑做：

| 层级 | 内容 | 原因 |
|---|---|---|
| Locomotion | Idle、Walk，由 `IsWalking` 控制 | 保证行走稳定，不被 ambient 打断 |
| Ambient State | 10 个动作，由 `AmbientAction` / trigger 控制 | 没有行为识别时也能自然展示猫的生命感 |

如果 Generic rig 的 layer mask 不稳定，先退回单层状态机：行走时禁播 ambient，停下后播 ambient，播放结束回 idle。

## 5. 连续行走和拐弯能力细分表

| 能力 | 当前状态 | 目标状态 | 具体操作 |
|---|---|---|---|
| 目标点选择 | 单个随机点 | 目标队列或连续重采样 | 生成 2-3 个候选点，按距离和视野筛选 |
| 停顿 | 到点后随机停 | 可配置短停或不停 | `waitSecondsRange` 支持 0 和短暂停顿 |
| 转向 | 朝目标直接 Slerp | 提前转向、限制角速度 | 用 `turnSpeed` + `slowdownAngle` 控制 |
| 路径平滑 | 直线折线 | 曲线感移动 | 近目标时提前选下个目标，避免直角折返 |
| 边界处理 | x/z 矩形 | 相机可见安全区 | 目标点来自 camera viewport ground projection |
| 动画同步 | `IsWalking` bool | 速度阈值驱动 | 速度低于阈值才进入 idle/ambient |
| QA | 手动看 | 采样验证 | 每 0.5 秒记录位置和 viewport 坐标 |

优先级：

| 优先级 | 项 |
|---|---|
| P0 | 目标点只在镜头安全区；连续路径；不出画 |
| P1 | 平滑拐弯；停走节奏自然 |
| P2 | 避开建筑/障碍；沿道路移动 |

## 6. 镜头内活动验证表

猫“只在摄像头能看到的范围内活动”不能靠目测，需要程序验证。

| 验证 | 方法 | 通过标准 |
|---|---|---|
| 目标点可见 | 生成目标点后转 viewport 坐标 | `x` 在 `0.08..0.92`，`y` 在 `0.12..0.82`，`z > 0` |
| 猫当前位置可见 | Play Mode 每 0.5 秒采样 cat position | 连续 60 秒无越界 |
| 拐弯过程可见 | 采样移动中间点 | 不只目标点可见，中间路径也可见 |
| 截图可见 | 10 秒、30 秒、60 秒各截一张 Game View | 猫完整或主体可见 |
| UI 不遮挡 | 加 UI 后重复截图 | 猫头和主体不被顶部/右侧/CTA 覆盖 |

建议新增工具：

| 工具 | 作用 |
|---|---|
| `CatCameraVisibleWalkArea` | 根据 Main Camera 和 groundY 计算可行走安全区域 |
| `CatCameraVisibilityProbe` | Play Mode 中采样猫位置，输出 viewport 坐标和越界 warning |
| `CatLifeCameraShotSetup` | Editor 菜单：对齐、框选、截图 |

## 7. 模块接入顺序

| 接入顺序 | 模块 | 什么时候加入 Unity | 依赖 |
|---:|---|---|---|
| 1 | Camera Shot / 截图验证 | 立即加入，所有后续视觉工作前 | 当前主场景 |
| 2 | 10 动作无识别播放 | Camera Shot v1 后加入 | 已有 10 个 `.anim` |
| 3 | 连续行走 v2 | 动画播放稳定后加入 | Walk clip + CatTownWalker |
| 4 | 可视范围约束 | 连续行走同批或紧随其后加入 | Main Camera 固定 |
| 5 | 首页 UI Overlay | 猫可见范围稳定后加入 | 9:16 构图 |
| 6 | 状态机 mock | UI v1 后加入 | `cat_reaction_state_table.json` |
| 7 | 行为识别 Unity mock | 状态机 mock 后加入 | `behavior_event_schema.json` |
| 8 | Android 事件桥接 | Unity mock 验证后加入 | Android 侧接口 |
| 9 | LLM/气泡文案 | 状态机可驱动 UI 后加入 | 本地 mock + 无密钥策略 |
| 10 | Android build / 真机证据 | Unity Editor 主流程稳定后加入 | Build Settings、性能预算 |

不要提前接入的内容：

| 模块 | 暂缓原因 |
|---|---|
| 真实 Android 行为采集 | Unity 侧状态机和可视演示尚未完全稳定，过早接入会增加排障维度 |
| 真实云端 LLM key | 代码包和隐私降级策略未冻结前，不应把密钥或真实调用散落到 Unity 工程 |
| 障碍物导航/NavMesh | 当前首要问题是镜头可见和演示效果；NavMesh 可在 P2 优化 |

## 8. 预览图效果达成时刻

这里的“预览图效果”指：竖屏首页里猫处于中心视觉焦点，小镇作为低多边形背景，顶部/右侧/底部 UI 完整，Play Mode 截图能作为 PPT/视频素材。

| 级别 | 预计达成点 | 画面效果 | 尚缺 |
|---|---|---|---|
| Preview 0 | 当前状态 | 小镇和猫已在同一场景，猫可行走 | 镜头验证、UI、10 动作 ambient、视野约束 |
| Preview 1 | U-001 到 U-010 完成 | 可稳定截图：猫在小镇内移动，镜头可验证，猫不会出画 | UI Overlay 和状态反馈 |
| Preview 2 | U-011 到 U-014 完成 | 接近首页目标：有 CatLife UI、按钮、CTA，猫有自然动作 | Android 真机画面和真实状态数据 |
| Preview 3 | U-015 到 U-020 完成 | 可作为比赛演示素材：状态切换、猫反馈、录屏证据完整 | 只剩剪辑、PPT/海报排版 |

如果目标是“尽快得到一张接近参考图的 Unity 截图”，最短路径是：

```text
U-001 相机截图闭环
U-002 Home Shot v1
U-008 相机可见活动范围
U-011 首页 UI Overlay
U-012 Home Shot v2
```

如果目标是“可交互演示”，必须再做：

```text
U-005/U-006 10 动作无识别播放
U-007 连续行走
U-013/U-014 状态机 mock 和动作映射
```

## 9. 每批完成后的验收记录模板

```text
批次：
完成操作 ID：
修改文件：
Unity Console：
Play Mode 结果：
Game View 截图：
Scene View 截图：
猫位置/viewport 采样：
是否进入下一批：
剩余风险：
```

## 10. 立即下一步

下一步应执行 U-001 和 U-002：

1. 新增 `CatLifeCameraShotSetup.cs`，提供相机对齐和截图菜单。
2. 将 Main Camera 调成 Home Shot v1。
3. 用 MCP 抓一张 `game_view` 和一张 `scene_view`。
4. Play Mode 验证猫、小镇、地面是否在同一构图中。
5. 记录 camera transform，确认后再进入 10 动作播放和行走增强。
