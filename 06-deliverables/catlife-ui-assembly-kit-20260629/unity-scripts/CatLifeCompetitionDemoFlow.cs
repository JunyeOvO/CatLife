using UnityEngine;

namespace CatLife.UI
{
    public sealed class CatLifeCompetitionDemoFlow : MonoBehaviour
    {
        [SerializeField] private CatLifeUIScreenController screens;
        [SerializeField] private CatLifeCatChatBubbleController chatBubble;
        [SerializeField] private float splashSeconds = 1.2f;
        [SerializeField] private float focusPreviewSeconds = 4f;

        private float timer;
        private int step;

        private void Start()
        {
            if (screens != null)
            {
                screens.ShowSplash();
            }
        }

        private void Update()
        {
            if (screens == null) return;

            timer += Time.deltaTime;
            if (step == 0 && timer >= splashSeconds)
            {
                step = 1;
                timer = 0f;
                screens.ShowMainTown();
                ShowBubble(CatLifeCatBubbleState.Normal);
            }
            else if (step == 2 && timer >= focusPreviewSeconds)
            {
                step = 3;
                timer = 0f;
                screens.ShowRewardSummary();
                ShowBubble(CatLifeCatBubbleState.Reward, true);
            }
        }

        public void OnStartFocusClicked()
        {
            step = 2;
            timer = 0f;
            screens.ShowFocusRunning();
            ShowBubble(CatLifeCatBubbleState.Focus, true);
        }

        public void OnOpenFocusSetupClicked()
        {
            screens.ShowFocusSetup();
            ShowBubble(CatLifeCatBubbleState.Transition, true);
        }

        public void OnOpenRecordsClicked()
        {
            screens.ShowRecords();
            HideBubble();
        }

        public void OnOpenPrivacyClicked()
        {
            screens.ShowPrivacy();
            HideBubble();
        }

        public void OnBackToTownClicked()
        {
            step = 1;
            timer = 0f;
            screens.ShowMainTown();
            ShowBubble(CatLifeCatBubbleState.Normal, true);
        }

        public void OnHighDistractionDetected()
        {
            screens.ShowMainTown();
            ShowBubble(CatLifeCatBubbleState.DistractionNudge);
        }

        private void ShowBubble(CatLifeCatBubbleState state, bool force = false)
        {
            if (chatBubble != null)
            {
                chatBubble.ShowForState(state, force);
            }
        }

        private void HideBubble()
        {
            if (chatBubble != null)
            {
                chatBubble.Hide();
            }
        }
    }
}
