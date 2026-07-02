# CatLife Unity 开发流程规范：从当前状态到全量提交

日期：2026-07-02
适用范围：`work/CatLife_Unity_Main/` 主 Unity 工程、Unity 运行时代码、Editor 工具脚本、3D 资产导入、场景装配、Android 构建前验证、Git 提交与推送。

## 1. 当前状态基线

本规程建立时的项目状态：

| 项 | 当前值 |
|---|---|
| Git 分支 | `main` |
| 已推送基线 | `9ae7f8f Add town cat walking animation setup` |
| Unity 主工程 | `work/CatLife_Unity_Main/` |
| 当前主场景 | `Assets/Scenes/MainScene.unity` |
| 当前猫行走动画源 | `03-3d-models/blender-work/exports/cat-animations/CL_CAT_SRC_Walk_60fps.fbx` |
| 当前运行时猫位置 | `(0, 0.03, -8.5)`，缩放 `0.055`，挂载 `Animator` 和 `CatTownWalker` |
| 当前剩余未跟踪项 | `work/CatLife_Unity_Main/Assets/Screenshots/` 下历史验证截图 |

当前未跟踪截图不属于 Unity 源码变更。进入“全量提交”前必须明确处理：

| 处理方式 | 适用情况 | Git 动作 |
|---|---|---|
| 删除 | 只是临时验证截图，不会用于 PPT、视频、证据包 | 删除 `.png` 和对应 `.meta` |
| 晋升为证据 | 会作为最终证据或演示素材使用 | 移到 `06-deliverables/final-submission/evidence/03-screenshots/` 或命名清楚后提交 |
| 暂留本地 | 仍需人工对比，暂时不进提交 | 在最终状态说明中明确“未跟踪旧截图保留本地” |

## 2. 总原则

| 原则 | 具体要求 |
|---|---|
| 先读现状，再改工程 | 每次改 Unity 前先看 `git status`、主场景、Console、相关文档和资产 manifest。 |
| Unity 工程为运行基线 | 当前可运行工程只认 `work/CatLife_Unity_Main/`，旧 handoff 和参考工程只读。 |
| 大二进制不默认进 Git | `.blend`、`.fbx`、`.glb`、视频、zip 默认按 `.gitignore` 和 `IMPORT_MANIFEST.md` 记录来源、大小、hash。 |
| 场景改动必须可验证 | 场景、材质、动画、相机、UI 改动必须通过 Console、Play Mode、截图或对象查询验证。 |
| 脚本改动必须等编译 | C# 新增或修改后必须等 Unity 编译完成，再读 Console。 |
| 只提交意图明确的文件 | `git add` 必须列具体路径，禁止把旧截图、缓存、忽略的大文件混进提交。 |
| 全量提交不等于全文件提交 | 全量提交指“所有应该进入版本控制的源码、场景、配置、文档、证据都已提交”，不是强行提交所有本地文件。 |

## 3. 阶段门禁总览

| 阶段 | 名称 | 输入 | 通过标准 | 输出 |
|---:|---|---|---|---|
| 0 | 任务定界 | 用户需求、当前文档索引 | 明确改什么、不改什么、是否需要 Unity MCP/Blender/Android | 简短执行口径 |
| 1 | 当前状态冻结 | Git、Unity、文档、资产状态 | 知道当前分支、未跟踪文件、主场景、关键对象 | 状态快照 |
| 2 | 资料和源资产确认 | 技术规格、manifest、源 FBX/GLB/贴图 | 确认权威源和目标 Unity 路径 | 资产映射表 |
| 3 | Unity 实施 | C#、场景、Prefab、材质、Animator、UI | 改动范围和项目结构一致 | Unity 文件变更 |
| 4 | Unity 验证 | 编译、Console、Play Mode、截图、对象查询 | 无脚本错误，核心行为可复现 | 验证记录 |
| 5 | 文档和证据同步 | manifest、开发文档、截图/录屏 | 资产来源、hash、验证结果可追溯 | 更新文档和证据 |
| 6 | Git 分类和暂存 | `git status`、diff、ignore 规则 | 暂存区只含本次应提交文件 | 干净暂存区 |
| 7 | 提交和推送 | 暂存区、验证结论 | commit 成功，push 到远端 | 新 commit hash |
| 8 | 全量提交确认 | 远端状态、剩余本地文件 | 源控应提交内容已全部提交；剩余项有解释 | 最终交接说明 |

