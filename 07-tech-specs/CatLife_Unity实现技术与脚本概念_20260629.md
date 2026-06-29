# CatLife Unity 实现技术与脚本概念

日期：2026-06-29
目标：说明实现 UI、状态机、行为识别、LLM 降级和比赛演示所需的具体技术与 Unity 简单脚本概念。

## 1. Unity 技术选型

| 模块 | 技术 |
|---|---|
| 3D 小镇 | Unity 6 + URP/Mobile URP |
| UI | uGUI + TextMesh Pro，必要时加 SafeArea 适配 |
| 猫动画 | Animator Controller + Animation Clips |
| 状态机 | C# enum + transition guard |
| 行为采集 | Unity Input + UI 事件埋点；Android 原生桥后续增强 |
| 数据存储 | PlayerPrefs MVP，正式可换 JSON 本地文件 |
| LLM | `ILLMClient` 接口 + `MockLLMClient` + 真实客户端 |
| 隐私网关 | 请求前字段白名单检查 |
| Android 构建 | Unity Build Settings + Editor build script |

## 2. 推荐目录

```text
Assets/CatLife/
  Scenes/
    CatLife_Main.unity
  Scripts/
    Core/
    Behavior/
    Cat/
    UI/
    LLM/
    Data/
    Debug/
  Prefabs/
    CatCompanion.prefab
    TownRoot.prefab
    UI/
  Materials/
  Configs/
```

## 3. 状态枚举

```csharp
public enum CatLifeState
{
    Normal = 0,
    Transition = 1,
    Focus = 2,
    Reward = 3
}
```

## 4. 行为分值结构

```csharp
public struct BehaviorScores
{
    public float Focus;
    public float Arousal;
    public float Distraction;
    public float Confidence;
    public float IdleSeconds;
}
```

## 5. 核心状态机概念

```csharp
using UnityEngine;
using System;

public class FocusStateMachine : MonoBehaviour
{
    public CatLifeState CurrentState { get; private set; } = CatLifeState.Normal;
    public event Action<CatLifeState, string> OnStateChanged;

    [SerializeField] private float minDwellSeconds = 3f;
    [SerializeField] private float transitionIdleSeconds = 8f;
    [SerializeField] private float focusConfirmSeconds = 30f;

    private float stateEnteredAt;
    private float focusCandidateTime;

    public void Tick(BehaviorScores scores, bool manualStart, bool sessionComplete, bool exitRequested)
    {
        float dwell = Time.time - stateEnteredAt;
        if (dwell < minDwellSeconds) return;

        switch (CurrentState)
        {
            case CatLifeState.Normal:
                if (manualStart || (scores.Focus >= 45f && scores.Arousal < 45f && scores.IdleSeconds >= transitionIdleSeconds))
                    SetState(CatLifeState.Transition, manualStart ? "manual_start" : "low_activity_detected");
                break;

            case CatLifeState.Transition:
                if (scores.Focus >= 70f && scores.Distraction < 35f)
                    focusCandidateTime += Time.deltaTime;
                else
                    focusCandidateTime = 0f;

                if (focusCandidateTime >= focusConfirmSeconds)
                    SetState(CatLifeState.Focus, "focus_confirmed");
                else if (scores.Arousal >= 60f || scores.Distraction >= 55f)
                    SetState(CatLifeState.Normal, "activity_recovered");
                break;

            case CatLifeState.Focus:
                if (sessionComplete)
                    SetState(CatLifeState.Reward, "session_completed");
                else if (exitRequested)
                    SetState(CatLifeState.Normal, "user_exit");
                break;

            case CatLifeState.Reward:
                break;
        }
    }

    public void FinishReward()
    {
        SetState(CatLifeState.Normal, "reward_finished");
    }

    private void SetState(CatLifeState next, string reason)
    {
        if (CurrentState == next) return;
        CurrentState = next;
        stateEnteredAt = Time.time;
        focusCandidateTime = 0f;
        OnStateChanged?.Invoke(next, reason);
    }
}
```

## 6. 猫动画驱动

Animator 参数：

| 参数 | 类型 | 用途 |
|---|---|---|
| `State` | int | 0 Normal, 1 Transition, 2 Focus, 3 Reward |
| `Focus` | float | 控制趴下/呼吸混合 |
| `Arousal` | float | 控制尾巴和动作速度 |
| `Distraction` | float | 控制抬头/提醒 |
| `Reward` | trigger | 播放奖励动作 |

```csharp
using UnityEngine;

public class CatAnimationDriver : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void ApplyState(CatLifeState state)
    {
        animator.SetInteger("State", (int)state);
        if (state == CatLifeState.Reward)
            animator.SetTrigger("Reward");
    }

    public void ApplyScores(BehaviorScores scores)
    {
        animator.SetFloat("Focus", scores.Focus / 100f);
        animator.SetFloat("Arousal", scores.Arousal / 100f);
        animator.SetFloat("Distraction", scores.Distraction / 100f);
    }
}
```

## 7. UI 状态控制

```csharp
using UnityEngine;

public class CatLifeUIStateView : MonoBehaviour
{
    [SerializeField] private GameObject mainActions;
    [SerializeField] private GameObject focusOverlay;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMPro.TMP_Text catBubble;

    public void ApplyState(CatLifeState state)
    {
        mainActions.SetActive(state == CatLifeState.Normal || state == CatLifeState.Transition);
        focusOverlay.SetActive(state == CatLifeState.Focus);
        rewardPanel.SetActive(state == CatLifeState.Reward);

        catBubble.text = state switch
        {
            CatLifeState.Normal => "先不用急，我在这里。",
            CatLifeState.Transition => "你慢下来了，我也安静一点。",
            CatLifeState.Focus => "我会轻轻陪着你，不打扰。",
            CatLifeState.Reward => "完成啦，猫咪给你一个小爪印。",
            _ => ""
        };
    }
}
```

