# CatLife UI 顶格比赛提交装配说明

日期：2026-06-29
目标：把 UI 文档升级为“看到即可实现”的比赛提交级装配说明，配套预览图、图片素材、布局 JSON、Unity 脚本和一键安装脚本。

## 1. 交付包位置

```text
06-deliverables/catlife-ui-assembly-kit-20260629/
```

核心入口：

```text
06-deliverables/catlife-ui-assembly-kit-20260629/README.md
```

## 2. 预览图

总览：

![CatLife UI contact sheet](../../06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/00_contact_sheet.png)

单屏：

| 页面 | 图片 |
|---|---|
| 启动页 | `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/01_splash.png` |
| 主小镇页 | `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/02_main_town.png` |
| 专注准备卡 | `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/03_focus_setup.png` |
| 专注进行层 | `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/04_focus_running.png` |
| 奖励结算 | `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/05_reward_summary.png` |
| 记录页 | `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/06_records.png` |
| 隐私与智能解释 | `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/07_privacy_llm.png` |

## 3. 一键安装

```powershell
powershell -ExecutionPolicy Bypass -File 06-deliverables\catlife-ui-assembly-kit-20260629\scripts\install-to-unity.ps1 -UnityProjectPath "D:\YourUnityProject"
```

安装后 Unity 目录：

```text
Assets/CatLife/UIAssemblyKit/
  assets/
  layout/
  unity-scripts/
```

## 4. 顶格比赛提交组装顺序

| 阶段 | 操作 | 结果 |
|---|---|---|
| 1 | 安装 UI kit 到 Unity | 素材、layout、脚本进入工程 |
| 2 | 创建 `CatLife_Main.unity` | 作为最终主场景 |
| 3 | 创建 Canvas 1080x1920 | 对齐预览图尺寸 |
| 4 | 创建 7 个 Panel | Splash/Main/FocusSetup/FocusRunning/Reward/Records/Privacy |
| 5 | 先用预览图做全屏 Image | 10 分钟内搭出可点击 Demo |
| 6 | 拆分正式 UI 组件 | 用按钮、文本、图标替换整图 |
| 7 | 接入 `CatLifeUIScreenController` | 所有页面可切换 |
| 8 | 接入猫咪聊天气泡 | 猫可根据状态冒出提醒/鼓励 |
| 9 | 接入状态机 | Normal/Transition/Focus/DistractionNudge/Reward 驱动 UI 和气泡 |
| 10 | 替换正式 Game View 截图 | 消除工作视图网格，进入比赛版画面 |
| 11 | 真机录屏 | 生成 PPT/视频/海报用证据 |

## 5. 为什么这样设计

这套 UI 只围绕 CatLife 的真实产品闭环：

```text
启动 -> 小镇首页 -> 专注准备 -> 专注陪伴 -> 奖励结算 -> 记录 -> 隐私/LLM
```

明确不做：

- 活动页；
- 抽奖；
- 商城；
- 签到；
- 比赛重置；
- 社交排行。

原因：这些会稀释 CatLife 的核心创新点，也不服务复赛 5 项提交物。

## 6. 和比赛提交物的对应

| 提交物 | 使用方式 |
|---|---|
| APK | 直接使用 Unity 装配后的 7 屏 UI 和状态机 |
| 演示视频 | 按 contact sheet 顺序录制：启动、小镇、专注、奖励、记录、隐私 |
| PPT | 使用 `assets/previews` 作为设计占位，最终替换真机截图 |
| 海报 | 使用主小镇页和奖励页作为主视觉布局参考 |
| 大模型代码包 | 隐私/智能解释页与 LLM 降级脚本对应 |

## 7. 当前素材限制

当前预览图已经能用于装配，但还不是最终比赛截图：

| 限制 | 处理 |
|---|---|
| 小镇背景来自现有 demo preview，仍有工作视图痕迹 | 后续用正式 Unity Game View/真机截图替换 |
| 猫咪专注态部分为示意图 | 后续用真实猫动画截图替换 |
| 记录数据为示例 | 后续用 Demo 会话或真机测试数据替换 |

## 8. 后续实现者最低读取顺序

1. `06-deliverables/catlife-ui-assembly-kit-20260629/README.md`
2. `06-deliverables/catlife-ui-assembly-kit-20260629/assets/previews/00_contact_sheet.png`
3. `06-deliverables/catlife-ui-assembly-kit-20260629/layout/catlife_ui_layout.json`
4. `06-deliverables/catlife-ui-assembly-kit-20260629/unity-scripts/CatLifeUIScreenController.cs`
5. `06-deliverables/catlife-ui-assembly-kit-20260629/unity-scripts/CatLifeCatChatBubbleController.cs`
6. `08-handoff-docs/planning/CatLife_猫咪聊天气泡模块实现说明_20260630.md`
7. `07-tech-specs/CatLife_Unity实现技术与脚本概念_20260629.md`

## 9. 完成定义

只有满足以下条件，才算这套 UI 真正进入顶格比赛提交状态：

- Unity 中 7 个 Panel 均可点击切换；
- 主小镇页使用正式 Unity/Android 运行画面；
- 猫咪聊天气泡能按状态提醒/鼓励，并跟随猫咪锚点；
- 专注页使用猫咪真实专注动画；
- 奖励页使用真实奖励动画或正式图标；
- 隐私页能对应大模型代码包；
- 真机录屏中 UI、猫咪状态、LLM/隐私说明能够连贯展示。
