using UnityEngine;

namespace CatLife.UI
{
    public sealed class CatLifeCompetitionDemoFlow : MonoBehaviour
    {
        [SerializeField] private CatLifeUIScreenController screens;
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
            }
            else if (step == 2 && timer >= focusPreviewSeconds)
            {
                step = 3;
                timer = 0f;
                screens.ShowRewardSummary();
            }
        }

        public void OnStartFocusClicked()
        {
            step = 2;
            timer = 0f;
            screens.ShowFocusRunning();
        }

        public void OnOpenFocusSetupClicked()
        {
            screens.ShowFocusSetup();
        }

        public void OnOpenRecordsClicked()
        {
            screens.ShowRecords();
        }

        public void OnOpenPrivacyClicked()
        {
            screens.ShowPrivacy();
        }

        public void OnBackToTownClicked()
        {
            step = 1;
            timer = 0f;
            screens.ShowMainTown();
        }
    }
}
