using UnityEngine;
using UnityEngine.SceneManagement;
using CatLife.Core;
using CatLife.LLM;

namespace CatLife
{
    /// <summary>
    /// 游戏主管理器
    /// 协调各子系统，管理场景切换，应用生命周期事件
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("核心组件（自动查找）")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private BehaviorTracker behaviorTracker;
        [SerializeField] private DataManager dataManager;

        [Header("LLM 配置")]
        [SerializeField] private bool useMockLLM = true; // Editor 下用 Mock
        [SerializeField] private ILLMClient llmClient;

        [Header("场景配置")]
        [SerializeField] private string focusSceneName = "FocusScene";
        [SerializeField] private string mainMenuSceneName = "mainscene";

        // 是否初始化完成
        public bool IsReady { get; private set; } = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // 应用切入后台 = 放弃专注
                if (behaviorTracker != null)
                {
                    behaviorTracker.RecordAppSwitch();
                }

                // 通知 FocusStarter 放弃专注
                FocusStarter[] starters = FindObjectsOfType<FocusStarter>();
                foreach (var starter in starters)
                {
                    if (starter.IsFocusActive)
                    {
                        starter.EndFocus(false);
                        Debug.Log("[GameManager] 切App，专注已放弃");
                    }
                }
            }

            // 保存数据
            if (pauseStatus && dataManager != null)
            {
                dataManager.Save();
            }
        }

        private void OnApplicationQuit()
        {
            if (dataManager != null)
            {
                dataManager.Save();
            }

            if (llmClient != null)
            {
                llmClient.Release();
            }
        }

        /// <summary>
        /// 初始化所有子系统
        /// </summary>
        public void Initialize()
        {
            Debug.Log("[GameManager] 开始初始化...");

            // 1. 初始化 DataManager
            if (dataManager == null)
                dataManager = FindFirstObjectByType<DataManager>();
            if (dataManager == null)
            {
                dataManager = gameObject.AddComponent<DataManager>();
            }

            // 2. 初始化 BehaviorTracker
            if (behaviorTracker == null)
                behaviorTracker = FindFirstObjectByType<BehaviorTracker>();
            if (behaviorTracker == null)
            {
                behaviorTracker = gameObject.AddComponent<BehaviorTracker>();
            }

            // 3. 绑定 BehaviorTracker → StateMachine（自动专注检测）
            behaviorTracker.OnFocusStateChanged += (bool isFocused) =>
            {
                if (stateMachine == null) return;
                if (isFocused && stateMachine.CanEnterFocus())
                {
                    // 自动进入专注（autoFocusEnabled 时）
                    if (dataManager != null && dataManager.UserData.autoFocusEnabled)
                    {
                        stateMachine.SwitchState(CatState.Focus);
                        Debug.Log("[GameManager] 自动专注触发");
                    }
                }
                else if (!isFocused && stateMachine.IsState(CatState.Transition))
                {
                    // 恢复活跃，回到普通
                    stateMachine.SwitchState(CatState.Normal);
                }
            };

            // 4. 初始化 StateMachine
            if (stateMachine == null)
                stateMachine = FindObjectOfType<StateMachine>();
            if (stateMachine == null)
            {
                stateMachine = gameObject.AddComponent<StateMachine>();
            }

            // 4. 初始化 LLM Client
            InitializeLLM();

            IsReady = true;
            Debug.Log("[GameManager] 初始化完成");
        }

        /// <summary>
        /// 初始化 LLM 客户端
        /// </summary>
        private void InitializeLLM()
        {
#if UNITY_EDITOR
            // Editor 下使用 Mock
            if (useMockLLM || llmClient == null)
            {
                MockLLMClient mock = new MockLLMClient();
                llmClient = mock;
                Debug.Log("[GameManager] 使用 Mock LLM 客户端");
            }
#elif UNITY_ANDROID
            // Android 下使用 BlueLM
            if (llmClient == null)
            {
                llmClient = new BlueLLMClient();
            }
#else
            // 其他平台用 Mock
            MockLLMClient mock = gameObject.AddComponent<MockLLMClient>();
            llmClient = mock;
#endif

            llmClient.Initialize(ret =>
            {
                if (ret == 0)
                {
                    Debug.Log("[GameManager] LLM 初始化成功");
                }
                else
                {
                    Debug.LogWarning($"[GameManager] LLM 初始化失败，错误码: {ret}");
                }
            });
        }

        /// <summary>
        /// 场景加载完成回调
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[GameManager] 场景加载: {scene.name}");

            // 场景加载后重新查找引用
            if (stateMachine == null) stateMachine = FindFirstObjectByType<StateMachine>();
            if (behaviorTracker == null) behaviorTracker = FindFirstObjectByType<BehaviorTracker>();

            // 设置 LLM 客户端到 SmartFocusAnalyzer
            #pragma warning disable CS0618
            SmartFocusAnalyzer[] analyzers = FindObjectsOfType<SmartFocusAnalyzer>();
            #pragma warning restore CS0618
            foreach (var analyzer in analyzers)
            {
                analyzer.SetLLMClient(llmClient);
            }
        }

        /// <summary>
        /// 切换到专注场景
        /// </summary>
        public void LoadFocusScene()
        {
            SceneManager.LoadScene(focusSceneName);
        }

        /// <summary>
        /// 返回主场景
        /// </summary>
        public void LoadMainScene()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        /// <summary>
        /// 获取 LLM 客户端（供其他脚本使用）
        /// </summary>
        public ILLMClient GetLLMClient()
        {
            return llmClient;
        }

        /// <summary>
        /// 记录专注完成
        /// </summary>
        public void OnFocusComplete(int minutes)
        {
            dataManager?.RecordFocusSession(minutes);
        }
    }
}