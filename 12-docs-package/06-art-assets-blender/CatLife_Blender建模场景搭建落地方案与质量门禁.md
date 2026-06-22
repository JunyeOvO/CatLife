# CatLife Blender 建模与场景搭建落地方案及质量门禁

同步来源：`10-art-guide/CatLife_Blender建模场景搭建落地方案与质量门禁.md`

本资料包副本用于交接阅读。后续维护请优先修改项目源文档，再同步到资料包。

核心范围：

- 负责人：傅钧烨。
- 只覆盖 Blender 建模、场景搭建、材质整理、Runtime/Render 双轨文件、Unity 交接、PPT/海报/视频渲染素材。
- 不覆盖 Unity URP 代码、Android 打包、大模型调用、PPT 排版。

核心质量门禁：

- Runtime 分支：0 modifiers，scale=1，rotation=0，关键对象英文 `CL_` 命名。
- Unity 交接：`CatLife_runtime.fbx` + `asset_manifest.csv` + `unity_import_notes.md`。
- 材质归并：P0 <=100，P1 <=80。
- Render 分支：输出 PPT 主图、海报竖图、猫特写、四状态参考图。
- 分支隔离：展示专用灯光/天空/粒子不进入 Runtime FBX。

完整方案见项目源文档：

```text
10-art-guide/CatLife_Blender建模场景搭建落地方案与质量门禁.md
```
