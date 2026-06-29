# CatLife Unity 项目 · 交接文档
> 撰写人：陈泓森
> 最后更新：2026-06-29
> 仓库地址：https://github.com/JunyeOvO/CatLife

---

## 一、项目概览

| 项目 | 内容 |
|------|------|
| 引擎 | Unity 2021.3 LTS |
| 平台 | Android |
| 场景 | startscene（开屏）、mainscene（主场景）、FocusScene（专注场景） |
| 核心玩法 | 行为识别 → 状态机切换 → 猫咪动画引导专注 |

---

## 二、目录结构

```
Assets/
├── 脚本/                     # 所有 C# 代码（UTF-8 BOM）
│   ├── Cat/
│   │   └── CatController.cs        # 猫咪行为 + 动画调度
│   ├── Core/
│   │   ├── StateMachine.cs          # 四状态机（Normal/Transition/Focus/Reward）
│   │   ├── BehaviorTracker.cs       # 行为数据采集（30s 窗口）
│   │   ├── DataManager.cs           # 本地持久化（PlayerPrefs/JSON）
│   │   ├── FocusStarter.cs          # 专注计时器
│   │   └── GameManager.cs           # 主管理器（单例，启动链）
│   ├── LLM/
│   │   ├── ILLMClient.cs            # LLM 接口定义
│   │   ├── MockLLMClient.cs         # Editor 模拟实现
│   │   ├── BlueLLMClient.cs         # Android 蓝心 3B 端侧 SDK
│   │   ├── SmartFocusAnalyzer.cs    # 行为分析 → 状态切换决策
│   │   └── UnityMainThreadDispatcher.cs
│   ├── UI/
│   │   └── FocusUIManager.cs         # 专注场景 UI 管理
│   ├── MainSceneManager.cs           # 主场景控制器
│   ├── SplashController.cs           # 开屏控制器
│   └── CameraTransition.cs           # 相机过渡动画
├── Art/                        # 美术资产（GitHub 未上传，见下方说明）
│   ├── Cat/                    # 猫模型（Meshy AI 输出 .glb）
│   ├── Animations/             # 动画控制器
│   ├── Materials/              # 材质
│   └── Models/                 # 场景建筑模型

> ⚠️ **Art 文件夹说明**：Art/ 目录未上传至 GitHub，原因：部分 .fbx 模型文件超过 GitHub 单文件 25MB 限制。协作者需要：
> - 从 Unity Asset Store 或源文件（.glb）重新导入猫模型
> - 或联系傅钧烨获取完整模型文件后手动放入 Assets/Art/
> - 核心代码（状态机、行为检测、UI）在缺少 Art 的情况下仍可正常编译运行
├── Scenes/
│   ├── startscene.unity        # 开屏（注意小写）
│   ├── mainscene.unity         # 主场景（小写）
│   └── FocusScene.unity        # 专注场景（注意大小写 F）
├── MSYH*.ttc                    # 字体文件（不要改名）
└── InputSystem_Actions.inputactions

ProjectSettings/                # Unity 项目配置
Packages/                        # NuGet/Unity 包
```

> ⚠️ **场景名大小写敏感！** `mainscene` 全小写、`FocusScene` F 大写。Build Settings 中的 Scene 列表必须与这里完全一致，否则打包后场景加载失败。

---

## 三、核心架构

### 3.1 四状态机

```
CatState.Normal（普通）
    ↓ 用户活跃度持续下降
CatState.Transition（过渡）—— 猫咪走近注视
    ↓ 连续 3 次窗口判断为专注
CatState.Focus（专注）—— 猫咪安静趴下 + 轻锁定 UI
    ↓ 用户上滑退出 / 主动放弃
CatState.Reward（奖励）—— 庆祝动画
    ↓ 自动返回
CatState.Normal
```

**状态转换核心**：`Assets/脚本/Core/StateMachine.cs`
- 事件驱动：`OnStateChanged`、`OnEnterState`、`OnExitState`
- 全局静态事件：`GlobalStateChanged`（其他脚本无需引用 StateMachine 即可订阅）

### 3.2 行为检测链

```
BehaviorTracker（Update 实时采集）
    ↓ 每 30s 窗口分析
    ↓ 活跃度分数 = f(点击×0.3 + 滑动×0.2 + 滚动×0.2 + 空闲×0.3)
    ↓ 连续 3 次 < 0.3 阈值 → 触发 OnFocusStateChanged(true)
    ↓
SmartFocusAnalyzer（订阅 OnDataUpdated）
    ├─ 规则分析（默认，Editor/Mock）
    └─ LLM 分析（生产，蓝心 3B）
    ↓
StateMachine.SwitchState(CatState.Focus)
```

### 3.3 启动链（GameManager 单例）

