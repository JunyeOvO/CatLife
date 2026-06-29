#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections.Generic;

namespace CatLife.EditorSetup
{
    /// <summary>
    /// 运行一次即可：自动创建所有主场景UI元素，绑定FocusStarter
    /// 执行完后自动删除本脚本
    /// 菜单路径：CatLife → Setup Main Scene UI
    /// </summary>
    public class MainSceneSetup : MonoBehaviour
    {
        [MenuItem("CatLife/Setup Main Scene UI")]
        public static void Run()
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(scene.path))
            {
                Debug.LogError("[Setup] 请先保存场景再运行此脚本");
                return;
            }

            // 1. 找到关键对象
            var canvasObj = GameObject.Find("主画布");
            if (canvasObj == null) { Debug.LogError("[Setup] 未找到'主画布'"); return; }
            var canvas = canvasObj.GetComponent<Canvas>();
            if (canvas == null) { Debug.LogError("[Setup] 主画布没有Canvas组件"); return; }

            // 2. 获取或创建 FocusStarter
            var mainSceneMgr = GameObject.Find("MainSceneManager");
            if (mainSceneMgr == null) { Debug.LogError("[Setup] 未找到MainSceneManager"); return; }
            var focusStarter = mainSceneMgr.GetComponent<CatLife.Core.FocusStarter>();
            if (focusStarter == null) focusStarter = mainSceneMgr.AddComponent<CatLife.Core.FocusStarter>();

            var toHide = new List<GameObject>();

            // ==============================================================
            // 3. 创建"开始专注"按钮
            // ==============================================================
            var startBtn = CreateButton(canvasObj.transform, "Btn_StartFocus",
                new Vector2(0.5f, 0.35f), new Vector2(320, 120),
                new Vector2(0, 0), "开始专注", 36);
            startBtn.GetComponent<Button>().onClick.AddListener(() => focusStarter.StartFocus());
            toHide.Add(startBtn);
            Debug.Log($"[Setup] 创建: Btn_StartFocus");

            // ==============================================================
            // 4. 创建"今日累计"文本（左下角）
            // ==============================================================
            var todayTxt = CreateText(canvasObj.transform, "Txt_TodayMinutes",
                new Vector2(0.08f, 0.06f), new Vector2(300, 50),
                new Vector2(0, 0), "今日 0 分钟", 28, Color.white);
            Debug.Log($"[Setup] 创建: Txt_TodayMinutes");

            // ==============================================================
            // 5. 创建奖励弹窗 Panel_Reward（初始隐藏）
            // ==============================================================
            var rewardPanel = CreatePanel(canvasObj.transform, "Panel_Reward",
                new Vector2(0.5f, 0.5f), new Vector2(600, 400),
                new Vector2(0, 0), new Color(0, 0, 0, 0.85f));
            rewardPanel.SetActive(false);

            // 标题
            CreateText(rewardPanel.transform, "RewardTitle",
                new Vector2(0.5f, 0.72f), new Vector2(400, 60),
                new Vector2(0, 0), "🎉 专注完成！", 42, new Color(1f, 0.95f, 0.4f));

            // 本次分钟
            CreateText(rewardPanel.transform, "MinutesText",
                new Vector2(0.5f, 0.55f), new Vector2(400, 50),
                new Vector2(0, 0), "本次: 0 分钟", 32, Color.white);

            // 金币
            CreateText(rewardPanel.transform, "CoinsText",
                new Vector2(0.5f, 0.42f), new Vector2(400, 50),
                new Vector2(0, 0), "+0 金币", 32, new Color(1f, 0.8f, 0.2f));

