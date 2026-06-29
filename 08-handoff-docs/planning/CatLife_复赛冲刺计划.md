# CatLife 复赛冲刺计划 v3（2026-06-29 当前进度版）

> 复赛提交截止：2026-07-06 08:00  
> 当前日期：2026-06-29，处于 W5 打磨期。  
> 本版根据本地文件盘点、猫咪动画交付包、猫咪小镇场景整理、Unity 交接包和现有技术文档同步更新。

---

## 2026-06-29 非 Unity 准备工作补充

为避免后续开发只停留在单点资产验证，本项目已补齐从当前状态到 APK/演示视频的执行方案：

- `07-tech-specs/CatLife_MVP_非Unity准备工作总方案.md`：汇总当前状态、资料调研结论、非 Unity 工作流和验收门禁。
- `07-tech-specs/CatLife_移动端3D性能预算与优化方案.md`：定义小镇、猫、粒子、贴图、光照、Android 性能预算。
- `07-tech-specs/CatLife_猫咪小镇场景Unity落地方案.md`：明确 no-merge 小镇源文件、导入步骤、黑线毛刺排查和 Prefab 结构。
- `08-handoff-docs/planning/CatLife_MVP_从当前状态到APK与演示视频执行计划.md`：把当前资产推进到 APK、真机验证和演示视频的 P0/P1 执行清单。
- `12-docs-package/00-index/CatLife_当前交付物位置索引_20260629.md`：同步最新可用文件、废弃文件和交付路径。
- `08-handoff-docs/planning/CatLife_复赛官方材料校准记录_20260629.md`：根据新增 PDF 校准 5 项提交物、评分权重、视频/海报规格和代码包要求。
- `01-competition-docs/`：已归档 `应用赛道复赛交流会-0528.pdf`、`创新创意经验分享.pdf`、`02-开发规约与质量门禁.pdf`、`03-评审对照检查表.pdf`、`04-官方信息完全整合.pdf`，作为当前复赛规则和质量门禁的本地依据。
- `07-tech-specs/CatLife_Android打包与真机QA方案.md`：补齐 APK 构建、真机安装、logcat、录屏、云真机和性能证据流程。
- `08-handoff-docs/planning/CatLife_演示视频脚本与镜头表.md`：按官方“尽量 <=3 分钟，最长 <=5 分钟”要求规划镜头。
- `08-handoff-docs/planning/CatLife_最终提交包检查表.md`：建立 `06-deliverables/final-submission/` 的五项提交物核对入口。
- `08-handoff-docs/planning/CatLife_作品介绍PPT_10页精修脚本.md`：把初赛材料重构为复赛 10 页真实作品表达。
- `08-handoff-docs/planning/CatLife_海报文案与版式方案.md`：按 70cm x 150cm 竖版海报要求准备主视觉和文案。
- `08-handoff-docs/planning/CatLife_用户验证访谈与问卷模板.md`：补齐至少 5 份用户验证证据的采集入口。
- `06-deliverables/llm-code-package-template/`：准备可打包的大模型调用代码模板和隐私降级样例。
- `08-handoff-docs/planning/CatLife_当前缺口审计_20260629.md`：从当前状态倒推 APK、视频、PPT、海报、代码包的未完成证据。
- `tools/final-submission/check-final-submission.ps1`：最终提交目录自动检查、哈希生成和密钥扫描脚本。
- `tools/final-submission/package-llm-code.ps1`：大模型代码包模板打包脚本，默认输出到 `work/`，避免误交模板。
- `08-handoff-docs/planning/CatLife_Android真机测试记录模板.md`：APK 真机验证记录表。
- `08-handoff-docs/planning/CatLife_录屏剪辑执行清单.md`：演示视频素材录制和剪辑执行清单。
- `08-handoff-docs/planning/CatLife_最终发布证据包与提交运行手册.md`：定义最终提交目录、`evidence/` 证据结构、ADB 命令、录屏和上传前验收。
- `tools/final-submission/init-final-evidence.ps1`：创建最终证据包目录和空模板文件，避免 APK 完成后证据散落。

