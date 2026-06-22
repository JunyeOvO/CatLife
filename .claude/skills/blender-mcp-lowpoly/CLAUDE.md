# Blender MCP Low-Poly 场景搭建 Skill

> 适用项目：05-AIGC CatLife
> BlenderMCP: ahujasid/blender-mcp v1.5.5
> 触发：用户在 Blender 中搭建 low-poly 场景、3D 资产处理、场景组装

---

## 架构

```
Claude Code  ←→  uvx blender-mcp (MCP Server)  ←TCP:9876→  Blender Addon
```

MCP 已在 Claude Code 注册：`claude mcp add blender uvx blender-mcp`
配置文件：`C:\Users\fujunye\.claude.json` (project-scoped to 05-AIGC)

---

## 核心工作流：分层迭代法（黄金法则）

**一次只做一个操作，做完验证再继续。**

```
检查场景 → 规划一步 → 执行 → 截图验证 → 通过/回退 → 下一步
```

| 轮次 | 焦点 | 示例 prompt |
|------|------|------------|
| 1 | 整体布局 | "用简单的立方体搭建场景轮廓布局" |
| 2 | 细化几何 | "把方块替换成低多边形造型，面数控制在300以内" |
| 3 | 材质/颜色 | "给所有物体上平涂色：暖色泥土调，木头上棕色，屋顶暗红" |
| 4 | 环境资产 | "搜索Poly Haven添加低面数树木、石头、围栏" |
| 5 | 灯光/相机 | "设置等距视角摄像机 + 一个日光 + 柔和环境光" |

### 资产来源优先级

```
外部低面数资产库 (Kenney/pmnd.rs) → Poly Haven/Sketchfab → Hyper3D/Hunyuan3D AI生成 → 纯Python程序化构建
```

### 场景清理脚本（切换场景前必执行）

```python
import bpy
for obj in list(bpy.context.scene.objects):
    bpy.data.objects.remove(obj, do_unlink=True)
bpy.ops.outliner.orphans_purge(do_local_ids=True, do_linked_ids=False, do_recursive=True)
print(f"Objects: {len(bpy.data.objects)}, Actions: {len(bpy.data.actions)}, Armatures: {len(bpy.data.armatures)}, Meshes: {len(bpy.data.meshes)}")
```

---

## 能力边界

### 能做好
- 场景搭建（基础几何体、布局、对位、缩放）
- 材质应用（Principled BSDF、平涂色、金属度/粗糙度）
- 批量操作（重命名、统一材质、批量导出 FBX/GLB）
- 场景调试（找丢失文件、错位物体、链接库问题）
- 灯光 + 相机（日光、区域光、等距视角、HDRI 环境光）
- Python 脚本执行 + 自动纠错（错误信息回传 Claude 自动修复）
- Low-poly 特定：装饰器 + 阵列修改器批量生成栅栏/屋顶瓦片

### 做不好
- 有机建模（脸、树、角色——Sculpt 模式无法通过 MCP 访问）
- 干净拓扑（边循环、极点管理、细分就绪几何体）
- 角色绑定（IK 链、权重绘制——自动权重在薄几何体上失败）
- Geometry Nodes（API 版本脆弱，训练数据常与你的 Blender 版本不匹配）
- 复杂节点树（超过几个节点后错误复合增长）
- 动画（基本关键帧可以，角色动画不行）
- 生产级输出（这是快速原型工具，不是 3D 艺术家替代品）

---

## 关键坑与解法

