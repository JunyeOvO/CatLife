# FocusScene 场景搭建指南

FocusScene.unity 需要在 Unity Editor 中手动创建，步骤如下：

---

## 1. 创建场景

1. Unity Editor → File → New Scene
2. 保存为 `Assets/Scenes/FocusScene.unity`

---

## 2. Canvas 设置

1. 右键 Canvas → UI → Canvas
2. Render Mode: **Screen Space Overlay**
3. Canvas Scaler:
   - Dynamic Pixels Per UIP: 1
   - Reference Resolution: 1080 x 1920（竖屏手机）
   - Screen Match Mode: Match Width Or Height

---

## 3. 背景

1. 右键 Canvas → UI → Image（作为背景）
2. Anchor: Stretch Full
3. Color: #1A1A2E（深蓝黑色）
4. 添加渐变或背景图（可选）

---

## 4. 专注状态文字

1. 右键 Canvas → UI → Text - TextMeshPro
2. Text: "专注中..."
3. Font Size: 48
4. Color: #FFFFFF
5. Alignment: Center + Middle
6. Anchor: 中部居中

---

## 5. 计时器文字

1. 右键 Canvas → UI → Text - TextMeshPro
2. Text: "00:00"
3. Font Size: 72
4. Color: #FFFFFF
5. Anchor: 上方居中

---

## 6. 专注进度条

1. 右键 Canvas → UI → Slider
2. 设置为只读模式（交互关闭）
3. Fill Source: Fill Amount（通过脚本更新）

---

## 7. 上滑退出提示

1. 右键 Canvas → UI → Text - TextMeshPro
2. Text: "↑ 上滑退出专注"
3. Font Size: 24
4. Color: #888888
5. Anchor: 底部居中
6. 添加悬浮动画（挂载 UnlockBarAnimation.cs）

---

## 8. 脚本挂载

在 Canvas 上依次添加组件：

### FocusUIManager
- Hide In Focus Objects: 主菜单按钮等（专注时隐藏）
- Focus Hint Object: "专注中..." 文字
- Swipe Up Hint Object: 上滑退出提示
- Focus Progress Bar: 进度条
- Swipe Threshold: 100

### FocusSceneController
- Status Text: 状态文字引用
- Timer Text: 计时器引用
- Focus Score Text: 专注度文字引用
- Progress Fill Image: 进度条 Fill Image

### BehaviorTracker（如果 Scene 中没有）
- 挂载到任意 GameObject
- Analysis Window: 30
- Activity Threshold: 0.3

### StateMachine
- Initial State: Focus（专注场景进入即为专注状态）
- Auto Transition: false

### SmartFocusAnalyzer
- LLM Client: （拖入对应 LLM Client）
- State Machine: 拖入 StateMachine 引用
- Use LLM Analysis: true

### GameManager（单例，场景只需一个）
- 如果 MainScene 已有可不加

---

## 9. 猫咪模型（可选）

专注场景可以放一只安静的猫咪模型：
- 播放 Rest 动画
- 位置：场景中下位置
- Scale: 适中大小

---

## 10. 导出设置

1. File → Build Settings → Add Open Scenes → FocusScene
2. 确保在 Scene List 中

---

## 快速检查清单

- [ ] Canvas (Screen Space Overlay)
- [ ] 背景 Image/Panel
- [ ] 状态文字 TextMeshPro
- [ ] 计时器 TextMeshPro
- [ ] 进度条 Slider
- [ ] 上滑退出提示 + UnlockBarAnimation
- [ ] FocusUIManager 组件
- [ ] FocusSceneController 组件
- [ ] StateMachine (Focus)
- [ ] BehaviorTracker
- [ ] SmartFocusAnalyzer
- [ ] LLM 客户端组件（BlueLLMClient 或 MockLLMClient）
