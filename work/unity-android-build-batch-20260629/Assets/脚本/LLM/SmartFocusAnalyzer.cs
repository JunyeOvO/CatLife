using UnityEngine;
using System;
using System.Text;
using CatLife.Core;

namespace CatLife.LLM
{
    /// <summary>
    /// 专注状态分析器
    /// 结合 BehaviorTracker 数据 + BlueLM LLM 分析，决定是否进入专注状态
    /// </summary>
    public class SmartFocusAnalyzer : MonoBehaviour
    {
        [Header("LLM 客户端")]
        [SerializeField] private ILLMClient llmClient;

        [Header("分析配置")]
        [SerializeField] private float analysisInterval = 30f;  // 每 30 秒分析一次
        [SerializeField] private float focusThreshold = 0.3f;    // 活跃度阈值
        [SerializeField] private bool useLLMAnalysis = true;      // 是否启用 LLM 分析

        [Header("状态机引用")]
        [SerializeField] private StateMachine stateMachine;

        // 内部状态
        private float _lastAnalysisTime = 0f;
        private bool _isAnalyzing = false;
        private StringBuilder _pendingResponse = new StringBuilder();

        public bool IsAnalyzing => _isAnalyzing;

        private void Awake()
        {
            if (stateMachine == null)
                stateMachine = FindFirstObjectByType<StateMachine>();
        }

        private void OnEnable()
        {
            if (BehaviorTracker.Instance != null)
            {
                BehaviorTracker.Instance.OnDataUpdated += HandleBehaviorDataUpdated;
            }
        }

        private void OnDisable()
        {
            if (BehaviorTracker.Instance != null)
            {
                BehaviorTracker.Instance.OnDataUpdated -= HandleBehaviorDataUpdated;
            }
        }

        /// <summary>
        /// 行为数据更新回调（来自 BehaviorTracker）
        /// </summary>
        private void HandleBehaviorDataUpdated(BehaviorData data)
        {
            // 只在 Normal 状态且未分析时检查
            if (stateMachine != null && !stateMachine.IsState(CatState.Normal))
                return;

            if (Time.time - _lastAnalysisTime < analysisInterval)
                return;

            _lastAnalysisTime = Time.time;

            if (useLLMAnalysis && llmClient != null && llmClient.IsInitialized)
            {
                AnalyzeWithLLM(data);
            }
            else
            {
                AnalyzeWithRules(data);
            }
        }

        /// <summary>
        /// 基于规则的分析（简单阈值判断）
        /// </summary>
        private void AnalyzeWithRules(BehaviorData data)
        {
            float score = data.GetActivityScore();
            bool isFocused = score < focusThreshold;

            Debug.Log($"[SmartFocusAnalyzer] 规则分析 - 活跃度分数: {score:F2}, 专注: {isFocused}");

            if (isFocused && stateMachine != null && stateMachine.CanEnterFocus())
            {
                // 连续 3 次低于阈值才触发
                if (BehaviorTracker.Instance != null && BehaviorTracker.Instance.IsCurrentlyFocused())
                {
                    TriggerFocusMode();
                }
            }
        }

        /// <summary>
        /// 基于 LLM 的分析（更智能）
        /// </summary>
        private void AnalyzeWithLLM(BehaviorData data)
        {
            if (_isAnalyzing) return;
            _isAnalyzing = true;

            string prompt = BuildAnalysisPrompt(data);
            Debug.Log($"[SmartFocusAnalyzer] LLM 分析 Prompt:\n{prompt}");

            _pendingResponse.Clear();

            llmClient.Generate(prompt, new LLMCallback
            {
                OnToken = (token) =>
                {
                    _pendingResponse.Append(token);
                },
                OnComplete = () =>
                {
                    string response = _pendingResponse.ToString();
                    FocusAnalysisResult result = ParseLLMResponse(response);
                    ProcessAnalysisResult(result);
                    _isAnalyzing = false;
                },
                OnError = (code, msg) =>
                {
                    Debug.LogError($"[SmartFocusAnalyzer] LLM 分析失败: {code} - {msg}");
                    _isAnalyzing = false;
                    // 降级到规则分析
                    AnalyzeWithRules(data);
                }
            });
        }

        /// <summary>
        /// 构建分析 Prompt
        /// </summary>
        private string BuildAnalysisPrompt(BehaviorData data)
        {
            return $"[|Human|]:用户在过去30秒的行为数据如下：\n" +
                   $"- 点击次数: {data.clickCount30s}\n" +
                   $"- 滑动次数: {data.swipeCount30s}\n" +
                   $"- 滚动次数: {data.scrollCount30s}\n" +
                   $"- 空闲时间: {data.idleSeconds30s:F1}秒\n" +
                   $"请判断用户是否处于专注状态（不是分心/走神），" +
                   $"只回答「专注」或「分心」，不用解释。\n[|AI|]:";
        }

        /// <summary>
        /// 解析 LLM 响应
        /// </summary>
        private FocusAnalysisResult ParseLLMResponse(string rawResponse)
        {
            var result = new FocusAnalysisResult
            {
                rawResponse = rawResponse,
                isFocused = rawResponse.Contains("专注"),
                score = rawResponse.Contains("专注") ? 0.2f : 0.7f
            };

            if (rawResponse.Contains("专注"))
                result.suggestion = "检测到用户专注，建议进入专注模式";
            else if (rawResponse.Contains("分心"))
                result.suggestion = "检测到用户分心，猫咪将走近提醒";
            else
                result.suggestion = "无法判断，维持当前状态";

            return result;
        }

        /// <summary>
        /// 处理分析结果
        /// </summary>
        private void ProcessAnalysisResult(FocusAnalysisResult result)
        {
            Debug.Log($"[SmartFocusAnalyzer] 分析结果: {result.suggestion}");

            if (result.isFocused && stateMachine != null && stateMachine.CanEnterFocus())
            {
                // LLM 也认为专注，进入专注
                TriggerFocusMode();
            }
            else if (!result.isFocused && stateMachine != null && stateMachine.IsState(CatState.Focus))
            {
                // 检测到分心，从专注退出
                stateMachine.ExitFocus();
            }
        }

        /// <summary>
        /// 触发专注模式
        /// </summary>
        private void TriggerFocusMode()
        {
            if (stateMachine != null)
            {
                stateMachine.SwitchState(CatState.Transition);
                // 延迟一点再进入 Focus，让过渡动画播放
                stateMachine.SwitchStateDelayed(CatState.Focus, 2f);
            }
        }

        /// <summary>
        /// 手动触发一次分析
        /// </summary>
        public void TriggerAnalysis()
        {
            if (BehaviorTracker.Instance != null)
            {
                HandleBehaviorDataUpdated(BehaviorTracker.Instance.CurrentData);
            }
        }

        /// <summary>
        /// 设置 LLM 客户端
        /// </summary>
        public void SetLLMClient(ILLMClient client)
        {
            llmClient = client;
        }
    }
}