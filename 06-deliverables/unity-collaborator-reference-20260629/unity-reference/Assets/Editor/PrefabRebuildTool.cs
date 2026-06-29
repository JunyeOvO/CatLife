using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

public class PrefabRebuildTool : EditorWindow
{
    [MenuItem("CatLife/Rebuild Missing Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<PrefabRebuildTool>("Rebuild Prefabs");
    }

    void OnGUI()
    {
        GUILayout.Label("重建缺失的 Prefab 引用", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);
        GUILayout.Label("操作步骤：");
        GUILayout.Label("1. 重新导入 GLB 资源（生成 .meta）");
        GUILayout.Label("2. 为 Meshy 和 catlife 创建 Prefab 变体");
        GUILayout.Label("3. 在 mainscene 中替换损坏的 PrefabInstance");
        GUILayout.Label("4. 修复 MainSceneController 的 catModel/catAnimator 引用");
        EditorGUILayout.Space(10);
        if (GUILayout.Button("执行重建", GUILayout.Height(35)))
        {
            Rebuild();
        }
    }

    [MenuItem("CatLife/Rebuild Missing Prefabs (Auto)")]
    public static void RebuildAuto()
    {
        var scene = EditorSceneManager.GetSceneByPath("Assets/Scenes/mainscene.unity");
        if (!scene.IsValid())
        {
            Debug.LogError("mainscene 未打开！");
            return;
        }
        DoRebuild(scene);
    }

    static void Rebuild()
    {
        var scene = EditorSceneManager.GetSceneByPath("Assets/Scenes/mainscene.unity");
        if (!scene.IsValid())
        {
            Debug.LogError("mainscene 未打开！请先打开它。");
            return;
        }
        DoRebuild(scene);
    }

    static void DoRebuild(UnityEngine.SceneManagement.Scene scene)
    {
        Debug.Log("=== 开始重建 Prefab ===");

        // Step 1: 确保 GLB 被重新导入
        string[] glbPaths = {
            "Assets/Meshy_AI_model_Animation_Walking_withSkin.glb",
            "Assets/catlife_backup_1.1GB.glb"
        };

        foreach (var p in glbPaths)
        {
            if (File.Exists(p))
            {
                AssetDatabase.ImportAsset(p, ImportAssetOptions.ForceUpdate);
                Debug.Log($"重新导入: {p}");
            }
            else
            {
                Debug.LogWarning($"文件不存在: {p}");
            }
        }

        // Step 2: 加载 GLB 资源
        var meshyGO = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshy_AI_model_Animation_Walking_withSkin.glb");
        var catlifeGO = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/catlife_backup_1.1GB.glb");

        if (meshyGO == null) { Debug.LogError("Meshy GLB 加载失败"); return; }
        if (catlifeGO == null) { Debug.LogError("catlife GLB 加载失败"); return; }

        // Step 3: 找到带 SkinnedMeshRenderer 的 Meshy 根对象
        var meshyRoot = FindMeshyRoot(meshyGO);

        // Step 4: 创建 Prefab
        var meshyPrefab = CreateOrUpdatePrefab(meshyRoot, "Assets/Art/Cat/Meshy_Cat_Prefab.prefab");
        var catlifePrefab = CreateOrUpdatePrefab(catlifeGO, "Assets/catlife_Prefab.prefab");

        if (meshyPrefab == null || catlifePrefab == null) return;

        // Step 5: 收集场景中所有 PrefabInstance，标记要替换的
        var rootObjs = scene.GetRootGameObjects();
        var replacements = new System.Collections.Generic.List<System.Tuple<GameObject, GameObject>>();

        foreach (var ro in rootObjs)
        {
            if (PrefabUtility.GetPrefabInstanceStatus(ro) != PrefabInstanceStatus.NotAPrefab)
            {
                var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ro);
                GameObject newPrefab = null;

                if (assetPath.Contains("Meshy") || assetPath.Contains("724e256a"))
                    newPrefab = meshyPrefab;
                else if (assetPath.Contains("catlife") || assetPath.Contains("122fd062"))
                    newPrefab = catlifePrefab;

                if (newPrefab != null)
                {
                    replacements.Add(new System.Tuple<GameObject, GameObject>(ro, newPrefab));
                }
            }
        }

        Debug.Log($"找到 {replacements.Count} 个 PrefabInstance 需要替换");

        // Step 6: 替换（先实例化新的，再销毁旧的）
        foreach (var replacement in replacements)
        {
            var oldObj = replacement.Item1;
            var newPrefab = replacement.Item2;
            var parent = oldObj.transform.parent;
            var pos = oldObj.transform.localPosition;
            var rot = oldObj.transform.localRotation;
            var scale = oldObj.transform.localScale;

            var newObj = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab, scene);
            newObj.transform.SetParent(parent);
            newObj.transform.localPosition = pos;
            newObj.transform.localRotation = rot;
            newObj.transform.localScale = scale;

            Debug.Log($"替换 PrefabInstance: {oldObj.name} -> {newPrefab.name}");
            UnityEngine.Object.DestroyImmediate(oldObj);
        }

        // Step 7: 修复 MainSceneController 的 catModel / catAnimator
        // 注意：Prefab 实例化后，需在 Inspector 手动拖入 catModel（猫根对象）和 catAnimator 组件
        // 下面这一步仅作提示用，实际引用需手动在 Inspector 中设置
        // var controllers = UnityEngine.Object.FindObjectsByType<MainSceneController>(UnityEngine.FindObjectsSortMode.None);
        // foreach (var ctrl in controllers)
        // {
        //     Debug.Log($"MainSceneController 引用待修复，请在 Inspector 中手动关联 catModel 和 catAnimator");
        // }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("=== 重建完成 ===");
    }

    static GameObject FindMeshyRoot(GameObject go)
    {
        // 找带 SkinnedMeshRenderer 的根对象
        var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr != null) return smr.transform.root.gameObject;

        // 备选：找带 Animator 的根对象
        var anim = go.GetComponentInChildren<Animator>();
        if (anim != null) return anim.transform.root.gameObject;

        // 备选：第一个子对象
        if (go.transform.childCount > 0) return go.transform.GetChild(0).root.gameObject;
        return go;
    }

    static GameObject CreateOrUpdatePrefab(GameObject source, string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
        }

        // 实例化一个临时副本来保存
        var instance = UnityEngine.Object.Instantiate(source);
        instance.name = source.name;

        var prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
        UnityEngine.Object.DestroyImmediate(instance);

        Debug.Log($"创建 Prefab: {path}");
        return prefab;
    }

    static GameObject FindPrefabInstanceInScene(UnityEngine.SceneManagement.Scene scene, GameObject prefab)
    {
        foreach (var ro in scene.GetRootGameObjects())
        {
            if (PrefabUtility.GetPrefabInstanceStatus(ro) != PrefabInstanceStatus.NotAPrefab)
            {
                var assetPath = AssetDatabase.GetAssetPath(prefab);
                var instanceRoot = PrefabUtility.GetPrefabInstanceStatus(ro) == PrefabInstanceStatus.Connected
                    ? PrefabUtility.GetNearestPrefabInstanceRoot(ro) : ro;
                if (instanceRoot != null)
                {
                    var instPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instanceRoot);
                    if (instPath == assetPath) return instanceRoot;
                }
            }
        }
        return null;
    }
}