## 4. 阶段 0：任务定界

每次 Unity 开发任务开始前，先回答四个问题：

| 问题 | 例子 |
|---|---|
| 改动对象是什么 | 猫动画、主场景、小镇材质、相机、UI、状态机、Android 构建 |
| 权威来源在哪里 | `07-tech-specs/`、`08-handoff-docs/planning/`、`03-3d-models/`、`IMPORT_MANIFEST.md` |
| 是否需要外部编辑器 | Blender MCP、Unity MCP、Android/ADB、Playwright 或普通文件编辑 |
| 不改什么 | 不动旧参考工程、不重写无关 docs、不提交本地大二进制、不清理用户未要求的截图 |

必须先读：

```text
08-handoff-docs/planning/CatLife_开发文档索引表_20260702.md
07-tech-specs/CatLife_猫咪小镇场景Unity落地方案.md
work/CatLife_Unity_Main/Assets/Art/IMPORT_MANIFEST.md
```

涉及行为识别或猫状态时加读：

```text
07-tech-specs/behavior_event_schema.json
07-tech-specs/cat_reaction_state_table.json
07-tech-specs/CatLife_非敏感行为识别与猫咪行为控制实现方案.md
```

## 5. 阶段 1：当前状态冻结

### 5.1 Git 状态

```powershell
git status --branch --short
git log -1 --oneline
git diff --stat
git diff --cached --stat
```

判定规则：

| 状态 | 处理 |
|---|---|
| 只有旧截图未跟踪 | 记录为本地验证残留，不默认提交。 |
| 有用户未提交源码改动 | 先读 diff，判断是否相关；不相关则绕开。 |
| 有 Unity 自动生成配置 | 判断是否是当前操作必需，例如 `ProjectSettings`、`Packages`、`.meta`。 |
| 有忽略的大资产 | 不强行 `git add -f`，先更新 manifest 或请求明确批准。 |

### 5.2 Unity 状态

使用 Unity MCP 时，顺序固定：

```text
1. 读 mcpforunity://instances
2. 读 mcpforunity://custom-tools
3. 读 mcpforunity://editor/state
4. 读 Console
5. 查询目标 GameObject、组件、材质或资产
```

最低检查项：

| 项 | 通过标准 |
|---|---|
| Editor | 非 Play Mode，非编译中，主场景已打开 |
| Console | 没有本任务相关 Error |
| 主场景 | `Assets/Scenes/MainScene.unity` |
| Main Camera | 能看到猫和小镇目标区域 |
| Cat | `CatCompanionModel` 存在，材质、Animator、位置符合当前目标 |
| Town | `CatLifeTownRoot` 存在，旧 FBX backup 不参与运行 |

## 6. 阶段 2：资料和源资产确认

### 6.1 资产源选择

| 资产类型 | 权威来源 | Unity 目标位置 | 必须记录 |
|---|---|---|---|
| 小镇模型 | `03-3d-models/catlife-town/current/` | `Assets/Art/Town/Source/` | 文件名、大小、SHA256、导入方式 |
| 猫模型和动画 | `03-3d-models/blender-work/exports/cat-animations/` 或 `06-deliverables/cat-animation-final-package-20260629/` | `Assets/Art/Cat/Animations/` | 源 rig、目标 rig、clip 名、loop |
| 猫贴图 | `03-3d-models/source-cat-models/` | `Assets/Art/Cat/Textures/` | base/normal/metallic/roughness 来源 |
| 材质 | Unity 内生成或导入 | `Assets/Art/**/Materials/` | shader、贴图绑定、颜色 |
| UI 素材 | `06-deliverables/catlife-ui-assembly-kit-20260629/` | `Assets/` 下对应 UI 路径 | 尺寸、用途、是否运行时用 |

