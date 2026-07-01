# CatLife — 低多边形猫咪陪伴式游戏化专注软件

> 中国高校计算机大赛 · AIGC 创新赛 · 应用赛道复赛
> 团队：大学城今天吃什么 | 状态：复赛开发冲刺中（截止 2026-07-06 8:00）

## 项目定位

Cat Life 是一款面向大学生学习与备考场景的陪伴式专注辅助软件。通过 AI 行为识别判断用户从高频操作到专注的状态迁移，再用虚拟猫的行为反馈和轻锁定界面，引导用户自然进入专注。

**核心表达**：陪伴式设计替代高压管理，温和引导替代惩罚打断，可持续专注替代短时控制。

## 技术栈

| 维度 | 选型 |
|------|------|
| 主引擎 | Unity (主场景、猫咪动画、状态机、交互) |
| 打包平台 | Android Studio (工程集成、真机调试、APK) |
| 3D资产 | Meshy AI (.glb) + ChatGPT DALL·E 参考图 |
| AI行为识别 | MVP: 规则引擎 → 扩展: LLM辅助判定 |
| 大模型 | 端侧: 蓝心3B (猫咪对话) + 云端: DeepSeek-V3 (专注分析) |
| 团队规模 | 5人 (傅钧烨/吴若琪/陈泓森/严辰乐/傅钧漪) |
| 心理学支撑 | SDT自我决定理论 + 正念注意力回归 + 轻约束设计 |

## 复赛关键信息

| 项目 | 内容 |
|------|------|
| 提交截止 | **2026-07-06 8:00**（非6/23！） |
| 评审标准 | 创新性40% / 应用价值30% / 完成度20% / 大模型应用10% |
| 提交物 | PPT + 演示视频(≤5min) + 海报 + 可运行APK + 代码包 |
| vivo工具 | 云真机(X300 Pro/Android16) + 蓝心3B端侧模型SDK |
| 每队获真机 | 1台vivo手机用于调试 |

## 核心逻辑链

```
AI行为识别 → 判断专注状态 → 猫咪行为驱动 → 心理引导 → 专注形成
    │              │                │              │
  点击/滑动    普通→过渡       活跃→安静       SDT自主性
  停顿/切屏    →专注→奖励      交互→陪伴       胜任感+关系感
```

## MVP 四状态机

| 状态 | 触发 | 猫咪行为 | UI反馈 |
|------|------|----------|--------|
| 普通 | 高频操作 | 活跃可互动 | 完整按钮 |
| 过渡 | 频率下降 | 靠近注视 | 弱提示 |
| 专注 | 低频持续 | 安静趴下 | 轻锁定/上滑退出 |
| 奖励 | 完成专注 | 庆祝反馈 | 记录成长 |

## 产品差异化

| 类型 | 传统做法 | Cat Life |
|------|---------|----------|
| 番茄钟 | 倒计时管理 | 先识别状态，再引导专注 |
| 锁机类 | 强制限制 | 轻锁定，保留退出权 |
| 宠物类 | 情绪陪伴 | 猫咪随状态降低活跃度 |

## 目录结构

```
05-AIGC/
├── README.md                              ← 本文件
├── AGENTS.md                              ← Codex/Agent 项目规则
│
├── 01-competition-docs/                   ← 比赛文档+官方材料
│   ├── 中国高校计算机大赛-AIGC创新赛.pdf
│   ├── 创新创意经验分享.pdf                ← 复赛交流会材料
│   ├── 应用赛道复赛交流会-0528.pdf          ← 复赛交流会材料
│   ├── 复赛交流会-录屏.mp4                 ← 复赛交流会录屏(212MB)
│   ├── Cat_Life_项目框架与4人分工.docx
│   └── psychology/                        ← 心理学支撑
│
├── 02-asset-specs/                        ← 资产需求与预览汇总
├── 03-3d-models/                          ← Blender源文件、猫动画、小镇场景、原始模型包
│   ├── blender-work/                      ← 当前猫动画源文件+QA+导出
│   ├── catlife-town/                      ← 当前猫咪小镇场景+来源+归档
│   └── source-cat-models/                 ← 原始猫模型来源包
├── 04-reference-images/                   ← 参考图(82张场景 + 4张UI)
├── 05-textures/                           ← 贴图纹理
├── 06-deliverables/                       ← 提交物
│   ├── AIGC大赛初赛PPT_v3_22页.pptx
│   ├── CatLife_15页路演架构_心理学支撑文案.docx
│   ├── cat-animation-final-package-20260629/
│   └── unity-handoff-20260629/
├── 07-tech-specs/                         ← 技术方案
├── 08-handoff-docs/                       ← 交接文档
│   ├── planning/                          ← 计划、规约、评审、官方信息整合
│   └── role-plans/                        ← 5名成员分工文档
├── 09-defense/                            ← 答辩材料
└── 10-art-guide/                          ← 美术规范
```

