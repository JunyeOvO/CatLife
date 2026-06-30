using TMPro;
using UnityEngine;

namespace CatLife.UI
{
    public enum CatLifeCatBubbleState
    {
        Normal,
        Transition,
        Focus,
        Reward,
        DistractionNudge
    }

    public sealed class CatLifeCatChatBubbleController : MonoBehaviour
    {
        [Header("Scene Anchor")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Transform catAnchor;
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.35f, 0f);
        [SerializeField] private Vector2 screenOffset = new Vector2(0f, -110f);

        [Header("UI")]
        [SerializeField] private RectTransform bubbleRoot;
        [SerializeField] private TMP_Text bubbleText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Vector2 clampPadding = new Vector2(56f, 160f);

        [Header("Timing")]
        [SerializeField] private float defaultVisibleSeconds = 4.5f;
        [SerializeField] private float cooldownSeconds = 8f;
        [SerializeField] private bool showNormalOnStart;

        [Header("Messages")]
        [SerializeField] private string normalMessage = "先不用急，我在这里。";
        [SerializeField] private string transitionMessage = "你慢下来了，我也安静一点。";
        [SerializeField] private string focusMessage = "我会轻轻陪着你，不打扰。";
        [SerializeField] private string rewardMessage = "完成啦，猫咪给你一个小爪印。";
        [SerializeField] private string distractionNudgeMessage = "要不要回到刚才那件事？";

        private float hideAt = -1f;
        private float nextAllowedAt;

        private void Awake()
        {
            if (rootCanvas == null)
            {
                rootCanvas = GetComponentInParent<Canvas>();
            }

            if (bubbleRoot == null)
            {
                bubbleRoot = transform as RectTransform;
            }

            if (canvasGroup == null && bubbleRoot != null)
            {
                canvasGroup = bubbleRoot.GetComponent<CanvasGroup>();
            }

            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            HideImmediate();
        }

        private void Start()
        {
            if (showNormalOnStart)
            {
                ShowForState(CatLifeCatBubbleState.Normal, true);
            }
        }

        private void LateUpdate()
        {
            FollowCat();

            if (hideAt > 0f && Time.unscaledTime >= hideAt)
            {
                Hide();
            }
        }

        public void ShowForState(CatLifeCatBubbleState state)
        {
            ShowForState(state, false);
        }

        public void ShowForState(CatLifeCatBubbleState state, bool force)
        {
            ShowMessage(MessageForState(state), defaultVisibleSeconds, force);
        }

        public void ShowMessage(string message)
        {
            ShowMessage(message, defaultVisibleSeconds, false);
        }

        public void ShowMessage(string message, float visibleSeconds, bool force = false)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (!force && Time.unscaledTime < nextAllowedAt)
            {
                return;
            }

            if (bubbleText != null)
            {
                bubbleText.text = message;
            }

            if (bubbleRoot != null)
            {
                bubbleRoot.gameObject.SetActive(true);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            hideAt = Time.unscaledTime + Mathf.Max(0.5f, visibleSeconds);
            nextAllowedAt = Time.unscaledTime + Mathf.Max(0f, cooldownSeconds);
            FollowCat();
        }

        public void Hide()
        {
            hideAt = -1f;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            if (bubbleRoot != null)
            {
                bubbleRoot.gameObject.SetActive(false);
            }
        }

        public void HideImmediate()
        {
            nextAllowedAt = 0f;
            Hide();
        }

        private string MessageForState(CatLifeCatBubbleState state)
        {
            switch (state)
            {
                case CatLifeCatBubbleState.Transition:
                    return transitionMessage;
                case CatLifeCatBubbleState.Focus:
                    return focusMessage;
                case CatLifeCatBubbleState.Reward:
                    return rewardMessage;
                case CatLifeCatBubbleState.DistractionNudge:
                    return distractionNudgeMessage;
                default:
                    return normalMessage;
            }
        }

        private void FollowCat()
        {
            if (rootCanvas == null || bubbleRoot == null || catAnchor == null || worldCamera == null)
            {
                return;
            }

            RectTransform canvasRect = rootCanvas.transform as RectTransform;
            if (canvasRect == null)
            {
                return;
            }

            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCamera, catAnchor.position + worldOffset);
            screenPoint.x += screenOffset.x;
            screenPoint.y += screenOffset.y;

            Camera uiCamera = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, uiCamera, out Vector2 localPoint))
            {
                return;
            }

            Vector2 half = canvasRect.rect.size * 0.5f;
            localPoint.x = Mathf.Clamp(localPoint.x, -half.x + clampPadding.x, half.x - clampPadding.x);
            localPoint.y = Mathf.Clamp(localPoint.y, -half.y + clampPadding.y, half.y - clampPadding.y);
            bubbleRoot.anchoredPosition = localPoint;
        }
    }
}
