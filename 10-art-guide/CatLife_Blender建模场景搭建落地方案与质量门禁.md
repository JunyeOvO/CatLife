# CatLife Blender 建模与场景搭建落地方案及质量门禁

负责人：傅钧烨
版本：v1.0
日期：2026-06-22
适用范围：Blender 建模、场景搭建、材质整理、Runtime/Render 双轨文件、Unity 交接、PPT/海报/视频渲染素材

## 1. 职责边界

你的职责不是实现 Unity URP、Android APK 或大模型调用，而是把所有视觉资产的“源头质量”和“交接可用性”做扎实。

| 属于你的职责 | 不属于你的直接职责 | 你需要交接给谁 |
|---|---|---|
| Blender 模型整理、场景搭建、比例修正 | Unity 状态机代码 | 陈泓森 |
| Runtime FBX 导出、命名、材质归并 | Android 打包、真机安装 | 严辰乐 |
| Render 分支相机、灯光、截图、海报素材 | URP Asset/Renderer/Volume 实装 | 陈泓森 |
| 小镇视觉构图、主视觉统一、低模风格把关 | LLM API 调用 | 吴若琪 |
| 资产清单、模型质量审计、Unity 导入说明 | PPT 排版和视频剪辑 | 傅钧漪 |

你的最终交付必须能回答四个问题：

1. Unity 能不能直接导入并跑起来。
2. 宣传能不能直接拿图做 PPT、海报、视频。
3. 评委第一眼能不能看出 CatLife 是完整、统一、治愈的低多边形小镇。
4. 资产出了问题时，能不能追溯到 Blender 对象、材质和导出版本。

## 2. 当前工程基线

现有 Blender 双轨文件已建立：

```text
03-3d-models/blender-work/CatLife_master.blend
03-3d-models/blender-work/CatLife_runtime.blend
03-3d-models/blender-work/CatLife_render.blend
03-3d-models/blender-work/exports/CatLife_runtime.fbx
```

当前 Runtime 统计：

| 指标 | 当前值 | 结论 |
|---|---:|---|
| Meshes | 165 | 可控 |
| Materials | 161 | 偏碎，需归并 |
| Remaining modifiers | 0 | 已通过 |
| Polygons | 516,410 | 视图口径约 51 万面 |
| Triangles | 875,306 | Android 偏高 |
| Vertices | 722,458 | Android 偏高 |
| Non-unit scale objects | 0 | 已通过 |
| Non-zero rotation objects | 0 | 已通过 |

注意：当前不是“重新从零建模”，而是在已有 CatLife 小镇基础上做**可交付化整理**。重点是减风险、定版本、补构图、补渲染素材、配合 Unity 验证。

## 3. 总体目标

### 3.1 Runtime 目标

Runtime 资产用于 Unity/Android，优先级是稳定、轻量、可导入。

| 项 | 目标 |
|---|---|
| 文件 | `CatLife_runtime.blend` + `CatLife_runtime.fbx` |
| 命名 | 全部关键对象英文 `CL_` 前缀，无中文、无空格 |
| Transform | scale = 1，rotation = 0，单位为米 |
| 修改器 | Runtime 分支 0 modifiers |
| 材质 | 尽量归并，目标不超过 80，理想不超过 60 |
| 三角面 | 当前 875k，短期不再盲目全局 decimate；Unity 验证后定向减面 |
| 导出 | `Forward = -Z`，`Up = Y`，Apply Unit Scale |
| 交接 | 必须附 `asset_manifest.csv` 与 `unity_import_notes.md` |

### 3.2 Render 目标

Render 资产用于 PPT/海报/视频，优先级是视觉完成度和镜头好看。

| 项 | 目标 |
|---|---|
| 文件 | `CatLife_render.blend` |
| 主视觉 | 奶油蓝天静态午后专注花园 |
| 灯光 | 暖主光 + 柔补光 + 顶部柔光 |
| 相机 | 至少 5 个可复用相机 |
| 输出图 | 全景、竖版海报、猫特写、四状态对比 |
| 分辨率 | PPT 1920x1080，海报至少 2160x3840，透明素材 1080p+ |

## 4. 目录与版本规范

### 4.1 工作目录

