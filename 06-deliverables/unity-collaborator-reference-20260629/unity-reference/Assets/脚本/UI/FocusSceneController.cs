using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CatLife.Core;
using CatLife.LLM;

namespace CatLife.UI
{
    /// <summary>
    /// FocusScene 专注场景的运行时控制器
    /// 需要挂载到 FocusScene 的 Canvas 所在 GameObject 上
    ///
    /// 场景搭建步骤：
    /// 1. 创建 FocusScene.unity
    /// 2. 添加 Canvas + Camera（Screen Space Overlay）
    /// 3. 添加 Panel 作为背景（可选）
    /// 4. 添加 TextMeshPro 显示「专注中...」
    /// 5. 添加上滑退出提示 UI
    /// 6. 将本脚本挂载到 Canvas 上，配置引用
    /// 7. 添加 FocusUIManager 组件
    /// </summary>
    public class FocusSceneController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI focusScoreText;
        [SerializeField] private Image progressFillImage;
        [SerializeField] private Button exitButton;         // 备用退出按钮
        [SerializeField] private GameObject[] hideInFocus; // 专注时隐藏的 UI

        [Header("专注配置")]
        [SerializeField] private int focusMinutes = 25;

        // 内部状态
        private FocusUIManager _focusUI;
        private float _focusStartTime;
        private bool _isActive = false;

        private void Awake()
        {
            _focusUI = GetComponent<FocusUIManager>();
        }

        private void OnEnable()
        {
            if (_focusUI != null)
            {
                _focusUI.OnEnterFocus += HandleEnterFocus;
                _focusUI.OnExitFocus += HandleExitFocus;
                _focusUI.OnFocusProgressUpdate += HandleProgressUpdate;
            }

            // 监听状态机
            StateMachine.GlobalStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (_focusUI != null)
            {
                _focusUI.OnEnterFocus -= HandleEnterFocus;
                _focusUI.OnExitFocus -= HandleExitFocus;
                _focusUI.OnFocusProgressUpdate -= HandleProgressUpdate;
            }

            StateMachine.GlobalStateChanged -= HandleStateChanged;
        }

        private void Start()
        {
            _focusStartTime = Time.time;
            _isActive = true;

            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClicked);
            }
        }

        private void Update()
        {
            if (!_isActive) return;

            // 更新计时器
            float elapsed = Time.time - _focusStartTime;
            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);

            if (timerText != null)
            {
                timerText.text = $"{minutes:D2}:{seconds:D2}";
            }
        }

        private void HandleStateChanged(CatState newState)
        {
            if (newState == CatState.Reward)
            {
                // 专注结束，进入奖励
                ShowReward();
            }
        }

        private void HandleEnterFocus()
        {
            Debug.Log("[FocusSceneController] 进入专注");
            if (statusText != null)
                statusText.text = "专注中...";
        }

        private void HandleExitFocus()
        {
            Debug.Log("[FocusSceneController] 退出专注");
            _isActive = false;
        }

        private void HandleProgressUpdate(float progress)
        {
            if (progressFillImage != null)
            {
                progressFillImage.fillAmount = progress;
            }

            if (focusScoreText != null && BehaviorTracker.Instance != null)
            {
                float score = BehaviorTracker.Instance.GetCurrentActivityScore();
                focusScoreText.text = $"专注度: {(1f - score) * 100:F0}%";
            }

            // 专注达标
            if (progress >= 1f)
            {
                OnFocusComplete();
            }
        }

        /// <summary>
        /// 专注时间达标
        /// </summary>
        private void OnFocusComplete()
        {
            if (statusText != null)
                statusText.text = "太棒了！专注完成！";

            // 通知数据管理器
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnFocusComplete(focusMinutes);
            }

            // 触发奖励
            StateMachine sm = FindFirstObjectByType<StateMachine>();
            sm?.SwitchState(CatState.Reward);
        }

        /// <summary>
        /// 退出按钮点击
        /// </summary>
        private void OnExitClicked()
        {
            if (_focusUI != null)
            {
                _focusUI.ExitFocus();
            }
        }

        /// <summary>
        /// 显示奖励界面
        /// </summary>
        private void ShowReward()
        {
            // 隐藏专注 UI，显示奖励
            if (statusText != null)
                statusText.text = "🎉 专注完成！";

            // 显示星星/奖励特效
            // TODO: 添加粒子效果或奖励界面
        }

        /// <summary>
        /// 手动设置专注时长（分钟）
        /// </summary>
        public void SetFocusDuration(int minutes)
        {
            focusMinutes = minutes;
            if (_focusUI != null)
            {
                _focusUI.SetFocusDuration(minutes);
            }
        }
    }
}