当前判断：动画猫 MVP 已进入 Unity 交接包；猫咪小镇资产已整理好，但还不能视为已经进入正式 Unity 主场景，下一步必须在 Unity 中导入并记录性能数据。

---

## 当前项目状态

| 模块 | 当前状态 | 关键文件/位置 | 下一步 |
|---|---|---|---|
| 猫咪动画 | 已产出 10 个动作的最终包；第 1-9 个已验收，第 10 个为 draft ready for review | `06-deliverables/cat-animation-final-package-20260629/` | Unity 导入 Animator，逐动作播放验收 |
| 动画源文件 | 已整理为单一当前源文件，旧 `.blend/.blend1` 已可逆归档 | `03-3d-models/blender-work/CatLife_cat_animation_coordinate_corrected.blend` | 后续动画只从该文件继续 |
| 猫咪小镇场景 | 已整理当前安全场景；废弃合并方案已归档 | `03-3d-models/catlife-town/current/catlife_v2_view_clean_no_merge.blend` | 导出 Unity 运行时版本并做移动端性能验证 |
| 原始猫模型 | 已从根目录移入 3D 来源目录 | `03-3d-models/source-cat-models/original-meshy-quadruped/` | 仅作恢复/溯源，不作为当前动画工作文件 |
| Unity 交接包 | 已归类到交付目录；MCP 已完成动画导入、Animator 映射、状态脚本接入、`mainscene` 动画猫接入和 Play Mode 验证；已产出 `mvp-unity-assets` 增量资产目录 | `06-deliverables/unity-handoff-20260629/` | 用 `mvp-unity-assets` 还原最终 Unity 工程并做 Android 构建验证 |
| 行为识别/状态机 | 方案和事件 schema 已存在 | `07-tech-specs/` | 与 Unity 状态机和动画动作表对齐 |
| 文档交接 | 已有交接总览、开发规约、素材规范 | `08-handoff-docs/`, `10-art-guide/`, `12-docs-package/` | 按最终提交物补齐截图、视频、APK 状态 |

---

## 复赛 5 项提交物状态

| # | 提交物 | 当前判断 | 负责人 | 6/29 后续动作 |
|---|---|---|---|---|
| 1 | 作品宣传海报 | 未在本地确认终版 | 傅钧漪 | 以猫咪小镇当前场景和猫动画截图补齐终版素材 |
| 2 | 作品演示视频 | 未在本地确认终版 | 傅钧漪 | 使用 Unity/Blender 可视化素材完成 9:16 演示视频 |
| 3 | 可运行产品包 APK | 已有 Unity 交接包，但未在本次盘点中确认 APK | 严辰乐 | 还原 Unity 工程、导入最新动画、真机/模拟器验证 |
| 4 | 大模型调用代码包 | 技术方案已整理，源码包状态需 Unity/Android 侧确认 | 吴若琪 | 标注 API 调用位置，准备可审阅代码包 |
| 5 | 策划说明 PPT | 未在本地确认终版 | 傅钧漪 + 吴若琪 | 纳入最新动画、小镇场景、状态机闭环和评审亮点 |

---

## W5（2026-06-24 至 2026-06-30）收口计划

| 优先级 | 任务 | 验收标准 | 负责人 |
|---|---|---|---|
| P0 | Unity 导入猫咪 10 动作包 | MCP 验证已通过导入/映射：10 个目标动作可见、LoopTime 已设置、验证 Controller 已挂到 Cat，`CatController` 已接入真实 Animator state；`mainscene` 已有 `CatModel_AnimatedMVP`，Game View 可见橘猫，Play Mode 0 runtime errors | 陈泓森 |
| P0 | Unity 导入猫咪小镇当前场景 | 运行时显示正确；黑线毛刺不影响最终渲染；记录面数/内存/帧率 | 陈泓森 + 严辰乐 |
| P0 | Android 构建验证 | APK 可安装启动；核心专注闭环可演示 | 严辰乐 |
| P1 | 行为状态到动画映射 | 已完成第一版：Normal/Focus 使用 IdleBreath，Transition 使用 CuriousSniff，Reward 使用 TailWagHappy；下一步按视觉验收微调 | 吴若琪 + 陈泓森 |
| P1 | 演示素材采集 | 小镇全景、猫特写、4 状态动作、专注流程录屏齐备 | 傅钧漪 |
| P1 | 文档与交接包复核 | 关键路径、负责人、提交物状态一致，无旧路径误导 | 吴若琪 |