```
GameManager.Start()
  → Initialize()
    → DataManager 初始化
    → BehaviorTracker 初始化 + 绑定 OnFocusStateChanged
    → StateMachine 初始化
    → LLM 客户端初始化（Mock 或 BlueLM）
      → GameManager.IsReady = true
```

**不要在 Awake/Start 里调用其他单例**，全部通过 Initialize 链式初始化。

### 3.4 Android 下的 LLM 调用

- 模型路径：`/sdcard/1225/`（vivo 真机）
- Prompt 格式：`[|Human|]:{输入}\n[|AI|]:`
- 纯文本生成，n_predict=512，temperature=0.95
- 回调必须在主线程执行（`UnityMainThreadDispatcher`）

---

## 四、关键常量

| 常量 | 值 | 位置 |
|------|----|------|
| 活跃度阈值 | 0.3 | `BehaviorTracker.activityThreshold` |
| 分析窗口 | 30 秒 | `BehaviorTracker.analysisWindow` |
| 连续专注次数要求 | 3 | `BehaviorTracker.consecutiveFocusRequired` |
| 过渡动画延迟 | 2 秒 | `SmartFocusAnalyzer`（硬编码） |
| LLM temperature | 0.95 | `BlueLLMClient` |
| LLM n_predict | 512 | `BlueLLMClient` |

---

## 五、编译与构建

### 5.1 本地编译检查（无需打开 Unity Editor）

```powershell
# 先关闭 Unity Editor，再运行
# 脚本位置：项目根目录/build_check.ps1
.\build_check.ps1
```

### 5.2 Android APK 打包流程

1. File → Build Settings → Android → Switch Platform
2. 确保 Scenes in Build 顺序正确（startscene → mainscene → FocusScene）
3. Player Settings → Package Name = `com.vivo.catlifefocus`
4. Build

### 5.3 Editor 下调试（Mock LLM）

`GameManager.useMockLLM = true`（默认），BehaviorTracker 规则分析工作正常，无需真机。

---

## 六、已上传文件说明

### 6.1 新增文件（本次上传）

| 文件 | 用途 |
|------|------|
| `.gitignore` | 过滤 Library/、Build/、*.apk 等 20GB+ 缓存 |
| `.gitattributes` | Git LFS 配置（.glb/.fbx 走 LFS） |
| `upload_to_github.ps1` | 上传脚本（PowerShell） |

### 6.2 已忽略的大文件（勿提交）

```
Library/          ~20GB（Unity 缓存，可重新生成）
Logs/             （日志）
Build/            （打包输出）
*.apk / *.aab     （APK 产物）
UserSettings/     （本地编辑器设置）
```

---

## 七、当前状态与待办

### 已完成
- ✅ 四状态机（Normal/Transition/Focus/Reward）
- ✅ 行为追踪器（30s 窗口 + 活跃度分数）
- ✅ 猫咪控制器（动画 + 随机小动作）
- ✅ SmartFocusAnalyzer（规则 + LLM 双分析模式）
- ✅ BlueLLMClient（vivo 端侧 SDK 集成）
- ✅ GameManager（单例 + 初始化链）
- ✅ 开屏、主场景、专注场景三个场景
- ✅ CameraTransition（相机动画）

### 未完成 / 待优化
- ⬜ FocusScene 内的完整 UI 交互（目前 FocusUIManager 框架在，细节可能需补全）
- ⬜ DataManager 的持久化格式确认（目前结构存在，具体字段以实际测试为准）
- ⬜ Android 真机完整联调（蓝心 3B SDK 在模拟器上无法测，必须真机）
- ⬜ APK 打包验证（确认所有场景在真机上加载正常）

---

## 八、继续开发建议

1. **优先用 Editor 测试**：BehaviorTracker 规则分析在 Editor 下完全可用，不需要真机
2. **真机调试**：连上 vivo 手机后用 `adb logcat | grep Unity` 看日志
3. **蓝心 3B 集成**：联系吴若琪确认 SDK 的 `LlmManager` 类路径是否与代码一致（`com.vivo.llm.LlmManager`）
4. **场景切换**：检查 Build Settings 里的 Scene 列表，确认大小写完全匹配

---

## 九、联系人

| 角色 | 负责人 | 对接内容 |
|------|--------|---------|
| Unity 脚本 | 陈泓森（你） | 本工程、状态机、BehaviorTracker |
| Blender 资产 | 傅钧烨 | .glb 模型、骨骼动画 |
| Android 集成 | 严辰乐 | APK 打包、签名、提交 |
| 大模型方案 | 吴若琪 | LLM 接口、蓝心 3B |
| 宣传物料 | 傅钧漪 | PPT、演示视频 |

---

*由 Hermes Agent 生成 · 2026-06-29*