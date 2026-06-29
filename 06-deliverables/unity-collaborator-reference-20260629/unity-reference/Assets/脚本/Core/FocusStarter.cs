using UnityEngine;
using CatLife.Core;
using CatLife.UI;

namespace CatLife.Core
{
    /// <summary>
    /// 专注启动器：挂在"开始专注"按钮上
    /// 处理用户主动触发专注，以及定时/开放两种模式
    /// </summary>
    public class FocusStarter : MonoBehaviour
    {
        public enum FocusMode
        {
            Timer,  // 定时模式
            Open    // 开放模式（无时限）
        }

        [Header("专注配置")]
        [SerializeField] private FocusMode focusMode = FocusMode.Timer;
        [SerializeField] private float timerMinutes = 25f;   // 定时模式时长

        [Header("需要隐藏的UI（进入专注时）")]
        [SerializeField] private GameObject[] hideOnFocus;

        [Header("奖励弹窗")]
        public GameObject rewardPanel;

        [Header("上滑退出提示")]
        public GameObject swipeHint;

        private StateMachine stateMachine;
        private FocusUIManager focusUI;
        private bool isFocusActive = false;
        private float focusStartTime;

        public bool IsFocusActive => isFocusActive;

        private void Awake()
        {
            stateMachine = FindFirstObjectByType<StateMachine>();
            focusUI = FindFirstObjectByType<CatLife.UI.FocusUIManager>();
        }

        private void OnEnable()
        {
            StateMachine.GlobalStateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            StateMachine.GlobalStateChanged -= OnStateChanged;
        }

        /// <summary>
        /// UI按钮调用：开始专注
        /// </summary>
        public void StartFocus()
        {
            if (isFocusActive) return;
            if (stateMachine == null || !stateMachine.CanEnterFocus()) return;

            isFocusActive = true;
            focusStartTime = Time.time;

            // 设置专注时长（如果是定时模式）
            if (focusUI != null && focusMode == FocusMode.Timer)
            {
                focusUI.SetFocusDuration(timerMinutes);
            }

            // 隐藏普通UI
            SetObjectsActive(hideOnFocus, false);

            // 显示上滑提示
            if (swipeHint != null) swipeHint.SetActive(true);

            // 进入专注状态
            stateMachine.SwitchState(CatState.Focus);

            Debug.Log($"[FocusStarter] 开始专注，模式={focusMode}，时长={timerMinutes}分钟");
        }

        /// <summary>
        /// 结束专注（定时到 / 上滑退出 / 放弃）
        /// </summary>
        public void EndFocus(bool completed)
        {
            if (!isFocusActive) return;

            isFocusActive = false;
            float elapsed = Time.time - focusStartTime;
            int minutes = Mathf.FloorToInt(elapsed / 60f);

            // 恢复UI
            if (swipeHint != null) swipeHint.SetActive(false);
            SetObjectsActive(hideOnFocus, true);

            if (completed && minutes > 0)
            {
                // 记录数据
                if (DataManager.Instance != null)
                {
                    DataManager.Instance.RecordFocusSession(minutes);
                    DataManager.Instance.AddCoins(minutes);
                }

                // 切换到奖励状态
                stateMachine.SwitchState(CatState.Reward);

                // 显示奖励弹窗
                ShowReward(minutes);
            }
            else
            {
                // 放弃：直接回到普通
                stateMachine.SwitchState(CatState.Normal);
            }

            Debug.Log($"[FocusStarter] 结束专注，完成={completed}，时长={minutes}分钟");
        }

        /// <summary>
        /// 定时到达自动结束
        /// </summary>
        public void OnTimerComplete()
        {
            EndFocus(true);
        }

        private void Update()
        {
            if (!isFocusActive) return;

            // 定时模式：检查时间到
            if (focusMode == FocusMode.Timer && focusUI != null)
            {
                if (focusUI.FocusProgress >= 1f)
                {
                    OnTimerComplete();
                }
            }
        }

        private void OnStateChanged(CatState state)
        {
            if (state == CatState.Normal || state == CatState.Transition)
            {
                // 如果之前在专注中但被外部强制切走，说明放弃了
                if (isFocusActive)
                {
                    isFocusActive = false;
                    if (swipeHint != null) swipeHint.SetActive(false);
                    SetObjectsActive(hideOnFocus, true);
                }
            }
        }

        private void ShowReward(int minutes)
        {
            if (rewardPanel != null)
            {
                rewardPanel.SetActive(true);

                // 更新显示
                var minutesText = rewardPanel.transform.Find("MinutesText");
                var coinsText = rewardPanel.transform.Find("CoinsText");
                if (minutesText != null)
                    minutesText.GetComponent<UnityEngine.UI.Text>().text = $"{minutes}分钟";
                if (coinsText != null)
                    coinsText.GetComponent<UnityEngine.UI.Text>().text = $"+{minutes} 金币";
            }
        }

        /// <summary>
        /// 奖励弹窗关闭按钮调用
        /// </summary>
        public void CloseRewardPanel()
        {
            if (rewardPanel != null)
                rewardPanel.SetActive(false);
            stateMachine?.SwitchState(CatState.Normal);
        }

        public void SetTimerMinutes(float minutes)
        {
            timerMinutes = minutes;
        }

        public void SetFocusMode(FocusMode mode)
        {
            focusMode = mode;
        }

        private void SetObjectsActive(GameObject[] objects, bool active)
        {
            if (objects == null) return;
            foreach (var obj in objects)
                if (obj != null) obj.SetActive(active);
        }
    }
}
