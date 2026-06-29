# CatLife 从最新本地场景从零重建 App 流程

日期：2026-06-29
目标：后续开发从本地最新猫咪小镇和猫咪动画资产出发，完整重建 Unity/Android App。协作者 Unity 内容只作为参考，不作为工程基线。

## 1. 基线原则

| 原则 | 说明 |
|---|---|
| 资产基线 | 以当前本地最新 Blender/导出资产为准，不以旧 Unity 场景为准 |
| Unity 工程 | 新建干净 Unity 工程，逐步导入资产、脚本和设置 |
| 参考包 | `06-deliverables/unity-collaborator-reference-20260629/` 只用于阅读和对照 |
| 验收 | 每一步有截图、日志或 manifest，不用“看起来导入了”替代证据 |
| Android | 以真机或 vivo 云真机安装启动为最终证明 |

## 2. 当前资产基线

| 内容 | 路径 | 用途 |
|---|---|---|
| 最新小镇源场景 | `03-3d-models/catlife-town/current/catlife_v2_view_clean_no_merge.blend` | 新 Unity 主场景的美术基准 |
| 猫咪动画源文件 | `03-3d-models/blender-work/CatLife_cat_animation_coordinate_corrected.blend` | 后续动画修订源 |
| 猫咪动画交付包 | `06-deliverables/cat-animation-final-package-20260629/` | Unity 导入猫动作和 Animator 的资产来源 |
| APK 前视觉预览 | `06-deliverables/demo-preview-20260629/` | PPT/视频预排，不作为 runtime 证据 |
| 协作者 Unity 参考包 | `06-deliverables/unity-collaborator-reference-20260629/` | 参考脚本、场景结构、包依赖、设置 |

## 3. 不从旧 Unity 工程继承的内容

| 内容 | 原因 |
|---|---|
| 旧 `mainscene.unity` 作为主场景 | 其内容并非从最新本地小镇场景重建，可能包含过时引用 |
| 旧 `Library/Builds/UserSettings` | 机器生成或输出目录，不是源资产 |
| 旧 `Assets/Art/Cat/Animations` 大模型资产 | 后续应从当前猫动画交付包重新导入 |
| 旧序列化对象引用 | 新项目中 GUID 和对象层级会变化 |
| 旧 Package 全量依赖 | 可能包含 Unity MCP、测试、示例包等非 App 必需依赖 |

## 4. 从零到 App 的阶段

### Phase 0：建立干净工程

1. 使用 Unity `6000.4.9f1` 新建 3D URP/Mobile 项目。
2. 设置项目名 `CatLife`，包名 `com.catlife.mvp`。
3. 建立目录：

```text
Assets/
  Art/Town/
  Art/Cat/
  Materials/
  Prefabs/
  Scenes/
  Scripts/Core/
  Scripts/Cat/
  Scripts/UI/
  Scripts/LLM/
  Editor/
```

4. 只添加必要包：URP、uGUI/TMP、Input System 如需再加。
5. 记录 `Packages/manifest.json` 和 `ProjectSettings/ProjectVersion.txt`。

验收：空工程可打开，无 Console error。

### Phase 1：导入小镇主场景

1. 从 `catlife_v2_view_clean_no_merge.blend` 导出 Unity runtime 格式，优先 FBX/GLB 二选一。
2. 导入到 `Assets/Art/Town/Source/`。
3. 建立 `CatLifeTownRoot` prefab。
4. 处理材质：低多边形色块优先，减少透明/重叠材质。
5. 主场景 `Assets/Scenes/MainScene.unity` 只放：
   - `CatLifeTownRoot`
   - 主相机
   - 主光源
   - 后续猫 spawn point
   - Runtime bootstrap

验收：

- Scene View/Game View 可见完整小镇；
- 无黑色毛刺或明确记录可接受残留；
- Unity Stats 记录 triangles、batches；
- 截图存入 `06-deliverables/final-submission/evidence/03-screenshots/`。

### Phase 2：导入猫和动画

1. 从 `06-deliverables/cat-animation-final-package-20260629/` 导入最终 FBX。
2. 设置 scale、rotation、rig、animation clips。
3. 创建 `CatAnimator.controller`。
4. 初始只绑定 4 个状态：
   - Normal：IdleBreath
   - Transition：CuriousSniff
   - Focus：HeadTiltListen 或 IdleBreath
   - Reward：TailWagHappy
5. 建立 `CatCompanion.prefab`，放入小镇主场景。

验收：