```text
03-3d-models/blender-work/
  CatLife_master.blend
  CatLife_runtime.blend
  CatLife_render.blend
  exports/
    CatLife_runtime.fbx
    CatLife_buildings.fbx
    CatLife_props.fbx
    CatLife_environment.fbx
    CatLife_cat_character.fbx
  reports/
    asset_manifest.csv
    unity_import_notes.md
    blender_quality_gate_report.md
    render_shot_list.md

06-deliverables/render-stills/
  CL_Render_01_Town_45.png
  CL_Render_02_Poster_Vertical.png
  CL_Render_03_Cat_Closeup.png
  CL_Render_04_State_Normal.png
  CL_Render_05_State_Transition.png
  CL_Render_06_State_Focus.png
  CL_Render_07_State_Reward.png
```

### 4.2 文件命名

| 类型 | 命名 |
|---|---|
| 建筑 | `CL_BLD_FocusHouse_01` |
| 道路/广场 | `CL_ROAD_PlazaRing_01` |
| 环境 | `CL_ENV_TreePine_01` |
| 道具 | `CL_PROP_Bench_01` |
| 猫主角 | `CL_CAT_Body`、`CL_CAT_Head`、`CL_CAT_Full` |
| 灯光 | `LGT_Sun_Key`、`LGT_Area_Fill` |
| 相机 | `CAM_45`、`CAM_Poster_Vertical` |
| 材质 | `MAT_Roof_Orange`、`MAT_Grass_SoftGreen` |

禁止：

- `node_0.017` 作为最终关键对象名。
- 中文对象名进入 Runtime 导出。
- 空格、特殊符号进入 FBX 对象名。
- 同一材质颜色重复创建十几个材质。

## 5. 任务拆解

## 5.1 P0：Unity 可导入 Runtime 交接

### P0-1 固定 Runtime 基线

目标：保证 Unity 同学拿到的 FBX 是稳定版本，不再每天变化。

操作：

1. 打开 `CatLife_runtime.blend`。
2. 确认 `EXPORT_RT` collection 内包含全部 Runtime 对象。
3. 确认无隐藏但应导出的关键物体。
4. 导出 `exports/CatLife_runtime.fbx`。
5. 生成 `reports/asset_manifest.csv`。

质量门禁：

| 检查项 | 通过标准 |
|---|---|
| 文件存在 | `CatLife_runtime.blend` 与 `CatLife_runtime.fbx` 均存在 |
| 修改器 | Runtime 对象剩余 modifiers = 0 |
| Transform | 非单位缩放对象 = 0，非零旋转对象 = 0 |
| 命名 | 关键对象 100% `CL_` 前缀 |
| 导出大小 | FBX 建议 < 60MB，超过需说明 |
| Unity 导入 | 陈泓森确认场景可见、比例正确、材质不全丢 |

### P0-2 关键对象语义重命名

目标：让 Unity、PPT、答辩材料能读懂资产。

最低必须命名的对象：

| 类别 | 对象 |
|---|---|
| 主入口 | 城门、CATLIFE 标志 |
| 中心 | 圆形猫爪广场、主路环 |
| 建筑 | 专注小屋、番茄钟塔、猫窝休息屋、鱼干商店 |
| 视觉锚点 | 星星奖励树、猫头屋 |
| 交互道具 | 长椅、路灯、猫碗/猫粮、桌椅、伞 |
| 环境 | 主要树组、栅栏、花丛、石头 |

质量门禁：

| 检查项 | 通过标准 |
|---|---|
| 语义对象数 | 至少 25 个关键对象有语义英文名 |
| 追溯 | 保留 `source_name` custom property |
| 分类 | 每个关键对象归入 BLD/ROAD/ENV/PROP/CAT 中一种 |

### P0-3 材质归并第一轮

目标：减少 Unity 材质碎片，避免移动端 SetPass 风险继续扩大。

优先归并材质：

| 材质组 | 目标材质 |
|---|---|
| 屋顶暖橙 | `MAT_Roof_Orange` |
| 奶白墙体 | `MAT_Wall_Cream` |
| 草地柔绿 | `MAT_Grass_SoftGreen` |
| 树叶浅绿 | `MAT_Leaf_LightGreen` |
| 树叶深绿 | `MAT_Leaf_DarkGreen` |
| 木材暖棕 | `MAT_Wood_WarmBrown` |
| 石路奶白 | `MAT_Stone_Cream` |
| 暗金属 | `MAT_Metal_Dark` |
| 暖灯发光 | `MAT_Emission_WarmLight` |
| 星星金 | `MAT_Star_Gold` |

