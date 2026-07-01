using UnityEngine;
using System;
using System.Collections;
using CatLife.Core;

namespace CatLife.UI
{
    /// <summary>
    /// 页面管理枚举
    /// </summary>
    public enum UIPage
    {
        None,
        Splash,    // 开屏页
        Main,      // 主页（猫咪展示+开始专注按钮）
        Focus,     // 专注页（专注计时界面）
        Reward,    // 奖励页（专注完成的庆祝界面）
        Settings   // 设置页
    }

    /// <summary>
    /// 统一的页面管理系统
    /// 管理所有 Panel 之间的切换和过渡动画
    /// 替换原来只有淡入功能的 UIManager
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("所有页面 Panel（按 UIPage 顺序排列）")]
        [SerializeField] private GameObject splashPage;    // 开屏页
        [SerializeField] private GameObject mainPage;       // 主页
        [SerializeField] private GameObject focusPage;      // 专注页
        [SerializeField] private GameObject rewardPage;     // 奖励页
        [SerializeField] private GameObject settingsPage;    // 设置页

        [Header("过渡动画配置")]
        [SerializeField] private float pageFadeDuration = 0.4f;
        [SerializeField] private AnimationCurve fadeCurve = null; // 可选曲线

        [Header("背景遮罩（可选，用于页面间过渡）")]
        [SerializeField] private GameObject transitionOverlay;

        // 事件
        public event Action<UIPage> OnPageChanged;
        public event Action<UIPage, UIPage> OnPageTransitionStart; // from -> to

        // 当前状态
        public UIPage CurrentPage { get; private set; } = UIPage.None;
        public UIPage PreviousPage { get; private set; } = UIPage.None;
        private bool _isTransitioning = false;

        // 页面堆栈（用于返回）
        private System.Collections.Generic.Stack<UIPage> _pageStack = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            StateMachine.GlobalStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            StateMachine.GlobalStateChanged -= HandleStateChanged;
        }

        private void Start()
        {
            // 初始：全部隐藏，等协程统一处理
            SetPageActive(splashPage, false);
            SetPageActive(mainPage, false);
            SetPageActive(focusPage, false);
            SetPageActive(rewardPage, false);
            SetPageActive(settingsPage, false);

            // 启动时显示主页（或开屏页）
            ShowPage(UIPage.Main, false);
        }

        /// <summary>
        /// 显示指定页面（带过渡动画）
        /// </summary>
        /// <param name="page">目标页面</param>
        /// <param name="animate">是否播放动画，默认 true</param>
        /// <param name="addToStack">是否加入返回堆栈，默认 true（主页不加入）</param>
        public void ShowPage(UIPage page, bool animate = true, bool addToStack = true)
        {
            if (_isTransitioning && page != CurrentPage) return;
            if (page == CurrentPage) return;

            UIPage fromPage = CurrentPage;
            PreviousPage = CurrentPage;

            // 主页不加堆栈（主页是最终返回点）
            if (addToStack && CurrentPage != UIPage.None && CurrentPage != UIPage.Main)
            {
                _pageStack.Push(CurrentPage);
            }

            StartCoroutine(TransitionToPage(fromPage, page, animate));

            OnPageTransitionStart?.Invoke(fromPage, page);
            CurrentPage = page;
            OnPageChanged?.Invoke(page);
        }

        /// <summary>
        /// 返回上一个页面（自动根据堆栈返回）
        /// </summary>
        public void GoBack()
        {
            if (_pageStack.Count == 0)
            {
                Debug.Log("[UIManager] 没有页面可返回");
                return;
            }

            UIPage targetPage = _pageStack.Pop();
            ShowPage(targetPage, true, false); // 不再加入堆栈，避免死循环
        }

        /// <summary>
        /// 直接切换到专注页
        /// </summary>
        public void ShowFocusPage()
        {
            ShowPage(UIPage.Focus);
        }

        /// <summary>
        /// 直接切换到奖励页
        /// </summary>
        public void ShowRewardPage()
        {
            ShowPage(UIPage.Reward);
        }

        /// <summary>
        /// 直接切换到设置页
        /// </summary>
        public void ShowSettingsPage()
        {
            ShowPage(UIPage.Settings);
        }

        /// <summary>
        /// 状态机状态变化时自动切换页面
        /// </summary>
        private void HandleStateChanged(CatState newState)
        {
            switch (newState)
            {
                case CatState.Normal:
                    ShowPage(UIPage.Main);
                    break;
                case CatState.Transition:
                    // 过渡状态还在主页，不需要强制切换
                    break;
                case CatState.Focus:
                    ShowPage(UIPage.Focus);
                    break;
                case CatState.Reward:
                    ShowPage(UIPage.Reward);
                    break;
            }
        }

        /// <summary>
        /// 页面过渡核心协程
        /// </summary>
        private IEnumerator TransitionToPage(UIPage from, UIPage to, bool animate)
        {
            _isTransitioning = true;

            GameObject fromObj = GetPageObject(from);
            GameObject toObj = GetPageObject(to);

            if (animate && pageFadeDuration > 0)
            {
                // 淡出旧页面
                if (fromObj != null)
                {
                    yield return StartCoroutine(FadePage(fromObj, 1f, 0f));
                    SetPageActive(fromObj, false);
                }

                // 淡入新页面
                if (toObj != null)
                {
                    SetPageActive(toObj, true);
                    yield return StartCoroutine(FadePage(toObj, 0f, 1f));
                }
            }
            else
            {
                // 无动画：直接切换
                if (fromObj != null) SetPageActive(fromObj, false);
                if (toObj != null) SetPageActive(toObj, true);
            }

            _isTransitioning = false;
            Debug.Log($"[UIManager] 页面切换: {from} → {to}");
        }

        /// <summary>
        /// 页面淡入淡出协程
        /// </summary>
        private IEnumerator FadePage(GameObject pageObj, float fromAlpha, float toAlpha)
        {
            CanvasGroup canvasGroup = pageObj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // 没有 CanvasGroup，自动添加一个
                canvasGroup = pageObj.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = fromAlpha;
            if (toAlpha > 0) canvasGroup.interactable = true;

            float elapsed = 0f;
            while (elapsed < pageFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / pageFadeDuration;
                if (fadeCurve != null) t = fadeCurve.Evaluate(t);
                canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = toAlpha;
            if (toAlpha == 0) canvasGroup.interactable = false;
        }

        /// <summary>
        /// 设置页面 active 状态
        /// </summary>
        private void SetPageActive(GameObject pageObj, bool active)
        {
            if (pageObj == null) return;
            if (active)
            {
                pageObj.SetActive(true);
            }
            else
            {
                pageObj.SetActive(false);
            }
        }

        /// <summary>
        /// 根据 UIPage 获取对应 GameObject
        /// </summary>
        private GameObject GetPageObject(UIPage page)
        {
            return page switch
            {
                UIPage.Splash => splashPage,
                UIPage.Main => mainPage,
                UIPage.Focus => focusPage,
                UIPage.Reward => rewardPage,
                UIPage.Settings => settingsPage,
                _ => null
            };
        }

        /// <summary>
        /// 获取页面堆栈深度（用于调试或返回按钮显示）
        /// </summary>
        public int GetStackDepth() => _pageStack.Count;

        /// <summary>
        /// 清空堆栈并返回主页
        /// </summary>
        public void ReturnToMain()
        {
            _pageStack.Clear();
            ShowPage(UIPage.Main);
        }
    }
}