using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using CatLife.Core;
using CatLife.Cat;

public class CatSceneSetup : EditorWindow
{
    [MenuItem("CatLife/自动放置猫模型到场景")]
    public static void ShowWindow()
    {
        GetWindow<CatSceneSetup>("放置猫模型");
    }

    public void OnGUI()
    {
        GUILayout.Label("自动在 startscene 里放置猫模型", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

        if (GUILayout.Button("执行放置", GUILayout.Height(40)))
        {
            PlaceCatInScene();
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "此操作会：\n" +
            "1. 确保 startscene 已加载\n" +
            "2. 创建或替换 Cat 对象\n" +
            "3. 自动挂载 CatController\n" +
            "4. 配置 Animator",
            MessageType.Info);
    }

    [MenuItem("CatLife/替换场景中的猫模型")]
    public static void ReplaceExistingCat()
    {
        PlaceCatInScene();
    }

    public static void PlaceCatInScene()
    {
        // 1. 确保 startscene 已打开
        string startscenePath = "Assets/Scenes/startscene.unity";
        var activeScene = EditorSceneManager.GetActiveScene();

        if (activeScene.path != startscenePath)
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(startscenePath);
            Debug.Log("[CatSceneSetup] 已打开 startscene");
        }

        // 2. 找到或创建 Cat 父对象
        GameObject catRoot = GameObject.Find("Cat");
        if (catRoot == null)
        {
            catRoot = new GameObject("Cat");
            Debug.Log("[CatSceneSetup] 创建了新 Cat 对象");
        }

        // 3. 删除已有的子对象（清理旧模型）
        while (catRoot.transform.childCount > 0)
        {
            DestroyImmediate(catRoot.transform.GetChild(0).gameObject);
        }

        // 4. 加载模型 Prefab 或 Instance
        string modelPath = "Assets/Art/Cat/Meshy_AI_Low_Poly_Orange_Cat_quadruped_Character_output.fbx";
        GameObject catModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);

        if (catModelPrefab == null)
        {
            Debug.LogError("[CatSceneSetup] 找不到模型: " + modelPath);
            EditorUtility.DisplayDialog("错误", "找不到猫模型文件！\n路径: " + modelPath, "确定");
            return;
        }

        // 5. 实例化模型
        GameObject instance = PrefabUtility.InstantiatePrefab(catModelPrefab, catRoot.transform) as GameObject;
        if (instance == null)
        {
            // 如果 PrefabUtility 失败，直接 GameObject.Instantiate
            instance = Instantiate(catModelPrefab, catRoot.transform);
            instance.name = catModelPrefab.name;
        }

        // 重置位置
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        Debug.Log($"[CatSceneSetup] 模型已放置: {instance.name}");

        // 6. 添加 CatController（如果没有）
        CatController controller = catRoot.GetComponent<CatController>();
        if (controller == null)
        {
            controller = catRoot.AddComponent<CatController>();
            Debug.Log("[CatSceneSetup] 已添加 CatController");
        }

        // 7. 添加或配置 Animator
        Animator animator = catRoot.GetComponent<Animator>();
        if (animator == null)
        {
            animator = catRoot.AddComponent<Animator>();
        }

        // 设置为 Generic（Low Poly 猫不是人形动画）
        // Animator Controller 需要手动创建并拖入，这里只给占位
        Debug.Log("[CatSceneSetup] Animator 已挂上，需手动配置 Animator Controller");

        // 8. 尝试设置材质（如果有的话）
        SetupMaterials(instance);

        // 9. 标记场景已修改
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("[CatSceneSetup] 完成！请在 Inspector 里检查 Cat 对象。");

        Selection.activeGameObject = catRoot;
        EditorGUIUtility.PingObject(catRoot);
    }

    static void SetupMaterials(GameObject root)
    {
        // 尝试找到贴图并应用
        string texPath = "Assets/Art/Cat/Meshy_AI_Low_Poly_Orange_Cat_quadruped_texture_0.png";
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

        if (tex == null)
        {
            Debug.LogWarning("[CatSceneSetup] 未找到贴图，跳过材质设置");
            return;
        }

        // 遍历所有 MeshRenderer
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in renderers)
        {
            if (mr.sharedMaterial == null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.mainTexture = tex;
                mr.material = mat;
                Debug.Log($"[CatSceneSetup] 已设置材质: {mr.gameObject.name}");
            }
        }
    }
}