质量门禁：

| 检查项 | 通过标准 |
|---|---|
| 材质总数 P0 | <= 100 |
| 材质总数 P1 | <= 80 |
| 材质命名 | 归并材质全部 `MAT_` 前缀 |
| 风格 | 不使用写实 PBR 贴图堆叠 |

## 5.2 P1：小镇场景构图与表现

### P1-1 小镇第一眼构图

目标：评委第一眼能看到“猫咪专注小镇”，而不是一堆散模型。

构图规则：

1. 中心圆形猫爪广场是视觉中心。
2. 番茄钟塔、猫窝/猫屋、星星奖励树构成三角锚点。
3. 城门或 CATLIFE 标志必须有一个镜头能完整看见。
4. 道路引导视线进入广场，不能被建筑切断。
5. 左右边缘建筑避免在主宣传镜头里被严重裁切。

质量门禁：

| 镜头 | 通过标准 |
|---|---|
| `CAM_45` | 广场完整、猫屋/星星树/建筑三角清晰 |
| `CAM_Poster_Vertical` | 上方留标题空间，下方留团队/二维码空间 |
| `CAM_Cat_Closeup` | 猫/猫屋主体清楚，背景不乱 |
| `CAM_Top` | 可用于解释场景布局 |

### P1-2 奶油蓝天静态午后风格对齐

Blender Render 分支用于输出宣传图，需先把灰棚感修掉。

Render 分支设置：

| 项 | 建议 |
|---|---|
| World Color | 浅蓝 `#A8D6FF` 到奶油白方向 |
| Key Light | 暖色 `#FFE2B8`，左前上方 |
| Fill Light | 低强度冷暖中性补光 |
| Shadow | 柔和，但保留接触感 |
| 背景 | 不用灰背景；必要时加简单低模云/远树线 |

质量门禁：

| 检查项 | 通过标准 |
|---|---|
| 灰棚感 | 主渲染不出现默认灰背景 |
| 过曝 | 奶白墙体不过曝丢细节 |
| 暗部 | 猫屋入口、树下不过度死黑 |
| 风格 | 不出现赛博、暗黑、写实 HDRI 风格 |

### P1-3 海报/视频渲染素材

必须输出：

| 文件 | 规格 | 用途 |
|---|---|---|
| `CL_Render_01_Town_45.png` | 1920x1080 | PPT 主图 |
| `CL_Render_02_Poster_Vertical.png` | >=2160x3840 | 海报主图 |
| `CL_Render_03_Cat_Closeup.png` | 1920x1080 或透明 PNG | 封面/视频片头 |
| `CL_Render_04_State_Normal.png` | 1920x1080 | 四状态说明 |
| `CL_Render_05_State_Transition.png` | 1920x1080 | 四状态说明 |
| `CL_Render_06_State_Focus.png` | 1920x1080 | 四状态说明 |
| `CL_Render_07_State_Reward.png` | 1920x1080 | 四状态说明 |

质量门禁：

| 检查项 | 通过标准 |
|---|---|
| 清晰度 | 文字/招牌/主体无明显糊 |
| 构图 | 主体不被裁掉 |
| 色彩 | 暖橙、奶白、柔绿统一 |
| 文件 | PNG 命名规范，放入 `06-deliverables/render-stills/` |

## 5.3 P2：定向减面与 Unity 联调

### P2-1 不做全局盲目 Decimate

当前 Runtime 875k triangles 偏高，但禁止继续对全场一键 Decimate。原因：

1. 已经有 164 个 Decimate 被烘焙。
2. 再全局压缩容易破坏猫脸、屋顶、石路、星星树等视觉锚点。
3. 真正影响移动端的是可见三角、材质、批次、透明 overdraw，不只是 raw triangles。

正确做法：

1. 先让 Unity 导入 Runtime FBX。
2. 获取 Main Camera visible triangles、batches、SetPass、FPS。
3. 按 Unity 实测 top offenders 回到 Blender 定向优化。

质量门禁：

| 阶段 | 通过标准 |
|---|---|
| Blender 当前 | Runtime <= 900k triangles，禁止继续劣化造型 |
| Unity 首测 | 默认镜头 visible triangles 目标 <= 500k |
| Unity 优化后 | 默认镜头 visible triangles 目标 <= 350k |
| 材质 | Runtime 材质 <= 80 |

