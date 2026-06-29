using UnityEngine;
using System.Collections;
using CatLife.Core;

public class MainSceneController : MonoBehaviour
{
    [Header("猫的设置")]
    [SerializeField] private GameObject catModel;
    [SerializeField] private Animator catAnimator;

    [Header("转身设置")]
    [SerializeField] private float turnDuration = 2.5f;
    [SerializeField] private float turnRadius = 1.5f;
    [SerializeField] private float walkDistance = 2f;
    [SerializeField] private float walkDuration = 0.8f;  // 走路用时

    [Header("UI 设置")]
    [SerializeField] private CanvasGroup mainUICanvasGroup;
    [SerializeField] private float uiFadeDuration = 1f;

    [Header("上滑条设置")]
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private float swipeThreshold = 50f;

    [Header("下一步设置")]
    [SerializeField] private string nextSceneName = "FocusScene";

    private bool isUnlockActive = false;
    private Vector2 touchStartPos;
    private bool isSequenceReady = false;
    private bool hasStartedWalking = false;  // 防止重复触发

private void OnEnable()
    {
        CameraExactTransition.OnCameraTransitionComplete += OnCameraMoveComplete;
        StateMachine.GlobalStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        CameraExactTransition.OnCameraTransitionComplete -= OnCameraMoveComplete;
        StateMachine.GlobalStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(CatState newState)
    {
        if (newState == CatState.Transition && !hasStartedWalking)
        {
            StartCoroutine(FullSequence());
        }
    }

    private void Start()
    {
        if (catAnimator != null)
        {
            catAnimator.speed = 0;
        }

        // 初始隐藏上滑条
        if (unlockPanel != null)
        {
            unlockPanel.SetActive(false);
        }
    }

    private void OnCameraMoveComplete()
    {
        Debug.Log("相机移动完成");
        StartCoroutine(FullSequence());
    }

    private IEnumerator FullSequence()
    {
        if (catAnimator != null)
        {
            catAnimator.speed = 1f;
        }

        yield return StartCoroutine(PlayCatAnimation());
        yield return StartCoroutine(ArcTurn());

        // 注意：UI 渐隐和上滑条已经在 ArcTurn 的走路阶段同时触发了
        // 这里只需要等待足够时间让动画完成
        Debug.Log("所有动画完成");
    }

    private IEnumerator PlayCatAnimation()
    {
        if (catAnimator != null && catAnimator.runtimeAnimatorController != null)
        {
            const string transitionStateName = "CL_CAT_CuriousSniff_v02_loop_112f";
            int transitionStateHash = Animator.StringToHash(transitionStateName);
            if (catAnimator.HasState(0, transitionStateHash))
            {
                Debug.Log("播放动画: " + transitionStateName);
                catAnimator.CrossFadeInFixedTime(transitionStateHash, 0.15f, 0, 0f);
                yield return new WaitForSeconds(1.9f);
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator ArcTurn()
    {
        if (catModel == null) yield break;

        Transform cat = catModel.transform;
        Vector3 startPos = cat.position;
        float startAngle = cat.eulerAngles.y;
        float targetAngle = startAngle + 180f;

        Vector3 center = startPos - cat.right * turnRadius;

        float elapsed = 0f;

        if (catAnimator != null) catAnimator.speed = 1f;

        // 第一阶段：弧形转身 180°
        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / turnDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            float angle = Mathf.Lerp(0, 180, smoothT);
            float rad = angle * Mathf.Deg2Rad;

            float x = center.x + Mathf.Cos(rad) * turnRadius;
            float z = center.z + Mathf.Sin(rad) * turnRadius;
            Vector3 newPos = new Vector3(x, startPos.y, z);

            float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, smoothT);

            cat.position = newPos;
            cat.eulerAngles = new Vector3(0, currentAngle, 0);

            yield return null;
        }

        // 确保最终位置精确
        float finalRad = 180 * Mathf.Deg2Rad;
        Vector3 finalPos = center + new Vector3(Mathf.Cos(finalRad) * turnRadius, 0, Mathf.Sin(finalRad) * turnRadius);
        cat.position = finalPos;
        cat.eulerAngles = new Vector3(0, targetAngle, 0);

        // ========== 第二阶段：往前走 + UI渐隐 + 上滑条出现 同时发生 ==========
        Vector3 walkStartPos = cat.position;
        Vector3 walkEndPos = walkStartPos + cat.forward * walkDistance;

        Debug.Log("猫开始往前走，同时触发 UI 渐隐和上滑条");

        // 同时启动三个协程
        StartCoroutine(ShowUnlockPanelAndFadeUI());

        elapsed = 0f;
        while (elapsed < walkDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / walkDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            cat.position = Vector3.Lerp(walkStartPos, walkEndPos, smoothT);
            yield return null;
        }

        cat.position = walkEndPos;

        // 猫走完后回到坐着状态
        if (catAnimator != null)
        {
            //catAnimator.Play("Sit");
            catAnimator.speed = 1f;
        }

        Debug.Log($"猫走完，最终位置: {walkEndPos}");
    }

    // 同时处理 UI 淡出和上滑条显示
    private IEnumerator ShowUnlockPanelAndFadeUI()
    {
        if (hasStartedWalking) yield break;
        hasStartedWalking = true;

        // 同时开始 UI 渐隐和上滑条显示
        StartCoroutine(FadeOutMainUI());

        // 稍微延迟一点点显示上滑条，让过渡更自然
        yield return new WaitForSeconds(0.1f);

        ShowUnlockPanel();
    }

    private IEnumerator FadeOutMainUI()
    {
        if (mainUICanvasGroup == null) yield break;

        float elapsed = 0f;
        float startAlpha = mainUICanvasGroup.alpha;

        Debug.Log("UI 开始渐隐");

        while (elapsed < uiFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / uiFadeDuration;
            mainUICanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        mainUICanvasGroup.alpha = 0f;
        mainUICanvasGroup.interactable = false;
        mainUICanvasGroup.blocksRaycasts = false;

        Debug.Log("UI 渐隐完成");
    }

    private void ShowUnlockPanel()
    {
        if (unlockPanel == null)
        {
            Debug.LogError("unlockPanel 未设置");
            return;
        }

        if (isUnlockActive) return;

        unlockPanel.SetActive(true);
        isUnlockActive = true;
        isSequenceReady = true;
        Debug.Log("上滑条已显示");
    }

    private void Update()
    {
        if (!isSequenceReady || !isUnlockActive) return;

        // 鼠标输入
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            float swipeDelta = Input.mousePosition.y - touchStartPos.y;
            if (swipeDelta > swipeThreshold)
            {
                OnUnlockSwipe();
            }
        }

        // 触摸输入
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float swipeDelta = touch.position.y - touchStartPos.y;
                if (swipeDelta > swipeThreshold)
                {
                    OnUnlockSwipe();
                }
            }
        }
    }

    private void OnUnlockSwipe()
    {
        if (!isUnlockActive) return;

        isUnlockActive = false;
        Debug.Log("上滑解锁！");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}
