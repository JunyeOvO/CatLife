using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimationClipExtractor : EditorWindow
{
    [MenuItem("CatLife/提取猫动画Clip")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipExtractor>("猫动画提取");
    }

    void OnGUI()
    {
        GUILayout.Label("从 Animation Walking FBX 提取走路动画", EditorStyles.boldLabel);
        GUILayout.Space(8);
        if (GUILayout.Button("执行提取", GUILayout.Height(35)))
        {
            Extract();
        }
    }

    static void Extract()
    {
        const string catFolder = "Assets/Art/Cat/";
        const string animFbxPath = catFolder + "Meshy_AI_Low_Poly_Orange_Cat_quadruped_Animation_Walking_frame_rate_60.fbx";

        var subAssets = AssetDatabase.LoadAllAssetsAtPath(animFbxPath);
        AnimationClip walkClip = null;

        foreach (var asset in subAssets)
        {
            if (asset is AnimationClip clip)
            {
                Debug.Log("[Extractor] 发现 Clip: '" + clip.name + "', length=" + clip.length.ToString("F3") + "s");
                if (walkClip == null || clip.length > walkClip.length)
                {
                    walkClip = clip;
                }
            }
        }

        if (walkClip == null)
        {
            Debug.LogError("[Extractor] 未从 Animation FBX 中找到 AnimationClip！");
            EditorUtility.DisplayDialog("错误", "未找到 AnimationClip，请检查 FBX 文件。", "确定");
            return;
        }

        Debug.Log("[Extractor] 使用 Clip: '" + walkClip.name + "', length=" + walkClip.length.ToString("F3") + "s");

        AnimationClip clonedClip = UnityEngine.Object.Instantiate(walkClip);
        clonedClip.name = "Cat_Walking";

        string outputPath = catFolder + "Cat_Walking.anim";
        AssetDatabase.CreateAsset(clonedClip, outputPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var verifiedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);
        if (verifiedClip != null)
        {
            Debug.Log("[Extractor] 成功: " + outputPath);
        }

        string msg = "动画Clip提取成功！\n\n文件: " + outputPath + "\n持续时间: " + clonedClip.length.ToString("F2") + "s\n\n下一步：将 Cat_Walking.anim 拖到 Character 模型的 Animator Controller，设为默认动画。";
        EditorUtility.DisplayDialog("完成", msg, "确定");
    }
}