### P2-2 可定向优化对象

优先优化：

| 类型 | 方法 |
|---|---|
| 远处树组 | 合并、减面、降低叶片层级 |
| 重复花草 | 合并 mesh，减少独立对象 |
| 石路小块 | 合并对象，保持外观不变 |
| 小道具背面 | 删除不可见面 |
| 建筑内部 | 删除镜头不可见内面 |
| 栅栏重复件 | 合并或实例化交给 Unity |

不优先动：

| 类型 | 原因 |
|---|---|
| 猫主角脸部 | 影响情绪表达 |
| CATLIFE 标志 | 影响品牌识别 |
| 中心猫爪广场 | 影响主视觉 |
| 星星奖励树正面 | 影响奖励机制表达 |

## 5.4 P3：展示增强但不进 Runtime

这些只进入 `CatLife_render.blend`，不进入 Runtime FBX：

| 增强 | 用途 |
|---|---|
| 额外 Rim Light | 海报和猫特写 |
| 更高质量天空/云 | PPT/视频截图 |
| 更多星点奖励效果 | 奖励态展示图 |
| 更高阴影质量 | 海报渲染 |
| 透明背景猫特写 | PPT 封面/角标 |

质量门禁：

| 检查项 | 通过标准 |
|---|---|
| 分支隔离 | Render 专用增强不进入 Runtime 导出 |
| 命名 | `SHOW_` 或 `RENDER_` 前缀标明展示专用 |
| 交接 | 给宣传同学说明哪些图可直接用 |

## 6. 四状态 Blender 侧表现

Unity 会做真正的状态切换，但 Blender 需要给 PPT/视频提供四状态参考图。

| 状态 | Blender 表现 | 不做 |
|---|---|---|
| 普通 | 猫活跃、场景明亮、环境粒子最少 | 不做庆祝星爆 |
| 过渡 | 猫靠近/注视，整体更安静 | 不突然变暗 |
| 专注 | 猫安静趴下/陪伴，星点少而慢 | 不增加大量粒子 |
| 奖励 | 猫庆祝，星星树/猫爪粒子短时增强 | 不烟花化、不强闪白 |

Blender 输出用途：

1. PPT 解释四状态机制。
2. 视频脚本分镜。
3. Unity 美术参考。

## 7. 质量门禁总表

### 7.1 P0 门禁：不能交付失败

| 编号 | 检查项 | 通过标准 | 证据文件 |
|---|---|---|---|
| B-P0-01 | Runtime 文件存在 | `.blend` + `.fbx` 均存在 | `exports/CatLife_runtime.fbx` |
| B-P0-02 | Transform 冻结 | 非单位缩放=0，非零旋转=0 | `blender_quality_gate_report.md` |
| B-P0-03 | 修改器清空 | Runtime modifiers=0 | `blender_quality_gate_report.md` |
| B-P0-04 | Unity 导入 | 场景可见，比例正确 | `unity_import_notes.md` |
| B-P0-05 | 关键对象命名 | 至少 25 个语义命名对象 | `asset_manifest.csv` |
| B-P0-06 | 灰棚感修复 | Render 主图有天空/环境色 | `CL_Render_01_Town_45.png` |

### 7.2 P1 门禁：影响复赛观感

| 编号 | 检查项 | 通过标准 | 证据文件 |
|---|---|---|---|
| B-P1-01 | 材质归并 | 材质 <= 80 | `blender_quality_gate_report.md` |
| B-P1-02 | 主视觉构图 | 3 个核心锚点清晰 | `render_shot_list.md` |
| B-P1-03 | PPT 主图 | 1920x1080，主体完整 | `CL_Render_01_Town_45.png` |
| B-P1-04 | 海报主图 | 竖版 >=2160x3840 | `CL_Render_02_Poster_Vertical.png` |
| B-P1-05 | 四状态图 | 4 张图完整输出 | `CL_Render_04-07_*.png` |

### 7.3 P2 门禁：移动端性能风险