            // 关闭按钮
            var closeBtn = CreateButton(rewardPanel.transform, "Btn_CloseReward",
                new Vector2(0.5f, 0.18f), new Vector2(200, 70),
                new Vector2(0, 0), "确定", 28);
            closeBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                rewardPanel.SetActive(false);
                var sm = Object.FindFirstObjectByType<CatLife.Core.StateMachine>();
                if (sm != null) sm.SwitchState(CatLife.Core.CatState.Normal);
            });

            focusStarter.rewardPanel = rewardPanel;
            Debug.Log($"[Setup] 创建: Panel_Reward");

            // ==============================================================
            // 6. 创建上滑退出提示 SwipeHint（初始隐藏）
            // ==============================================================
            var swipeHint = CreateText(canvasObj.transform, "SwipeHint",
                new Vector2(0.5f, 0.08f), new Vector2(400, 60),
                new Vector2(0, 0), "↑ 上滑退出 ↑", 30, new Color(1f, 1f, 1f, 0.6f));
            swipeHint.SetActive(false);
            focusStarter.swipeHint = swipeHint;
            Debug.Log($"[Setup] 创建: SwipeHint");

            // ==============================================================
            // 7. 创建商店 Panel_Shop（初始隐藏）
            // ==============================================================
            var shopPanel = CreatePanel(canvasObj.transform, "Panel_Shop",
                new Vector2(0.5f, 0.5f), new Vector2(700, 500),
                new Vector2(0, 0), new Color(0.1f, 0.12f, 0.2f, 0.95f));
            shopPanel.SetActive(false);

            CreateText(shopPanel.transform, "ShopTitle",
                new Vector2(0.5f, 0.88f), new Vector2(300, 50),
                new Vector2(0, 0), "商店", 38, Color.white);

            // 猫粮按钮
            var foodBtn = CreateButton(shopPanel.transform, "Btn_Food",
                new Vector2(0.3f, 0.55f), new Vector2(220, 160),
                new Vector2(0, 0), "🐟 猫粮\n10金币", 26);
            foodBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                var dm = CatLife.Core.DataManager.Instance;
                if (dm != null && dm.FeedCat(10))
                    Debug.Log("[Shop] 购买猫粮成功");
                else
                    Debug.Log("[Shop] 金币不足");
            });

            // 玩具按钮
            var toyBtn = CreateButton(shopPanel.transform, "Btn_Toy",
                new Vector2(0.7f, 0.55f), new Vector2(220, 160),
                new Vector2(0, 0), "🧶 玩具\n20金币", 26);
            toyBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                var dm = CatLife.Core.DataManager.Instance;
                if (dm != null && dm.PlayWithCat(20))
                    Debug.Log("[Shop] 购买玩具成功");
                else
                    Debug.Log("[Shop] 金币不足");
            });

            // 返回按钮
            var shopBackBtn = CreateButton(shopPanel.transform, "Btn_ShopBack",
                new Vector2(0.5f, 0.1f), new Vector2(180, 60),
                new Vector2(0, 0), "返回", 24);
            shopBackBtn.GetComponent<Button>().onClick.AddListener(() => shopPanel.SetActive(false));

            toHide.Add(shopPanel);
            Debug.Log($"[Setup] 创建: Panel_Shop");

            // ==============================================================
            // 8. 创建成就 Panel_Achievement（初始隐藏）
            // ==============================================================
            var achPanel = CreatePanel(canvasObj.transform, "Panel_Achievement",
                new Vector2(0.5f, 0.5f), new Vector2(700, 500),
                new Vector2(0, 0), new Color(0.1f, 0.12f, 0.2f, 0.95f));
            achPanel.SetActive(false);

            CreateText(achPanel.transform, "AchTitle",
                new Vector2(0.5f, 0.88f), new Vector2(300, 50),
                new Vector2(0, 0), "成就", 38, Color.white);

            string[] achievNames = { "首次专注", "10分钟专注", "半小时专注", "今日学习者" };
            string[] achievEmoji = { "🌟", "⏱️", "🏆", "📖" };
            for (int i = 0; i < achievNames.Length; i++)
            {
                float x = (i % 2 == 0) ? 0.3f : 0.7f;
                float y = 0.55f - (i / 2) * 0.28f;
                var achBtn = CreateButton(achPanel.transform, $"AchBtn_{i}",
                    new Vector2(x, y), new Vector2(240, 120),
                    new Vector2(0, 0), $"{achievEmoji[i]}\n{achievNames[i]}", 24);
            }

            var achBackBtn = CreateButton(achPanel.transform, "Btn_AchBack",
                new Vector2(0.5f, 0.1f), new Vector2(180, 60),
                new Vector2(0, 0), "返回", 24);
            achBackBtn.GetComponent<Button>().onClick.AddListener(() => achPanel.SetActive(false));

            toHide.Add(achPanel);
            Debug.Log($"[Setup] 创建: Panel_Achievement");

            // ==============================================================
            // 9. 创建设置 Panel_Settings（初始隐藏）
            // ==============================================================
            var setPanel = CreatePanel(canvasObj.transform, "Panel_Settings",
                new Vector2(0.5f, 0.5f), new Vector2(700, 500),
                new Vector2(0, 0), new Color(0.1f, 0.12f, 0.2f, 0.95f));
            setPanel.SetActive(false);

            CreateText(setPanel.transform, "SetTitle",
                new Vector2(0.5f, 0.88f), new Vector2(300, 50),
                new Vector2(0, 0), "设置", 38, Color.white);

            // 专注时长设置
            CreateText(setPanel.transform, "SetDurationLabel",
                new Vector2(0.3f, 0.7f), new Vector2(200, 40),
                new Vector2(0, 0), "专注时长（分钟）", 24, Color.white);

            var sliderObj = new GameObject("Slider_Duration");
            sliderObj.transform.SetParent(setPanel.transform);
            var sliderRT = sliderObj.AddComponent<RectTransform>();
            sliderRT.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRT.anchorMax = new Vector2(0.5f, 0.5f);
            sliderRT.anchoredPosition = new Vector2(0, -40);
            sliderRT.sizeDelta = new Vector2(300, 30);
            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 5;
            slider.maxValue = 120;
            slider.value = 25;
            slider.onValueChanged.AddListener((v) => focusStarter.SetTimerMinutes(v));

            var setBackBtn = CreateButton(setPanel.transform, "Btn_SetBack",
                new Vector2(0.5f, 0.1f), new Vector2(180, 60),
                new Vector2(0, 0), "返回", 24);
            setBackBtn.GetComponent<Button>().onClick.AddListener(() => setPanel.SetActive(false));

            toHide.Add(setPanel);
            Debug.Log($"[Setup] 创建: Panel_Settings");

            // ==============================================================
            // 10. 绑定 HideOnFocus 数组
            // ==============================================================
            // 将现有的4个按钮加入隐藏列表
            var settingsBtn = GameObject.Find("设置按钮");
            var catBtn = GameObject.Find("小猫按钮");
            var soundBtn = GameObject.Find("喇叭按钮");
            var imgBtn = GameObject.Find("图片按钮");
            if (settingsBtn) toHide.Add(settingsBtn);
            if (catBtn) toHide.Add(catBtn);
            if (soundBtn) toHide.Add(soundBtn);
            if (imgBtn) toHide.Add(imgBtn);

            // 反射设置 FocusStarter.hideOnFocus
            var listType = typeof(List<GameObject>);
            var hideField = typeof(CatLife.Core.FocusStarter).GetField("hideOnFocus",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (hideField != null)
            {
                var list = toHide.FindAll(x => x != null);
                hideField.SetValue(focusStarter, list.ToArray());
            }

            Debug.Log($"[Setup] 绑定 HideOnFocus: {toHide.Count} 个对象");

            // ==============================================================
            // 11. 连接现有的商店/成就/设置按钮（如果有）
            // ==============================================================
            // 商店按钮
            var shopBtnObj = GameObject.Find("图片按钮"); // 暂用图片按钮当商店入口
            if (shopBtnObj != null)
            {
                var btn = shopBtnObj.GetComponent<Button>();
                if (btn != null) btn.onClick.AddListener(() => shopPanel.SetActive(true));
            }

            Debug.Log("[Setup] 完成！所有UI元素已创建。");
            Debug.Log("[Setup] 注意：请手动设置各Panel的子元素布局，并验证按钮绑定。");

            // 12. 删除本脚本
            UnityEditor.EditorUtility.SetDirty(mainSceneMgr);
            UnityEditor.Undo.DestroyObjectImmediate(mainSceneMgr.GetComponent<MainSceneSetup>());
        }

        // ==============================================================
        // 辅助方法
        // ==============================================================

        static GameObject CreateButton(Transform parent, string name,
            Vector2 anchor, Vector2 size, Vector2 pivot, string label, int fontSize = 28)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = size;

            var img = go.AddComponent<Image>();
            img.color = new Color(0.3f, 0.5f, 0.9f, 1f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = img;
            button.transition = Selectable.Transition.ColorTint;

            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(go.transform);
            var txtRT = txtObj.AddComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.pivot = new Vector2(0.5f, 0.5f);
            txtRT.anchoredPosition = Vector2.zero;
            txtRT.sizeDelta = Vector2.zero;
            var txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = fontSize;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;

            return go;
        }

        static GameObject CreateText(Transform parent, string name,
            Vector2 anchor, Vector2 size, Vector2 pivot, string text, int fontSize = 28, Color? color = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = size;

            var txt = go.AddComponent<Text>();
            txt.text = text;
            txt.fontSize = fontSize;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = color ?? Color.white;

            go.AddComponent<CanvasRenderer>();

            return go;
        }

        static GameObject CreatePanel(Transform parent, string name,
            Vector2 anchor, Vector2 size, Vector2 pivot, Color bgColor)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = pivot;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = size;

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            go.AddComponent<CanvasRenderer>();

            return go;
        }
    }
}
#endif
