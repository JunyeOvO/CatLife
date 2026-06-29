# Unity 协作者参考包文件摘要 2026-06-29

本文件补充 `README.md` 和 `manifest.csv` 的人类可读摘要，用于快速判断每类文件是否值得在后续从零重建 App 时参考。

## 包级结论

| 项 | 内容 |
|---|---|
| 来源 | `work/unity-android-build-batch-20260629/` |
| 当前位置 | `06-deliverables/unity-collaborator-reference-20260629/` |
| 定位 | 参考包，不是新 Unity 工程基线 |
| 已排除 | `Library/`、`Temp/`、`Logs/`、`UserSettings/`、`Builds/`、IDE 生成文件、大模型/猫模型大二进制 |
| 详细清单 | `manifest.csv` |
| 安全检查 | `SECRET_SCAN_REVIEW.md` |

## 顶层文件

| 文件 | 摘要 |
|---|---|
| `README.md` | 说明参考包来源、包含内容、排除内容、复用顺序和不能盲用的边界。 |
| `SECRET_SCAN_REVIEW.md` | 记录导出包的凭据扫描复核，当前标记为 false positive 的 token/onToken/local token 等命名。 |
| `manifest.csv` | 每个导出文件的相对路径、大小、修改时间和 SHA256，用于追溯和查重。 |
| `FILE_SUMMARY.md` | 本文件，补充目录级和关键脚本级摘要。 |

## Runtime 脚本

| 文件 | 类型 | 摘要 | 复用建议 |
|---|---|---|---|
| `unity-reference/Assets/脚本/CameraTransition.cs` | `CameraExactTransition` | 摄像机位置/旋转过渡脚本，用于场景或视角切换。 | 可参考过渡节奏，需按新场景镜头重写目标点。 |
| `unity-reference/Assets/脚本/Cat/CatAnimationMvpDemo.cs` | `CatAnimationMvpDemo` | MVP 猫动画演示控制器，用键盘或状态切换触发动画。 | 可用来验证 Animator 状态名映射。 |
| `unity-reference/Assets/脚本/Cat/CatController.cs` | `CatController` | 猫角色控制核心，负责状态到动画、移动/互动逻辑的绑定。 | 优先阅读，但新工程应重建 Animator 参数和引用。 |
| `unity-reference/Assets/脚本/Core/BehaviorTracker.cs` | `BehaviorData`, `BehaviorTracker` | 行为数据记录与统计，支撑专注/分心判断。 | 可抽取数据结构和记录口径。 |
| `unity-reference/Assets/脚本/Core/DataManager.cs` | `UserData`, `DataManager` | 用户数据保存/读取和本地状态持久化。 | 可参考字段，但需要补隐私与异常处理。 |
| `unity-reference/Assets/脚本/Core/FocusStarter.cs` | `FocusStarter`, `FocusMode` | 专注模式启动入口和模式枚举。 | 可参考流程入口，需和新版 UI 状态机对齐。 |
| `unity-reference/Assets/脚本/Core/GameManager.cs` | `GameManager` | 游戏/应用全局状态协调器。 | 可参考生命周期，但新工程需避免单例过度耦合。 |
| `unity-reference/Assets/脚本/Core/StateMachine.cs` | `CatState`, `StateMachine` | 猫咪状态机，包含普通、困倦、陪伴、奖励等状态意图。 | 是后续重建核心逻辑的主要参考。 |
| `unity-reference/Assets/脚本/LLM/BlueLLMClient.cs` | `BlueLLMClient`, `TokenCallbackProxy`, `UnityMainThread`, `ThreadingHelper` | 蓝心/大模型客户端尝试，包含异步回调和主线程调度思路。 | 只参考接口形态；实际 API、密钥、超时和降级需重写。 |
| `unity-reference/Assets/脚本/LLM/ILLMClient.cs` | `ILLMClient`, `LLMCallback`, `FocusAnalysisResult`, `LLMResponse` | 大模型客户端接口和分析结果数据结构。 | 值得保留为新版 LLM adapter 的接口草案。 |
| `unity-reference/Assets/脚本/LLM/MockLLMClient.cs` | `MockLLMClient` | 离线 mock 客户端，用于没有真实 API 时演示状态变化。 | 可作为无网/隐私降级实现参考。 |
| `unity-reference/Assets/脚本/LLM/SmartFocusAnalyzer.cs` | `SmartFocusAnalyzer` | 智能专注分析逻辑，组合行为数据和 LLM 判断。 | 可复用算法意图，需补测试和阈值说明。 |
| `unity-reference/Assets/脚本/LLM/UnityMainThreadDispatcher.cs` | `UnityMainThreadDispatcher` | 把异步回调切回 Unity 主线程的工具。 | 可参考，但优先使用项目内统一调度实现。 |
| `unity-reference/Assets/脚本/MainSceneManager.cs` | `MainSceneController` | 主场景 UI、猫状态和场景入口协调。 | 可参考主场景信息架构。 |
| `unity-reference/Assets/脚本/SplashController.cs` | `SceneFadeTransition` | 开屏或场景淡入淡出控制。 | 可参考启动页体验。 |
| `unity-reference/Assets/脚本/UI/FocusSceneController.cs` | `FocusSceneController` | 专注场景控制器，管理专注流程和界面响应。 | 可参考专注场景的事件划分。 |
| `unity-reference/Assets/脚本/UI/FocusUIManager.cs` | `FocusUIManager` | 专注界面 UI 管理。 | 可参考 UI 状态切换。 |
| `unity-reference/Assets/脚本/UIManager.cs` | `UIPage`, `UIManager` | 通用页面枚举和 UI 页面管理。 | 可参考页面层级，但需按新版视觉重建。 |
| `unity-reference/Assets/脚本/UnlockBarAnimation.cs` | `UnlockBarAnimation` | 解锁条/滑条动画小脚本。 | 可参考单个 UI 动效。 |

