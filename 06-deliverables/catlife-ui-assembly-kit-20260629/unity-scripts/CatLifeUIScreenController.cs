using UnityEngine;

namespace CatLife.UI
{
    public enum CatLifeUIScreen
    {
        Splash,
        MainTown,
        FocusSetup,
        FocusRunning,
        RewardSummary,
        Records,
        PrivacyLLM
    }

    public sealed class CatLifeUIScreenController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject splashPanel;
        [SerializeField] private GameObject mainTownPanel;
        [SerializeField] private GameObject focusSetupPanel;
        [SerializeField] private GameObject focusRunningPanel;
        [SerializeField] private GameObject rewardSummaryPanel;
        [SerializeField] private GameObject recordsPanel;
        [SerializeField] private GameObject privacyPanel;

        public CatLifeUIScreen Current { get; private set; }

        public void Show(CatLifeUIScreen screen)
        {
            Current = screen;
            SetActive(splashPanel, screen == CatLifeUIScreen.Splash);
            SetActive(mainTownPanel, screen == CatLifeUIScreen.MainTown);
            SetActive(focusSetupPanel, screen == CatLifeUIScreen.FocusSetup);
            SetActive(focusRunningPanel, screen == CatLifeUIScreen.FocusRunning);
            SetActive(rewardSummaryPanel, screen == CatLifeUIScreen.RewardSummary);
            SetActive(recordsPanel, screen == CatLifeUIScreen.Records);
            SetActive(privacyPanel, screen == CatLifeUIScreen.PrivacyLLM);
        }

        public void ShowSplash() => Show(CatLifeUIScreen.Splash);
        public void ShowMainTown() => Show(CatLifeUIScreen.MainTown);
        public void ShowFocusSetup() => Show(CatLifeUIScreen.FocusSetup);
        public void ShowFocusRunning() => Show(CatLifeUIScreen.FocusRunning);
        public void ShowRewardSummary() => Show(CatLifeUIScreen.RewardSummary);
        public void ShowRecords() => Show(CatLifeUIScreen.Records);
        public void ShowPrivacy() => Show(CatLifeUIScreen.PrivacyLLM);

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active)
            {
                target.SetActive(active);
            }
        }
    }
}
