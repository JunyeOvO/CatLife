using UnityEngine;
using System;
using UnityEngine.UI;
using CatLife.Core;

namespace CatLife.UI
{
    /// <summary>
    /// 专注模式 UI 管理器
    /// 处理轻锁定 UI 的显示/隐藏，上滑退出检测
    /// </summary>
    public class FocusUIManager : MonoBehaviour
    {
        [Header("专注时隐藏的 UI 元素")]
        [SerializeField] private GameObject[] hideInFocusObjects;

        [Header("专注时显示的元素")]
        [SerializeField] private GameObject focusHintObject;      // "专注中..." 提示
        [SerializeField] private GameObject swipeUpHintObject;    // "上滑退出" 提示
        [SerializeField] private GameObject focusProgressBar;    // 专注进度条（可选）

        [Header("上滑配置")]
        [SerializeField] private float swipeDistanceThreshold = 200f;   // 触发距离（像素）
        [SerializeField] private float swipeVelocityThreshold = 500f;     // 触发速度（像素/秒）
        [SerializeField] private bool allowExitBySwipe = true;   // 是否允许上滑退出

        [Header("专注计时")]
        [SerializeField] private float focusDuration = 25f * 60f; // 默认 25 分钟（番茄钟）

        // 状态
        private bool isInFocusMode = false;
        private float focusStartTime;
        private Vector2 touchStartPos;
        private float touchStartTime;

        // 事件
        public event Action OnEnterFocus;
        public event Action OnExitFocus;
        public event Action<float> OnFocusProgressUpdate; // 0-1 进度

        // 只读属性
        public bool IsInFocusMode => isInFocusMode;
        public float FocusElapsed => isInFocusMode ? Time.time - focusStartTime : 0f;
        public float FocusProgress => isInFocusMode ? Mathf.Clamp01(FocusElapsed / focusDuration) : 0f;

        private void OnEnable()
        {
            StateMachine.GlobalStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            StateMachine.GlobalStateChanged -= HandleStateChanged;
        }

        private void Update()
        {
            if (!isInFocusMode || !allowExitBySwipe) return;

            // 桌面端鼠标滑动
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                touchStartTime = Time.time;
            }

            if (Input.GetMouseButtonUp(0))
            {
                float deltaY = Input.mousePosition.y - touchStartPos.y;
                float elapsed = Time.time - touchStartTime;
                float velocity = elapsed > 0 ? deltaY / elapsed : 0f;
                if (deltaY > swipeDistanceThreshold && velocity > swipeVelocityThreshold)
                {
                    ExitFocus();
                }
            }

            // 移动端触摸滑动
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                    touchStartTime = Time.time;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    float deltaY = touch.position.y - touchStartPos.y;
                    float elapsed = Time.time - touchStartTime;
                    float velocity = elapsed > 0 ? deltaY / elapsed : 0f;
                    if (deltaY > swipeDistanceThreshold && velocity > swipeVelocityThreshold)
                    {
                        ExitFocus();
                    }
                }
            }

            // 更新进度
            if (isInFocusMode)
            {
                OnFocusProgressUpdate?.Invoke(FocusProgress);
            }
        }

        /// <summary>
        /// 进入专注模式
        /// </summary>
        public void EnterFocus()
        {
            if (isInFocusMode) return;

            isInFocusMode = true;
            focusStartTime = Time.time;

            Debug.Log("[FocusUIManager] 进入专注模式");

            // 隐藏不需要的元素
            SetObjectsActive(hideInFocusObjects, false);

            // 显示专注相关元素
            if (focusHintObject != null) focusHintObject.SetActive(true);
            if (swipeUpHintObject != null) swipeUpHintObject.SetActive(true);
            if (focusProgressBar != null) focusProgressBar.SetActive(true);

            OnEnterFocus?.Invoke();
        }

        /// <summary>
        /// 退出专注模式
        /// </summary>
        public void ExitFocus()
        {
            if (!isInFocusMode) return;

            float elapsed = FocusElapsed;
            isInFocusMode = false;

            Debug.Log($"[FocusUIManager] 退出专注模式，专注时长: {elapsed:F1}秒");

            // 恢复 UI
            SetObjectsActive(hideInFocusObjects, true);

            if (focusHintObject != null) focusHintObject.SetActive(false);
            if (swipeUpHintObject != null) swipeUpHintObject.SetActive(false);
            if (focusProgressBar != null) focusProgressBar.SetActive(false);

            OnExitFocus?.Invoke();
        }

        /// <summary>
        /// 设置多个对象 active 状态
        /// </summary>
        private void SetObjectsActive(GameObject[] objects, bool active)
        {
            if (objects == null) return;
            foreach (var obj in objects)
            {
                if (obj != null) obj.SetActive(active);
            }
        }

        /// <summary>
        /// 状态变化回调
        /// </summary>
        private void HandleStateChanged(CatState newState)
        {
            if (newState == CatState.Focus)
            {
                EnterFocus();
            }
            else if (newState == CatState.Reward || newState == CatState.Normal)
            {
                if (isInFocusMode) ExitFocus();
            }
        }

        /// <summary>
        /// 设置专注时长
        /// </summary>
        public void SetFocusDuration(float minutes)
        {
            focusDuration = minutes * 60f;
        }

        /// <summary>
        /// 延长专注时间
        /// </summary>
        public void ExtendFocusDuration(float additionalMinutes)
        {
            focusDuration += additionalMinutes * 60f;
        }

        /// <summary>
        /// 获取当前专注时长（分钟）
        /// </summary>
        public float GetCurrentFocusMinutes()
        {
            return FocusElapsed / 60f;
        }
    }
}
