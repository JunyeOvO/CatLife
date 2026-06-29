#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using CatLife.UI;

namespace CatLife.Editor
{
    /// <summary>
    /// FocusScene 场景自动搭建工具
    /// 菜单: CatLife → Setup FocusScene
    /// </summary>
    public static class FocusSceneSetup
    {
        /// 尝试加载中文字体（微软雅黑 MSYH SDF）
        private static TMP_FontAsset GetChineseFont()
        {
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/font/MSYH SDF.asset");
            if (font != null) return font;

            // 回退：尝试从 Resources 加载
            font = Resources.Load<TMP_FontAsset>("Fonts & Materials/MSYH SDF");
            return font;
        }

        [MenuItem("CatLife/Setup FocusScene")]
        public static void SetupFocusScene()
        {
            // 0. 处理未保存的场景：保存或强制清空
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.isDirty)
            {
                if (activeScene.IsValid() && !string.IsNullOrEmpty(activeScene.path))
                {
                    EditorSceneManager.SaveOpenScenes();
                }
                else
                {
                    // 无效/无路径 + dirty，关闭不保存，让 Unity 重新创建干净默认场景
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                }
            }

            // 1. 创建新场景（Single 模式，直接用干净场景）
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // 等待场景加载
            string scenePath = "Assets/Scenes/FocusScene.unity";

            // 2. 创建 Canvas
            GameObject canvasObj = CreateCanvas();

            // 3. 创建背景
            GameObject background = CreateBackground(canvasObj.transform);

            // 4. 创建状态文字
            GameObject statusText = CreateStatusText(canvasObj.transform);

            // 5. 创建计时器
            GameObject timerText = CreateTimerText(canvasObj.transform);

            // 6. 创建进度条
            GameObject progressBar = CreateProgressBar(canvasObj.transform);

            // 7. 创建上滑退出提示
            GameObject swipeHint = CreateSwipeHint(canvasObj.transform);

            // 8. 创建退出按钮（备用）
            GameObject exitButton = CreateExitButton(canvasObj.transform);

            // 9. 添加组件到 Canvas
            FocusUIManager focusUI = canvasObj.AddComponent<FocusUIManager>();
            FocusSceneController sceneController = canvasObj.AddComponent<FocusSceneController>();

            // 10. 创建 GameManager（如果不存在）
            CreateGameManager();

            // 11. 配置 FocusUIManager 引用
            ConfigureFocusUIManager(focusUI, sceneController, exitButton, swipeHint);

            // 12. 配置 FocusSceneController 引用
            ConfigureSceneController(sceneController, statusText, timerText, progressBar, exitButton);

            // 13. 保存场景
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);

            Debug.Log("[FocusSceneSetup] FocusScene 搭建完成！场景路径: " + scenePath);
            Debug.Log("[FocusSceneSetup] 请在 Unity Editor 中检查并绑定缺失的引用！");

