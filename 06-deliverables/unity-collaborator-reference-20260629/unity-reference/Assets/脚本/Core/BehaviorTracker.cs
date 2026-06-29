using UnityEngine;
using System;

namespace CatLife.Core
{
    /// <summary>
    /// 行为数据结构
    /// </summary>
    [System.Serializable]
    public class BehaviorData
    {
        public int clickCount30s;
        public int swipeCount30s;
        public int scrollCount30s;
        public float idleSeconds30s;
        public float totalActiveTime;
        public int appSwitchCount; // 切换出去多少次
        public float sessionDuration; // 本次使用时长

        /// <summary>
        /// 获取活跃度分数（0-1）
        /// 分数越高说明越活跃（可能分心了）
        /// </summary>
        public float GetActivityScore()
        {
            float clickWeight = 0.3f;
            float swipeWeight = 0.2f;
            float scrollWeight = 0.2f;
            float idleWeight = 0.3f;

            // 点击和滑动越多，分数越高
            float clickScore = Mathf.Clamp01(clickCount30s / 20f);
            float swipeScore = Mathf.Clamp01(swipeCount30s / 15f);
            float scrollScore = Mathf.Clamp01(scrollCount30s / 10f);

            // 空闲越久，分数越低
            float idleScore = 1f - Mathf.Clamp01(idleSeconds30s / 30f);

            float score = clickScore * clickWeight +
                          swipeScore * swipeWeight +
                          scrollScore * scrollWeight +
                          idleScore * idleWeight;

            return Mathf.Clamp01(score);
        }

        /// <summary>
        /// 判断是否处于专注状态
        /// </summary>
        public bool IsFocused(float threshold = 0.3f)
        {
            return GetActivityScore() < threshold;
        }
    }

    /// <summary>
    /// 行为追踪器
    /// 采集用户在 30 秒窗口内的交互数据
    /// </summary>
    public class BehaviorTracker : MonoBehaviour
    {
        public static BehaviorTracker Instance { get; private set; }

        [Header("分析窗口（秒）")]
        [SerializeField] private float analysisWindow = 30f;

        [Header("活跃度阈值")]
        [SerializeField] private float activityThreshold = 0.3f;

        [Header("连续多少次低于阈值才触发专注判断")]
        [SerializeField] private int consecutiveFocusRequired = 3;

        // 实时数据
        private BehaviorData currentData = new BehaviorData();

        // 历史记录（用于连续判断）
        private float[] activityHistory;
        private int historyIndex = 0;
        private int consecutiveFocusCount = 0;

        // 事件
        public event Action<BehaviorData> OnDataUpdated;
        public event Action<bool> OnFocusStateChanged; // true=专注, false=分心

        // 属性
        public BehaviorData CurrentData => currentData;
        public float ActivityThreshold => activityThreshold;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            activityHistory = new float[10]; // 最近 10 次的分析结果
        }

        private void OnEnable()
        {
            StartCoroutine(ResetWindowRoutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            // 检测点击
            if (Input.GetMouseButtonDown(0))
            {
                currentData.clickCount30s++;
            }

            // 检测触摸滑动
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // 开始触摸
                if (touch.phase == TouchPhase.Began)
                {
                    // 可以记录开始位置
                }
                // 移动（滑动）
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (touch.deltaPosition.magnitude > 5f)
                    {
                        currentData.swipeCount30s++;
                    }
                }
            }

            // 检测滚动（鼠标滚轮）
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentData.scrollCount30s++;
            }

            // 检测空闲（无任何输入）
            if (!Input.anyKey && Input.touchCount == 0 &&
                Mathf.Approximately(Input.GetAxis("Mouse ScrollWheel"), 0f))
            {
                currentData.idleSeconds30s += Time.deltaTime;
            }
        }

        /// <summary>
        /// 定期重置计数器
        /// </summary>
        private System.Collections.IEnumerator ResetWindowRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(analysisWindow);
                AnalyzeCurrentWindow();
                ResetCounters();
            }
        }

        /// <summary>
        /// 分析当前窗口的数据
        /// </summary>
        private void AnalyzeCurrentWindow()
        {
            float score = currentData.GetActivityScore();

            // 记录历史
            activityHistory[historyIndex] = score;
            historyIndex = (historyIndex + 1) % activityHistory.Length;

            // 连续判断
            bool isFocused = currentData.IsFocused(activityThreshold);
            if (isFocused)
            {
                consecutiveFocusCount++;
            }
            else
            {
                consecutiveFocusCount = 0;
            }

            // 触发事件（连续 N 次低于阈值才认为真正专注）
            if (consecutiveFocusCount >= consecutiveFocusRequired)
            {
                OnFocusStateChanged?.Invoke(true);
            }
            else if (consecutiveFocusCount == 0)
            {
                OnFocusStateChanged?.Invoke(false);
            }

            // 通知数据更新
            OnDataUpdated?.Invoke(currentData);

            Debug.Log($"[BehaviorTracker] 活跃度分数: {score:F2}, 连续专注次数: {consecutiveFocusCount}");
        }

        private void ResetCounters()
        {
            currentData = new BehaviorData()
            {
                clickCount30s = 0,
                swipeCount30s = 0,
                scrollCount30s = 0,
                idleSeconds30s = 0f
            };
        }

        /// <summary>
        /// 获取当前活跃度分数
        /// </summary>
        public float GetCurrentActivityScore()
        {
            return currentData.GetActivityScore();
        }

        /// <summary>
        /// 检查当前是否专注
        /// </summary>
        public bool IsCurrentlyFocused()
        {
            return consecutiveFocusCount >= consecutiveFocusRequired;
        }

        /// <summary>
        /// 记录 App 切换
        /// </summary>
        public void RecordAppSwitch()
        {
            currentData.appSwitchCount++;
        }

        /// <summary>
        /// 更新会话时长
        /// </summary>
        public void UpdateSessionDuration(float duration)
        {
            currentData.sessionDuration = duration;
        }
    }
}