### 6.2 Hash 和大小

导入或替换资产前后都记录：

```powershell
Get-Item -LiteralPath "<path>" | Select-Object FullName,Length,LastWriteTime
Get-FileHash -Algorithm SHA256 -LiteralPath "<path>"
```

写入：

```text
work/CatLife_Unity_Main/Assets/Art/IMPORT_MANIFEST.md
```

如果资产被 `.gitignore` 忽略，manifest 中 `Git policy` 写：

```text
Local binary, ignored
```

## 7. 阶段 3：Unity 实施

### 7.1 文件放置规则

| 类型 | 路径 |
|---|---|
| Runtime C# | `work/CatLife_Unity_Main/Assets/Scripts/<Module>/` |
| Editor 工具 | `work/CatLife_Unity_Main/Assets/Editor/` |
| 猫动画 clip | `work/CatLife_Unity_Main/Assets/Art/Cat/Animations/Clips/` |
| 猫 Animator Controller | `work/CatLife_Unity_Main/Assets/Art/Cat/Animator/` |
| 猫材质和贴图 | `work/CatLife_Unity_Main/Assets/Art/Cat/Materials/`、`Textures/` |
| 小镇材质 | `work/CatLife_Unity_Main/Assets/Materials/TownGLB/` 或对应 art 路径 |
| 主场景 | `work/CatLife_Unity_Main/Assets/Scenes/MainScene.unity` |

### 7.2 脚本开发规则

| 规则 | 说明 |
|---|---|
| Runtime 与 Editor 分离 | Editor 代码不得放入 Runtime 目录，避免 Android build 带入 `UnityEditor`。 |
| 命名贴合模块 | 例如 `CatTownWalker`、`CatLifeCatTownWalkerSetup`。 |
| Inspector 字段可调 | 位置范围、速度、状态参数等用 `[SerializeField]`。 |
| 不硬编码无来源数据 | 必须从规格文档、manifest 或当前场景验证得来。 |
| 新脚本必须校验 | 用 Unity 编译、Console、`validate_script` 或等价方式确认。 |

### 7.3 场景和 Prefab 修改规则

| 改动 | 必须同时检查 |
|---|---|
| 猫位置/缩放 | 主相机画面、移动范围、地面高度 |
| 相机 | 9:16 和当前 Game View 构图 |
| 灯光 | URP 支持性；Area Light 必须 Baked |
| 材质 | URP shader、贴图、颜色和移动端预算 |
| Animator | controller、参数、clip loop、bone binding |
| UI Canvas | 不遮挡猫和主 CTA，中文字体可显示 |

## 8. 阶段 4：Unity 验证

### 8.1 编译和 Console

脚本或场景修改后：

```text
1. 触发 Unity refresh 或等待自动编译
2. 读 editor_state，确认 is_compiling=false
3. read_console(types=["error","warning"])
4. 对脚本运行 validate_script
```

通过标准：

| 项 | 标准 |
|---|---|
| C# 编译 | 0 error |
| Console | 0 本任务相关 error；warning 必须解释或修复 |
| Editor | 不停留在 Play Mode |

### 8.2 对象级验证

用 Unity 内查询确认，而不是只看文件：

| 目标 | 验证内容 |
|---|---|
| Cat | `position`、`rotation`、`scale`、`Animator`、`CatTownWalker` |
| Walk clip | `loopTime=true`、长度合理、binding root 是当前 rig |
| Controller | 有 `IsWalking` 参数、Idle/Walk 状态、transition 条件正确 |
| Town | active root、renderer 数、材质数、备份 root 是否 disabled |
| Camera | FOV、位置、画面里猫与小镇关系 |

### 8.3 Play Mode 验证

涉及运行时行为必须至少跑一次 Play Mode：