## Editor 脚本

| 文件 | 类型 | 摘要 | 复用建议 |
|---|---|---|---|
| `unity-reference/Assets/Editor/AnimationClipExtractor.cs` | `AnimationClipExtractor` | 从导入模型中提取动画 Clip 的编辑器工具。 | 对猫动画导入流程有参考价值。 |
| `unity-reference/Assets/Editor/CatLifeAndroidBuild.cs` | `CatLifeAndroidBuild` | Android batch build 入口。 | 可参考命令行构建参数，需按本机 Unity 6000.4.9f1 和正式场景重配。 |
| `unity-reference/Assets/Editor/CatModelConfig.cs` | `CatModelConfig` | 猫模型导入或配置辅助。 | 可参考导入设置，不直接覆盖新版资源。 |
| `unity-reference/Assets/Editor/CatSceneSetup.cs` | `CatSceneSetup` | 猫相关场景搭建自动化。 | 可参考自动摆放/引用绑定思路。 |
| `unity-reference/Assets/Editor/CompilationChecker.cs` | `CompilationChecker` | Unity 编译检查工具。 | 可整合进后续质量门禁。 |
| `unity-reference/Assets/Editor/FocusSceneSetup.cs` | `FocusSceneSetup` | 专注场景自动搭建工具。 | 可参考新版 FocusScene 生成流程。 |
| `unity-reference/Assets/Editor/MainSceneSetup.cs` | `MainSceneSetup` | 主场景自动搭建工具。 | 可参考 MainScene 结构，但目标场景需基于最新小镇模型。 |
| `unity-reference/Assets/Editor/PrefabRebuildTool.cs` | `PrefabRebuildTool` | Prefab 重建/修复工具。 | 可参考批处理资产修复。 |

## 场景、资源和配置

| 目录或文件 | 摘要 | 复用建议 |
|---|---|---|
| `unity-reference/Assets/Scenes/startscene.unity` | 启动场景。 | 可参考启动流，不作为最终场景。 |
| `unity-reference/Assets/Scenes/mainscene.unity` | 旧主场景。 | 只参考层级和 UI/脚本挂载方式；新主场景应从最新小镇模型重建。 |
| `unity-reference/Assets/Scenes/FocusScene.unity` | 专注场景。 | 可参考专注 UI/交互结构。 |
| `unity-reference/Assets/Scenes/SampleScene.unity` | Unity 默认或试验场景。 | 低优先级参考。 |
| `unity-reference/Assets/Scenes/mainscene/` | 主场景光照和反射探针 sidecar 文件。 | 只用于了解旧场景烘焙状态。 |
| `unity-reference/Assets/Images/` | UI 图片、开屏页、按钮/设置/猫等 2D 资源。 | 可作为旧 UI 参考，最终视觉应替换为新版截图/设计。 |
| `unity-reference/Assets/Screenshots/` | 旧 Unity 验证截图。 | 可用于对照旧效果，不进入最终提交物。 |
| `unity-reference/Assets/Settings/` | URP、Renderer、Volume Profile 等渲染设置。 | 可参考移动端渲染配置，需结合新小镇材质重新调。 |
| `unity-reference/Assets/Resources/` | Performance Test 运行配置。 | 可参考性能测试入口。 |
| `unity-reference/Assets/场景搭建指南/FocusScene搭建指南.md` | 专注场景搭建说明。 | 可作为从零重建时的辅助阅读。 |
| `unity-reference/Packages/manifest.json` | 旧 Unity 依赖清单。 | 新工程依赖以实际最小可运行为准。 |
| `unity-reference/Packages/packages-lock.json` | 旧依赖锁文件。 | 只用于追溯版本。 |
| `unity-reference/ProjectSettings/` | 旧 Unity ProjectSettings 快照。 | 可参考 Android、URP、质量、输入和构建场景设置。 |
| `unity-reference/MCP-log-excerpts/` | Unity MCP 日志尾部摘录。 | 用于排查当时的 MCP/Unity 状态，不作为产品证据。 |

## 不应直接复用的部分

| 项 | 原因 |
|---|---|
| 整个 `unity-reference/` 覆盖到新工程 | 会带入旧场景、旧引用、旧环境路径和未知依赖。 |
| 旧场景作为最终场景 | 用户已明确后续以最新本地小镇模型从零重建完整 App。 |
| 旧 ProjectSettings 全量覆盖 | 可能覆盖 Unity 版本、URP、包名、构建场景和平台设置。 |
| 旧 LLM 客户端直接接真实服务 | API、隐私、凭据和失败降级都需要重新审查。 |

## 推荐阅读顺序

1. `06-deliverables/unity-handoff-20260629/GITHUB_UPLOAD_SUMMARY.md`
2. `06-deliverables/unity-collaborator-reference-20260629/README.md`
3. `06-deliverables/unity-collaborator-reference-20260629/FILE_SUMMARY.md`
4. `06-deliverables/unity-collaborator-reference-20260629/manifest.csv`
5. `unity-reference/Assets/脚本/Core/StateMachine.cs`
6. `unity-reference/Assets/脚本/Cat/CatController.cs`
7. `unity-reference/Assets/脚本/LLM/ILLMClient.cs`
8. `unity-reference/Assets/Editor/MainSceneSetup.cs`
9. `unity-reference/ProjectSettings/EditorBuildSettings.asset`
