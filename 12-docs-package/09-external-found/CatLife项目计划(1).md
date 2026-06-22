# AIGC 比赛 - Cat Life 项目计划

## 四周工作安排

1. **Week 1**：搭建 Unity 工程，导入 catlife.glb 模型与场景资源
2. **Week 2**：实现四状态机（待机/活动/进食/睡眠），配置 Animator 动画
3. **Week 3**：开发轻锁定 UI，集成行为识别模块，接入 LLM
4. **Week 4**：联调测试，打包发布，录制演示视频

## 技术要点

- 状态机：Animator 优先，Transform 兜底
- LLM 调用前加本地缓冲判断，减少 API 消耗
- 行为数据本地聚合预判后再请求 LLM
- UI 参考现有 Demo，不重做
- 用 Timeline 做自动演示序列