            Selection.activeGameObject = canvasObj;
        }

        private static GameObject CreateCanvas()
        {
            // 使用 GameObject.UI 方法创建 Canvas
            GameObject canvasObj = new GameObject("FocusCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // 设置 RectTransform
            RectTransform rect = canvasObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            return canvasObj;
        }

        private static GameObject CreateBackground(Transform parent)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(parent);

            Image img = bg.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.18f, 1f); // 深蓝黑色 #1A1A2E

            RectTransform rect = bg.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            return bg;
        }

        private static GameObject CreateStatusText(Transform parent)
        {
            GameObject go = new GameObject("StatusText");
            go.transform.SetParent(parent);

            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
            text.text = "专注中...";
            text.fontSize = 48;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.font = GetChineseFont();

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, 100);
            rect.sizeDelta = new Vector2(800, 100);

            return go;
        }

        private static GameObject CreateTimerText(Transform parent)
        {
            GameObject go = new GameObject("TimerText");
            go.transform.SetParent(parent);

            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
            text.text = "00:00";
            text.fontSize = 72;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.font = GetChineseFont();

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.7f);
            rect.anchorMax = new Vector2(0.5f, 0.7f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(500, 120);

            return go;
        }

        private static GameObject CreateProgressBar(Transform parent)
        {
            // 创建进度条 Slider
            GameObject sliderObj = new GameObject("ProgressBar");
            sliderObj.transform.SetParent(parent);

            // Background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(sliderObj.transform);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(1, 1, 1, 0.2f);

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.1f, 0.45f);
            bgRect.anchorMax = new Vector2(0.9f, 0.5f);
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0);
            fillAreaRect.anchorMax = new Vector2(1, 1);
            fillAreaRect.sizeDelta = new Vector2(-4, 0);
            fillAreaRect.anchoredPosition = Vector2.zero;

            // Fill
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform);
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.26f, 0.75f, 0.85f, 1f); // #43BFD9

            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0.5f, 1);
            fillRect.sizeDelta = Vector2.zero;
            fillRect.anchoredPosition = Vector2.zero;

            // Slider 组件
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.navigation = new Navigation { mode = Navigation.Mode.None };
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.1f, 0.35f);
            sliderRect.anchorMax = new Vector2(0.9f, 0.45f);
            sliderRect.sizeDelta = Vector2.zero;

            return sliderObj;
        }

        private static GameObject CreateSwipeHint(Transform parent)
        {
            GameObject go = new GameObject("SwipeHint");
            go.transform.SetParent(parent);

            TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
            text.text = "↑ 上滑退出专注";
            text.fontSize = 24;
            text.color = new Color(0.53f, 0.53f, 0.53f, 1f); // #888888
            text.alignment = TextAlignmentOptions.Center;
            text.font = GetChineseFont();

            // 添加悬浮动画
            UnlockBarAnimation anim = go.AddComponent<UnlockBarAnimation>();
            anim.floatHeight = 15f;
            anim.floatSpeed = 2f;

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.1f);
            rect.anchorMax = new Vector2(0.5f, 0.1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(400, 50);

            return go;
        }

        private static GameObject CreateExitButton(Transform parent)
        {
            GameObject go = new GameObject("ExitButton");
            go.transform.SetParent(parent);

            // Button 背景
            Image bg = go.AddComponent<Image>();
            bg.color = new Color(1, 1, 1, 0.1f);

            // Button
            Button btn = go.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;

            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(go.transform);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "退出专注";
            text.fontSize = 28;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.font = GetChineseFont();

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.2f);
            rect.anchorMax = new Vector2(0.5f, 0.2f);
            rect.anchoredPosition = new Vector2(0, -50);
            rect.sizeDelta = new Vector2(200, 60);

            return go;
        }

        private static void CreateGameManager()
        {
            // 检查是否已存在
            GameObject existing = GameObject.Find("GameManager");
            if (existing != null)
            {
                if (existing.GetComponent<CatLife.GameManager>() == null)
                    existing.AddComponent<CatLife.GameManager>();
                return;
            }

            GameObject go = new GameObject("GameManager");
            go.AddComponent<CatLife.GameManager>();
        }

        private static void ConfigureFocusUIManager(FocusUIManager focusUI, FocusSceneController sceneController, GameObject exitButton, GameObject swipeHint)
        {
            // FocusUIManager 配置
            // 这些需要用户在 Inspector 中手动绑定，因为组件引用需要 GameObject 引用
            // 我们在这里只是记录需要绑定的字段名称

            Debug.Log("[Setup] FocusUIManager 需要绑定的字段:");
            Debug.Log("  - Focus Hint Object: StatusText (专注中...)");
            Debug.Log("  - Swipe Up Hint Object: SwipeHint (上滑退出)");
            Debug.Log("  - Exit Button: ExitButton");
        }

        private static void ConfigureSceneController(FocusSceneController controller, GameObject statusText, GameObject timerText, GameObject progressBar, GameObject exitButton)
        {
            Debug.Log("[Setup] FocusSceneController 需要绑定的字段:");
            Debug.Log("  - Status Text: StatusText");
            Debug.Log("  - Timer Text: TimerText");
            Debug.Log("  - Progress Fill Image: ProgressBar/Fill");
            Debug.Log("  - Exit Button: ExitButton");
        }
    }
}
#endif
