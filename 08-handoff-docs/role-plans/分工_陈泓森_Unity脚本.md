# 陈泓森 · Unity脚本 · 复赛专业计划

## 你的核心价值
你是让Cat Life"活起来"的人。Blender做的模型在你手里变成可以交互的App。

## 提交关联
傅钧烨资产 → 你搭建Unity场景+写脚本 → 严辰乐打包APK → 傅钧漪录视频

---

## Week 1（5/26-6/1）：工程搭建+场景导入

### Unity工程
```
Unity 2021.3 LTS / 2022.3 LTS
Platform: Android
Project: CatLife

Assets/
  _Scenes/MainScene.unity
  _Scripts/
    Core/          # 状态机、数据管理
    Cat/           # 猫咪行为控制
    UI/            # UI管理器
    LLM/           # 大模型调用（对接吴若琪的LLMClient）
  _Art/            # 傅钧烨提供
    Models/        # FBX导入
    Materials/     # 材质
    Animations/    # 动画Controller
  _Prefabs/
  _Resources/
```

### 场景搭建（对接傅钧烨的FBX）
1. 导入猫主角 + 建筑 + 道具
2. 设置地面（Plane + 草绿色材质）
3. 灯光：1 Directional Light（暖色）+ 环境光
4. 相机：Perspective，3/4俯视角度
5. Canvas：Screen Space Overlay（UI层）

### 猫咪基础脚本
```csharp
// CatController.cs
public class CatController : MonoBehaviour {
    public Animator animator;

    public void PlayIdle() => animator.SetTrigger("Idle");
    public void PlayApproach() => animator.SetTrigger("Approach");
    public void PlayRest() => animator.SetTrigger("Rest");
    public void PlayCelebrate() => animator.SetTrigger("Celebrate");

    // 简单随机动作（普通状态下）
    IEnumerator RandomIdleActions() {
        while (state == CatState.Normal) {
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            // 随机小动作：伸懒腰、眨眼、转头
        }
    }
}
```

---

## Week 2（6/2-6/8）：状态机+动画

### 四状态状态机
```csharp
public enum CatState { Normal, Transition, Focus, Reward }

public class StateMachine : MonoBehaviour {
    public CatState currentState = CatState.Normal;

    public void SwitchState(CatState newState) {
        currentState = newState;
        switch (newState) {
            case CatState.Normal:    OnEnterNormal();    break;
            case CatState.Transition: OnEnterTransition(); break;
            case CatState.Focus:     OnEnterFocus();     break;
            case CatState.Reward:    OnEnterReward();    break;
        }
    }
}
```

### 双入口专注
```csharp
// 入口1：手动点击"开始专注"
public void OnStartFocusClicked() {
    SwitchState(CatState.Focus);
    UIManager.Instance.ShowFocusUI();
}

// 入口2：自动识别（行为检测）
void Update() {
    if (currentState == CatState.Normal) {
        float activity = behaviorTracker.GetActivityRate(30f);
        if (activity < transitionThreshold) {
            SwitchState(CatState.Transition);
        }
    }
}
```

### 猫咪动画状态对应
| 状态 | 动画 | 循环 |
|------|------|------|
| Normal | Idle（含随机小动作） | ✓ |
| Transition | Approach（走近+注视） | ✓ |
| Focus | Rest（安静趴着） | ✓ |
| Reward | Celebrate（跳起+星星） | 1次 |

**如果没时间做骨骼动画，用Transform动画替代**：
- Idle: 小幅上下浮动
- Approach: 向镜头移动+放大
- Rest: 缩小+变半透明
- Celebrate: 旋转+弹跳

---

## Week 3（6/9-6/15）：轻锁定UI+行为识别

### 轻锁定UI
```csharp
public class FocusUIManager : MonoBehaviour {
    public GameObject[] hideInFocus;  // 专注时隐藏的按钮
    public GameObject swipeUpHint;     // "↑ 上滑退出"

    public void EnterFocus() {
        foreach (var obj in hideInFocus)
            obj.SetActive(false);
        swipeUpHint.SetActive(true);
        StartCoroutine(FocusTimer());
    }

    public void ExitFocus() {
        foreach (var obj in hideInFocus)
            obj.SetActive(true);
        swipeUpHint.SetActive(false);
    }

    // 上滑检测
    void Update() {
        if (Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Ended && t.deltaPosition.y > 100) {
                ExitFocus();
            }
        }
    }
}
```

### 行为数据采集
```csharp
public class BehaviorTracker : MonoBehaviour {
    public int clickCount30s;
    public int swipeCount30s;
    public float idleSeconds;

    void Update() {
        if (Input.GetMouseButtonDown(0)) clickCount30s++;
        // Touch滑动
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            swipeCount30s++;
        if (!Input.anyKey && Input.touchCount == 0)
            idleSeconds += Time.deltaTime;
    }

    public float GetActivityRate(float window) {
        return (clickCount30s + swipeCount30s) / window;
    }

    // 每30秒重置计数器
    IEnumerator ResetWindow() {
        while (true) {
            yield return new WaitForSeconds(30f);
            clickCount30s = 0; swipeCount30s = 0;
        }
    }
}
```

### 对接吴若琪的LLMClient
```csharp
public class SmartFocusAnalyzer : MonoBehaviour {
    public LLMClient llmClient;  // 吴若琪提供

    public void AnalyzeWithLLM() {
        var data = new BehaviorData {
            clickCount = behaviorTracker.clickCount30s,
            swipeCount = behaviorTracker.swipeCount30s,
            idleSeconds = behaviorTracker.idleSeconds
        };
        llmClient.AnalyzeFocusState(data, (result) => {
            // 解析JSON，决定是否切换状态
            // {"state":"focus","score":0.87}
        });
    }
}
```

---

## Week 4（6/16-6/23）：联调+录屏素材

### 联调
- 与傅钧烨：确认所有资产在Unity中正确
- 与吴若琪：LLMClient集成测试
- 与严辰乐：Unity导出给Android集成
- 与傅钧漪：提供录屏用的场景配置

### 录屏素材准备
- 场景配置为最佳展示角度
- 预设演示流程：开屏→领养→活跃→点专注→安静→庆祝
- 隐藏调试UI
- 确保帧率流畅

---

## 脚本交付清单
```
unity_scripts/
  ├── StateMachine.cs       # 四状态机
  ├── CatController.cs      # 猫咪行为
  ├── BehaviorTracker.cs    # 行为采集
  ├── FocusUIManager.cs     # 轻锁定UI
  ├── SmartFocusAnalyzer.cs # LLM集成
  ├── DataManager.cs        # 本地存储
  └── UIManager.cs          # 页面管理
```