## 当前进度（2026-07-01）

| 模块 | 状态 | 关键位置 |
|------|------|----------|
| 当前项目思路 | 已整理为新的 7/1 高层入口，后续以“最新小镇 + 当前猫动画 + Unity 干净主场景 + Android 真机证据”为主线 | `08-handoff-docs/planning/CatLife_当前项目内容与开发思路总览_20260701.md` |
| 完整开发流程 | 已按人类团队真实工作流拆到完整比赛提交级别 | `08-handoff-docs/planning/CatLife_人类团队完整开发流程调研_20260701.md` |
| 猫咪动画 | 已产出 10 个动作包，作为 Unity Animator 动作来源 | `06-deliverables/cat-animation-final-package-20260629/` |
| 当前动画源文件 | 已整理为单一工作源文件 | `03-3d-models/blender-work/CatLife_cat_animation_coordinate_corrected.blend` |
| 猫咪小镇 | 以 no-merge / no-skybox 思路为准；草地、底座和光照风格已有 style-pass 报告，正式天空建议在 Unity 做 | `03-3d-models/catlife-town/current/`、`03-3d-models/catlife-town/reports/style-pass/` |
| 原始猫模型 | 已移入 3D 来源目录 | `03-3d-models/source-cat-models/original-meshy-quadruped/` |
| UI / 用例 / 气泡 | 已完成 UI 规格、主页目标稿、摄像机控制和猫咪聊天气泡模块 | `07-tech-specs/`、`08-handoff-docs/planning/` |
| Unity 参考内容 | 协作者旧工程和 Unity work batch 已入库作参考；不能直接作为最终生产基线 | `06-deliverables/unity-collaborator-reference-20260629/`、`work/unity-android-build-batch-20260629/` |
| 下一步 | 建立 Unity 干净主场景，导入小镇/猫/基础 UI，完成 Android 真机闭环证据 | `08-handoff-docs/planning/CatLife_当前项目内容与开发思路总览_20260701.md` |

## 权威文件位置

| 类型 | 位置 |
|------|------|
| 当前项目内容与开发思路 | `08-handoff-docs/planning/CatLife_当前项目内容与开发思路总览_20260701.md` |
| 人类团队完整开发流程 | `08-handoff-docs/planning/CatLife_人类团队完整开发流程调研_20260701.md` |
| 开发规约 | `08-handoff-docs/planning/CatLife_DEV-GUIDE_复赛开发规约.md` |
| 当前冲刺计划 | `08-handoff-docs/planning/CatLife_复赛冲刺计划.md` |
| 评审检查表 | `08-handoff-docs/planning/CatLife_复赛评审对照检查表.md` |
| 官方信息整合 | `08-handoff-docs/planning/复赛交流会_信息整合报告.md` |
| 成员分工 | `08-handoff-docs/role-plans/` |
| Unity 交接包 | `06-deliverables/unity-handoff-20260629/` |
| 猫动画最终包 | `06-deliverables/cat-animation-final-package-20260629/` |

> 当前重点是 Unity/Android 真实运行闭环：先让猫、小镇、固定摄像机和基础 UI 在真机上稳定出现，再用真实画面替换 PPT/视频/海报中的概念图。

---

*由 Codex 同步维护 · 最后更新 2026-07-01*