- 猫与小镇比例正确；
- 头尾方向符合当前坐标标准；
- 4 个状态动作可手动切换；
- 截图和短录屏留证。

### Phase 3：重建核心状态机

参考：

- `unity-collaborator-reference-20260629/unity-reference/Assets/脚本/Core/StateMachine.cs`
- `unity-collaborator-reference-20260629/unity-reference/Assets/脚本/Cat/CatController.cs`

重写而不是直接复制：

```text
Normal -> Transition -> Focus -> Reward -> Normal
```

最小合同：

| 模块 | 责任 |
|---|---|
| `FocusStateMachine` | 管理状态、触发事件、状态切换 |
| `CatAnimationDriver` | 接收状态并播放 Animator state |
| `FocusSessionController` | 专注开始、结束、计时、奖励 |
| `BehaviorSignalProvider` | 非敏感行为信号输入，MVP 可用手动/模拟 |

验收：Editor Play Mode 下 3 分钟无 error，状态链完整跑通。

### Phase 4：重建 UI 流程

场景建议：

```text
StartScene -> MainScene -> FocusScene 或 MainScene 内模式切换
```

MVP 更稳的做法：单场景 MainScene 内切 UI 层，避免多场景引用断裂。

UI 最小集合：

- 开屏/进入按钮；
- 主场景状态提示；
- 专注开始按钮；
- 专注计时和轻锁定提示；
- 退出/上滑确认；
- 结束奖励反馈。

参考但不继承：

- `FocusSceneController.cs`
- `FocusUIManager.cs`
- `MainSceneManager.cs`
- `SplashController.cs`

验收：UI 不遮挡猫和小镇，手机竖屏/横屏目标方向明确。

### Phase 5：接入 LLM 最小闭环

参考：

- `ILLMClient.cs`
- `MockLLMClient.cs`
- `SmartFocusAnalyzer.cs`
- `BlueLLMClient.cs`

MVP 建议：

1. 先保留 `MockLLMClient`，保证无网可演示。
2. 再接一个真实云端 API 或端侧 SDK 调用。
3. 请求只发送聚合行为特征，不发送个人内容。
4. 超时 2-3 秒自动降级。
5. 代码包中重点标注真实调用位置。

验收：

- 有一次真实模型调用日志或 SDK 调用证据；
- 无 Key 入库；
- 无网络时 App 不崩溃。

### Phase 6：Android 构建

参考：

- `unity-collaborator-reference-20260629/unity-reference/Assets/Editor/CatLifeAndroidBuild.cs`
- `07-tech-specs/CatLife_Android打包与真机QA方案.md`

构建要求：

| 项 | 建议 |
|---|---|
| Scripting Backend | IL2CPP 优先，首轮可用 Mono 验证 |
| Architecture | ARM64 |
| Texture Compression | ASTC 优先 |
| Package Name | `com.catlife.mvp` |
| Output | `06-deliverables/final-submission/CatLife_MVP_Android_v0.1.0.apk` |

验收：APK 文件存在，SHA256 已记录。

### Phase 7：真机证据和视频

1. 运行：

```powershell
powershell -ExecutionPolicy Bypass -File tools/final-submission/init-final-evidence.ps1
```

2. 安装 APK、启动、录屏、保存 logcat。
3. 用真机画面替换 `demo-preview-20260629/` 里的预览图。
4. 输出最终 MP4、PPT、海报。
5. 运行：

```powershell
powershell -ExecutionPolicy Bypass -File tools/final-submission/check-final-submission.ps1
```

验收：最终检查通过，并人工打开 5 项提交物。

## 5. 协作者参考内容如何使用

| 参考内容 | 使用方式 |
|---|---|
| Core scripts | 抽取状态枚举、事件流和保存结构思想 |
| Cat scripts | 抽取 Animator state 映射方式 |
| UI scripts | 抽取交互流程，不继承具体 UI 层级 |
| Editor scripts | 抽取自动搭建/批量构建思路 |
| Scenes | 观察对象命名和旧结构，不能作为新主场景 |
| Packages/ProjectSettings | 对照依赖和设置，不全量复制 |

## 6. 新开发完成定义

只有同时满足以下条件，才算从零重建完成：

- 新 Unity 工程从最新小镇模型和猫动画资产建立；
- 主场景小镇和猫可见；
- 4 状态动画链可运行；
- 至少一个 LLM 调用或 SDK 调用可证明；
- Android APK 可安装启动；
- 有真机录屏、截图、logcat、构建日志；
- PPT/视频/海报使用真实运行画面；
- 最终提交检查脚本通过。
