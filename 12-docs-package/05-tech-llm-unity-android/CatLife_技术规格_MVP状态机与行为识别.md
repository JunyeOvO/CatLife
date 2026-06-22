# Cat Life 技术规格：MVP 状态机与行为识别

## 1. MVP 技术目标
MVP 不追求真实复杂 AI，而是先验证“行为状态识别 → 猫咪状态机 → 轻锁定界面”的闭环。第一版可用规则引擎实现，后续再替换为轻量本地模型或视觉识别模型。

## 2. 数据采集层
### 前台数据（App 已打开时）
- 点击频率：单位时间内点击次数。
- 滑动频率：单位时间内滑动次数。
- 互动类型：逗猫、切换场景、点击功能按钮等。
- 响应延迟：用户对提示或交互的反应时间。
- 停留时长：停留在主场景、设置页、专注页的时间。

### 授权后台数据（用户离开 App 使用其他软件时，可作为后续扩展）
- 切屏频率。
- 离开时长。
- 返回间隔。
- 跨应用触控节奏。

## 3. 特征计算层
使用同一套轻量级本地模型或规则引擎，根据场景调整输入特征权重和阈值。

基础统计：
- `click_count_window`: 最近 N 秒点击次数。
- `swipe_count_window`: 最近 N 秒滑动次数。
- `idle_duration`: 连续无操作时长。
- `session_active_duration`: 当前连续操作时长。
- `app_switch_count`: 切屏次数。
- `return_interval`: 离开后返回间隔。

建议状态评分：
```text
activity_score = w1 * click_rate + w2 * swipe_rate + w3 * app_switch_rate - w4 * idle_duration
focus_tendency = normalize(1 - activity_score)
```

## 4. 状态感知层
不要把状态写成简单的 0 / 0.5 / 1 离散切换，而应表达为连续注意状态谱：

```text
注意偏离 → 注意回收中 → 专注稳定
```

工程上仍可落为四个状态：

| 状态 | 触发条件 | 猫咪行为 | UI 反馈 |
|---|---|---|---|
| 普通状态 | 高频点击/滑动，未开始专注 | 活跃、可互动 | 展示完整按钮 |
| 过渡状态 | 操作频率下降，但未达到专注阈值 | 靠近、注视、慢动作 | 减少刺激，弱提示 |
| 专注状态 | 低频操作持续超过适应时间 | 安静趴下、弱存在感 | 轻锁定，隐藏按钮，上滑退出 |
| 奖励状态 | 完成一段专注 | 庆祝、成长反馈 | 显示记录与奖励 |

## 5. 状态机伪代码
```pseudo
state = NORMAL

on_every_tick:
    features = collect_recent_behavior(window = 30s)
    focus_score = calculate_focus_score(features)

    if state == NORMAL:
        if user_click_start_focus:
            enter_focus_state(reason = "manual")
        else if focus_score >= transition_threshold:
            state = TRANSITION
            cat.play("approach_and_watch")

    else if state == TRANSITION:
        if focus_score >= focus_threshold for adapt_time:
            enter_focus_state(reason = "auto")
        else if activity_score rises again:
            state = NORMAL
            cat.play("active_companion")

    else if state == FOCUS:
        hide_interaction_buttons()
        cat.play("quiet_rest")
        if user_swipe_up_exit:
            state = NORMAL
        if focus_session_completed:
            state = REWARD

    else if state == REWARD:
        cat.play("celebrate")
        update_growth_and_records()
        state = NORMAL
```

## 6. 推荐阈值（原型阶段）
| 参数 | 建议初值 | 说明 |
|---|---:|---|
| 时间窗 | 30 秒 | 用于统计近期操作频率 |
| 过渡阈值 | 操作频率下降 40% | 从高频娱乐转向低频准备 |
| 专注阈值 | 连续低频/无操作 | 低频状态持续后进入专注 |
| 适应时间 | 30 秒 / 1 分钟 / 2 分钟 | 设置页可选 |
| 退出方式 | 上滑退出 | 保留自主性，同时强化边界 |

## 7. 原型开发优先级
1. 先实现四状态状态机。
2. 再实现猫咪动画状态切换。
3. 再实现轻锁定 UI：隐藏按钮、弱化交互、上滑退出。
4. 再记录专注时长和猫咪成长值。
5. 最后再接更复杂的行为识别模型。

## 8. 技术边界
- 不要在初赛阶段把“AI 识别”吹成已经完成复杂视觉模型。
- 可以明确表述为：第一版用规则引擎验证闭环，后续可扩展到轻量本地模型和视觉特征识别。
- 权限设计应尽量克制，突出隐私友好和低成本实现。