---

## W6（2026-07-01 至 2026-07-06 08:00）最终提交计划

| 日期 | 任务 | 验收标准 |
|---|---|---|
| 7/1 | APK、视频、PPT 三件套对齐 | 视频内容与 APK 实际功能一致 |
| 7/2 | 大模型代码包整理 | API 调用段有注释，敏感信息不入包 |
| 7/3 | 全量回归 | Android 安装、启动、专注开始/结束、状态切换、动画播放通过 |
| 7/4 | 宣传海报/视频/PPT 终审 | 尺寸、时长、文件格式符合比赛要求 |
| 7/5 | 提交包预演 | 5 项材料放入最终提交目录并逐项打开检查 |
| 7/6 08:00 前 | 平台上传 | 上传成功截图留底 |

---

## 当前技术门禁

| 门禁 | 标准 | 当前依据 |
|---|---|---|
| 动画坐标 | Blender 源：`+Z` up，头朝 `-Y`，尾朝 `+Y` | `cat_actions_manifest.json` 与动画生产记录 |
| 动画数量 | 至少 10 个可识别动作 | `06-deliverables/cat-animation-final-package-20260629/cat_actions_manifest.json` |
| 小镇场景 | 使用 no-merge 当前版本，不使用 mesh clean 废弃版 | `03-3d-models/catlife-town/README.md` |
| 移动端性能 | 不以 Blender 面数直接下结论，必须在 Unity/Android Profiler 验证 | 场景落地方案与本地盘点 |
| 文件治理 | 大资产本地保留，轻量文档/manifest/截图进入 Git | `.gitignore` 与本地文件索引 |

---

## 风险追踪

| 风险 | 等级 | 当前触发点 | 应对 |
|---|---|---|---|
| 3D 场景过重导致移动端卡顿 | 高 | 小镇和猫资产均为高体量本地文件 | Unity 中按模块导入，先 Profile，再针对性减面/合批/贴图压缩 |
| 动画导入后方向或缩放错误 | 高 | Blender 与 Unity 坐标系不同 | 保持 Blender 源坐标不改，在 FBX/Unity Import 设置处理转换 |
| 第 10 个动作未最终验收 | 中 | manifest 标记为 `draft_ready_for_review` | Unity 导入时单独标记，如时间不足可作为备用动作 |
| 视频/PPT 未吸收最新素材 | 中 | 旧计划未反映 6/29 动画和小镇成果 | 使用本版计划和 `06-deliverables` 作为素材清单 |
| 大二进制误提交 | 中 | 本地 `.blend/.glb/.fbx/.zip` 很多 | 继续依赖 `.gitignore`，只提交文档、manifest、轻量 QA 图 |

---

## 文件清理结果

- `03-3d-models/blender-work/`：根目录仅保留当前动画源文件、生产说明、导出和 QA 帧。
- `03-3d-models/blender-work/archive/20260629-animation-iterations/`：归档旧动画 `.blend/.blend1` 中间版本。
- `03-3d-models/catlife-town/`：已整理为 `current/source/exports/reports/archive`。
- `03-3d-models/source-cat-models/original-meshy-quadruped/`：归档原始 Meshy 猫模型包。
- `work/local-file-inventory-20260629.md`：记录本次本地文件整理边界。

---

*v3 更新：Codex | 2026-06-29*  
*变更：同步猫咪 10 动作包、猫咪小镇当前场景、本地文件整理结果，并将计划从周计划改为提交前收口计划。*