| 编号 | 检查项 | 通过标准 | 证据文件 |
|---|---|---|---|
| B-P2-01 | Runtime triangles | Blender 当前 <=900k，Unity 优化目标 <=350k visible | Unity Stats 截图 |
| B-P2-02 | 小物件合并 | 重复花草/石块/栅栏不碎成过多对象 | `asset_manifest.csv` |
| B-P2-03 | 材质命名 | `MAT_` 统一，重复色归并 | `asset_manifest.csv` |
| B-P2-04 | 分支隔离 | Render 专用灯光/云/粒子不进 Runtime | `blender_quality_gate_report.md` |

### 7.4 P3 门禁：打磨项

| 编号 | 检查项 | 通过标准 |
|---|---|---|
| B-P3-01 | 透明猫素材 | 可用于 PPT 封面 |
| B-P3-02 | 视频分镜角度 | 至少 3 个推荐镜头 |
| B-P3-03 | 海报留白 | 竖版构图上方和下方有文字安全区 |
| B-P3-04 | 归档说明 | 所有输出图、FBX、报告路径清楚 |

## 8. 每日执行顺序

### Day 1：Runtime 交接稳定

1. 打开 `CatLife_runtime.blend`。
2. 检查 `EXPORT_RT`。
3. 语义重命名 25 个关键对象。
4. 导出 `CatLife_runtime.fbx`。
5. 生成 `asset_manifest.csv`。

交付：Unity 可导入 FBX + manifest。

### Day 2：材质归并与风格统一

1. 合并屋顶、墙体、草地、树叶、木材、石路等主材质。
2. 控制材质数到 <=100，争取 <=80。
3. 输出材质说明。

交付：材质清单 + 新 Runtime FBX。

### Day 3：Render 分支主视觉

1. 打开 `CatLife_render.blend`。
2. 调整 World/天空/灯光，修掉灰棚感。
3. 调整 `CAM_45` 构图。
4. 输出 PPT 主图。

交付：`CL_Render_01_Town_45.png`。

### Day 4：海报与猫特写

1. 调整 `CAM_Poster_Vertical`。
2. 调整 `CAM_Cat_Closeup`。
3. 输出海报竖图和猫特写。

交付：海报主图 + 猫特写。

### Day 5：四状态参考图

1. 复制场景或使用隐藏集合切换四状态。
2. 输出普通、过渡、专注、奖励四张图。
3. 写 `render_shot_list.md`。

交付：四状态图 + 镜头说明。

### Day 6：Unity 反馈修复

1. 根据 Unity 导入反馈修复比例、法线、材质、缺失对象。
2. 若 Unity 统计超标，定向优化 top offenders。
3. 重新导出 FBX。

交付：修正版 FBX + `unity_import_notes.md`。

### Day 7：最终归档

1. 运行 Blender 质量检查。
2. 补齐 `blender_quality_gate_report.md`。
3. 将渲染图放入 `06-deliverables/render-stills/`。
4. 将报告放入 `03-3d-models/blender-work/reports/`。

交付：最终可交接包。

## 9. 交接模板

### 9.1 给 Unity 的交接说明

```text
文件：03-3d-models/blender-work/exports/CatLife_runtime.fbx
单位：1 Blender unit = 1 Unity meter
Forward：-Z
Up：Y
材质：纯色低模材质，已归并
注意：Render 分支灯光/相机/展示粒子不在 Runtime FBX 内
需要 Unity 验证：
- imported triangles
- materials count
- main camera visible triangles
- batches
- SetPass calls
- FPS on Android
```

### 9.2 给宣传的交接说明

```text
主图：06-deliverables/render-stills/CL_Render_01_Town_45.png
海报：06-deliverables/render-stills/CL_Render_02_Poster_Vertical.png
猫特写：06-deliverables/render-stills/CL_Render_03_Cat_Closeup.png
四状态：CL_Render_04-07_*.png
使用建议：
- PPT 封面优先用 Town_45 或 Cat_Closeup
- 海报优先用 Poster_Vertical
- 四状态页使用 04-07 组图
```

## 10. 完成定义

只有同时满足以下条件，Blender 侧才算完成：

1. Unity 同学确认 Runtime FBX 能导入、能显示、比例正确。
2. 宣传同学拿到至少 7 张可直接使用的渲染图。
3. `asset_manifest.csv`、`unity_import_notes.md`、`blender_quality_gate_report.md` 三份报告齐全。
4. Runtime 与 Render 分支隔离清楚。
5. 所有关键对象、材质、相机、灯光命名规范。
6. 没有把展示专用灯光/粒子/高质量渲染内容混进 Runtime。