| # | 症状 | 原因 | 解决 |
|---|------|------|------|
| 1 | `poll() failed, context incorrect` | 错误的 Blender 模式 | 操作前先设 `bpy.context.view_layer.objects.active = obj` + 正确模式 |
| 2 | 整个 mesh 跟着一根骨头变形 | armature 的 `use_connect=True` | 设 `bone.use_connect = False`，手动 parent |
| 3 | 材质变成洋红色 | 上下文窗口满了，Claude 丢失场景状态 | 保存 .blend checkpoint → 新会话加载 |
| 4 | `rotation_euler` 无效 | Armature 在 Quaternion 模式 | 设 `bone.rotation_mode = 'XYZ'` |
| 5 | 截图显示成功但实际错误 | 视角不对 | 程序化设相机到 `RIGHT`/`FRONT`，截图前 `bpy.ops.wm.redraw_timer(type='DRAW_WIN_SWAP', iterations=1)` |
| 6 | 第一个命令发不过去 | 连接初始化延迟 | 再试一次通常就通 |
| 7 | 外部资产导入后比例错乱 | 未检查 bounding box | 导入后检查 `obj.dimensions` 并缩放 |
| 8 | 导出的 GLB 有上一场景的孤儿数据 | Orphan 数据未 purge | 执行场景清理脚本 |
| 9 | 自动权重在薄几何体上失败 | 所有骨骼对所有顶点都"近" | 手动高斯权重分配 `sigma=0.03` |
| 10 | 面数爆炸 | AI 不知道加了 subdivision modifier | 每次迭代后 `print(len(obj.data.polygons))` 检查面数 |

---

## 可用命令速查

| 命令类型 | 用途 |
|----------|------|
| `get_scene_info` | 获取场景中所有对象、数量、位置 |
| `get_object_info` | 获取特定对象的详细信息 |
| `get_viewport_screenshot` | 获取当前视口截图（用于视觉验证） |
| `execute_code` | 执行任意 Blender Python 代码 |
| `get_polyhaven_status` | 检查 Poly Haven 连接状态 |
| `search_polyhaven_assets` | 搜索 Poly Haven 资产 |
| `download_polyhaven_asset` | 下载 Poly Haven 资产到场景 |
| `get_hyper3d_status` / `create_rodin_job` | Hyper3D Rodin AI 生成 |
| `get_hunyuan3d_status` | Hunyuan3D AI 生成 |

---

## CatLife 项目特定策略

项目已有 36 个 Meshy AI `.glb` 模型 + 82 张参考图（`03-3d-models/catlife-town/`）。

### Blender MCP 适用场景
1. **场景组装**：导入 36 个 .glb，摆放到正确位置，用自然语言对位
2. **材质统一**：批量调整成 low-poly 平涂风格 → "把所有建筑的材质改成 flat color，去掉金属度，粗糙度拉满"
3. **灯光优化**：设置移动端友好的简洁灯光（单日光 + 低强度环境光，避免多光源性能问题）
4. **批量导出**：导出为 Unity 可用的 FBX/GLB
5. **补充资产**：用参考图 + AI 生成补充缺失的低面数模型

### 导出到 Unity 注意事项
- 导出前确认 Scale: `bpy.ops.export_scene.fbx(use_selection=True, axis_forward='-Z', axis_up='Y')`
- Low poly 移动端目标：单个资产 < 500 三角面
- 材质烘焙为单张纹理（移动端 Shader 兼容性）

### 工作节奏
- 每完成一个场景模块（小镇主街、猫屋内部、庭院等）→ 保存 `.blend` checkpoint
- 开新会话加载 checkpoint 继续下一个模块（避免上下文退化）
- 用 MiMo Vision skill 做截图视觉验证

---

## 安全提醒

- `execute_code` 以用户权限运行任意 Python——**执行前先保存 .blend**
- 可能删除文件、读取本地路径、修改插件、访问网络
- 正式操作前在测试场景验证代码

---

## 参考来源

- [BlenderMCP GitHub](https://github.com/ahujasid/blender-mcp)
- [Asset Creation Strategy (DeepWiki)](https://deepwiki.com/ahujasid/blender-mcp/4.4-asset-creation-strategy)
- [Claude + Blender MCP 真实性能测试](https://www.mindstudio.ai/blog/claude-blender-mcp-real-world-performance)
- [甜甜圈 Token 消耗实测](https://www.mindstudio.ai/blog/claude-blender-mcp-60-percent-tokens-donut-test-results)
- [Planner–Actor–Critic 框架 (MIT)](https://arxiv.org/html/2601.05016v1)
- [Blender Artists 社区讨论](https://blenderartists.org/t/from-blender-mcp-to-3d-agent-anthropic-partners-with-blender-claude-ai-connector-now-official/1639106/573)
- [3D 小白亲测踩坑指南](https://developer.volcengine.com/articles/7517866334034591755)
- [Anthropic Blender Connector 开发者体验](https://dev.classmethod.jp/en/articles/claude-blender-connector-desktop-and-code/)