| 行为 | 通过标准 |
|---|---|
| 猫自由行走 | 位置随时间变化，仍在允许范围内，`IsWalking` 状态正确 |
| 状态机 | Normal/Transition/Focus/Reward 可切换 |
| UI | 按钮可点，关键文案不重叠 |
| 场景 | 没有 NullReference、材质丢失、URP 错误 |

建议记录：

```text
进入 Play Mode 前位置
运行 2-5 秒后位置
Animator 参数
Console 结果
截图路径或截图是否仅临时
```

## 9. 阶段 5：文档和证据同步

### 9.1 必改文档

| 触发条件 | 必改文档 |
|---|---|
| 导入/替换模型、贴图、动画、材质 | `work/CatLife_Unity_Main/Assets/Art/IMPORT_MANIFEST.md` |
| 新增关键开发流程 | `08-handoff-docs/planning/CatLife_开发文档索引表_20260702.md` |
| 影响 Android 或最终提交 | `CatLife_最终提交包检查表.md` 或最终证据手册 |
| 影响行为识别/状态机 | 对应 `07-tech-specs/` 文档或 JSON |

### 9.2 证据策略

| 证据 | 存放建议 | 是否提交 |
|---|---|---|
| 临时 Unity 截图 | `Assets/Screenshots/` | 不提交，验证后删除或保留本地 |
| 最终运行截图 | `06-deliverables/final-submission/evidence/03-screenshots/` | 提交 |
| 真机录屏 | `06-deliverables/final-submission/evidence/04-recordings/` | 按体积和提交策略决定 |
| 构建日志 | `06-deliverables/final-submission/evidence/00-build/` | 提交文本日志 |
| APK | `06-deliverables/final-submission/` | 通常本地或发布包；是否进 Git 需确认体积策略 |

## 10. 阶段 6：Git 分类和暂存

### 10.1 分类表

| 文件类型 | 默认动作 |
|---|---|
| `.cs`、`.asmdef`、`.json`、`.md` | 提交 |
| `.unity`、`.prefab`、`.controller`、`.anim`、`.mat`、`.asset` | 如果是本任务必需，提交 |
| `.meta` | 与提交资产配套提交 |
| `.fbx`、`.glb`、`.blend` | 默认不提交，manifest 记录 |
| `.png` 截图 | 临时截图不提交；最终证据才提交 |
| `.mp4`、`.apk`、`.zip` | 默认不提交，除非最终提交策略明确要求 |
| `Library/`、`Temp/`、`Logs/` | 永不提交 |

### 10.2 暂存命令

禁止：

```powershell
git add .
```

推荐：

```powershell
git add -- `
  work/CatLife_Unity_Main/Assets/Scripts/Cat/CatTownWalker.cs `
  work/CatLife_Unity_Main/Assets/Scripts/Cat/CatTownWalker.cs.meta `
  work/CatLife_Unity_Main/Assets/Scenes/MainScene.unity `
  work/CatLife_Unity_Main/Assets/Art/IMPORT_MANIFEST.md
```

### 10.3 暂存后检查

```powershell
git diff --cached --name-status
git diff --cached --stat
git diff --cached --name-only | Select-String -Pattern 'Screenshots|\.fbx$|\.glb$|\.blend$|\.apk$|\.mp4$|\.zip$'
git diff --cached --check -- '*.cs' '*.md'
git status --short
```

说明：Unity YAML 和 `.meta` 常有空值尾随空格，`git diff --check` 可对 `.cs` 和 `.md` 强制执行；对 Unity 自动序列化文件只做人工判断。

## 11. 阶段 7：提交和推送

提交前必须满足：

| 项 | 标准 |
|---|---|
| Unity Console | 0 本任务相关 error |
| 脚本 | 0 编译 error |
| Play Mode | 已退出 |
| 暂存区 | 无旧截图、无未批准大文件 |
| 文档 | manifest 或索引已同步 |

命令：