## 8. 行为事件和评分器 MVP

```csharp
public enum InteractionType
{
    Tap,
    Swipe,
    PageSwitch,
    FocusStart,
    FocusEnd
}

public struct InteractionEvent
{
    public float Time;
    public InteractionType Type;
    public string RouteId;
}
```

```csharp
using System.Collections.Generic;
using UnityEngine;

public class SimpleBehaviorScoreEngine : MonoBehaviour
{
    private readonly List<InteractionEvent> events = new();
    private float lastInteractionTime;

    public void Track(InteractionType type, string routeId)
    {
        events.Add(new InteractionEvent { Time = Time.time, Type = type, RouteId = routeId });
        lastInteractionTime = Time.time;
    }

    public BehaviorScores Calculate()
    {
        float now = Time.time;
        events.RemoveAll(e => now - e.Time > 30f);

        int taps = events.FindAll(e => e.Type == InteractionType.Tap).Count;
        int swipes = events.FindAll(e => e.Type == InteractionType.Swipe).Count;
        int switches = events.FindAll(e => e.Type == InteractionType.PageSwitch).Count;
        float idle = now - lastInteractionTime;

        float arousal = Mathf.Clamp01((taps * 0.08f) + (swipes * 0.12f)) * 100f;
        float distraction = Mathf.Clamp01(switches * 0.25f) * 100f;
        float focus = Mathf.Clamp01((idle / 30f) - (arousal / 200f) - (distraction / 200f)) * 100f;

        return new BehaviorScores
        {
            Focus = focus,
            Arousal = arousal,
            Distraction = distraction,
            Confidence = events.Count >= 3 ? 1f : 0.4f,
            IdleSeconds = idle
        };
    }
}
```

## 9. LLM 接口与降级

```csharp
public interface ILLMClient
{
    System.Threading.Tasks.Task<string> ExplainAsync(BehaviorScores scores, CatLifeState state);
}

public class MockLLMClient : ILLMClient
{
    public System.Threading.Tasks.Task<string> ExplainAsync(BehaviorScores scores, CatLifeState state)
    {
        string text = state == CatLifeState.Focus
            ? "刚才你的节奏很稳，可以继续保持。"
            : "猫咪会根据你的操作节奏慢慢调整陪伴方式。";
        return System.Threading.Tasks.Task.FromResult(text);
    }
}
```

```csharp
public class PrivacyGateway
{
    public bool CanSendToCloud(bool userConsent, bool rawTextIncluded, bool rawTouchPathIncluded, bool screenContentIncluded)
    {
        return userConsent
            && !rawTextIncluded
            && !rawTouchPathIncluded
            && !screenContentIncluded;
    }
}
```

## 10. 主控拼装概念

```csharp
using UnityEngine;

public class CatLifeAppController : MonoBehaviour
{
    [SerializeField] private FocusStateMachine stateMachine;
    [SerializeField] private SimpleBehaviorScoreEngine scoreEngine;
    [SerializeField] private CatAnimationDriver catDriver;
    [SerializeField] private CatLifeUIStateView uiView;

    private bool manualStart;
    private bool exitRequested;
    private bool sessionComplete;

    private void Awake()
    {
        stateMachine.OnStateChanged += (state, reason) =>
        {
            catDriver.ApplyState(state);
            uiView.ApplyState(state);
            Debug.Log($"CatLife state changed: {state}, reason={reason}");
        };
    }

    private void Update()
    {
        var scores = scoreEngine.Calculate();
        catDriver.ApplyScores(scores);
        stateMachine.Tick(scores, manualStart, sessionComplete, exitRequested);
        manualStart = false;
        exitRequested = false;
    }

    public void OnStartFocusClicked()
    {
        scoreEngine.Track(InteractionType.FocusStart, "main_town");
        manualStart = true;
    }

    public void OnExitFocusSwipe()
    {
        exitRequested = true;
    }
}
```

## 11. 实现用例对应关系

| 用例 | 关键脚本 |
|---|---|
| 首次进入/主小镇 | `CatLifeAppController`, `CatLifeUIStateView` |
| 开始专注 | `FocusStateMachine`, `SimpleBehaviorScoreEngine` |
| 自动过渡 | `SimpleBehaviorScoreEngine`, `FocusStateMachine` |
| 猫动画反馈 | `CatAnimationDriver` |
| 专注 UI 低刺激 | `CatLifeUIStateView` |
| LLM 解释 | `ILLMClient`, `MockLLMClient`, `PrivacyGateway` |
| 无网降级 | `MockLLMClient`, 本地模板 |
| 调试演示 | `DebugScorePanel` 后续实现 |

## 12. 工程落地顺序

1. 建立 `CatLife_Main.unity` 和 UI Canvas。
2. 接入 `CatLifeAppController`。
3. 用按钮手动触发 Normal/Transition/Focus/Reward。
4. 接 Animator 参数。
5. 接行为评分器。
6. 接记录和奖励。
7. 接 LLM mock。
8. 接真实 LLM 客户端。
9. 做 Android 真机 QA。