```powershell
git commit -m "<动词开头的简短说明>"
git push
git status --branch --short
git log -1 --oneline
```

提交信息示例：

| 场景 | Commit message |
|---|---|
| 猫行走 | `Add town cat walking animation setup` |
| 小镇材质 | `Restore town material colors in Unity` |
| 文档流程 | `Document Unity development workflow` |
| Android 证据 | `Add Android submission evidence checklist` |

## 12. 阶段 8：全量提交确认

全量提交完成的定义：

| 检查 | 通过标准 |
|---|---|
| 远端同步 | `git status --branch --short` 显示当前分支和远端无 ahead/behind |
| 未暂存源码 | 没有应提交的 `.cs`、`.unity`、`.prefab`、`.controller`、`.anim`、`.mat`、`.asset`、`.md`、`.json` |
| 本地残留 | 每个未跟踪文件都有分类：临时、证据候选、忽略大文件 |
| 忽略资产 | `.fbx/.glb/.blend` 已在 manifest 或资产报告中记录 |
| 验证证据 | Console、Play Mode、截图/日志结果已记录到最终答复或证据目录 |

当前项目从现状到全量提交的最短路径：

```text
1. 确认当前未跟踪截图是否还要作为证据。
2. 如果不需要，删除旧截图和对应 .meta。
3. 如果需要，移动到 final-submission/evidence 并重命名。
4. 运行 git status，确认没有其它源码未提交。
5. 对被晋升为证据的文件运行 hash 和内容检查。
6. git add 具体路径。
7. git diff --cached --name-status，确认没有临时文件。
8. git commit。
9. git push。
10. final answer 中写清 commit hash、验证项、剩余本地限制。
```

## 13. Android 和最终提交接力

Unity 全量提交不是比赛最终提交。进入 APK 和最终材料前，还要按以下顺序接力：

| 阶段 | 入口文档 | 输出 |
|---|---|---|
| Android 构建 | `07-tech-specs/CatLife_Android打包与真机QA方案.md` | APK、构建日志 |
| 真机验证 | `08-handoff-docs/planning/CatLife_Android真机测试记录模板.md` | 安装日志、logcat、设备信息 |
| 录屏剪辑 | `08-handoff-docs/planning/CatLife_录屏剪辑执行清单.md` | 原始录屏、剪辑素材 |
| 最终包 | `08-handoff-docs/planning/CatLife_最终发布证据包与提交运行手册.md` | `final-submission/` 证据结构 |
| 提交核对 | `08-handoff-docs/planning/CatLife_最终提交包检查表.md` | 可上传的 5 项材料 |

## 14. 常见错误和处理

| 问题 | 处理 |
|---|---|
| Unity 和 Blender 画面不一致 | 先确认源文件、导出格式、材质策略、相机和 Unity 导入器；不要直接重调白材质。 |
| 低面数看起来异常 | 区分低多边形风格、Unity LOD/导入压缩、错误模型版本和材质缺失。 |
| 动画不动 | 检查 rig root、clip binding、Animator Controller、参数名、Play Mode 状态。 |
| Console 出 URP Area Light 错误 | Rectangle/Area Light 在 URP 下必须 Baked，不能 Realtime 或 Mixed。 |
| `.meta` 丢失 | 与资产一起提交对应 `.meta`，否则 Unity GUID 会变。 |
| 大 FBX 未出现在 Git status | 先查 `.gitignore`，默认不强行提交，记录 manifest。 |
| Unity 自动改了场景 YAML | 用对象级验证判断是否行为相关；无关但同一场景保存造成的 fileID 重排要在总结中说明。 |

## 15. 每次 Unity 任务结束模板

```text
完成内容：
- 改了哪些 Unity 对象/脚本/资产。

验证：
- Unity 编译：
- Console：
- Play Mode：
- 截图或对象查询：

Git：
- Commit：
- Push：
- 剩余未跟踪项：

限制：
- 哪些大文件未入 Git：
- 哪些只是本地证据：
```
