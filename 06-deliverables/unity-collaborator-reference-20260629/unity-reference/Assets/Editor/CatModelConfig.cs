using UnityEngine;
using UnityEditor;

public class CatModelConfig : EditorWindow
{
    [MenuItem("CatLife/配置猫模型")]
    public static void ShowWindow()
    {
        GetWindow<CatModelConfig>("配置猫模型");
    }

    public void OnGUI()
    {
        GUILayout.Label("Meshy AI 猫模型配置", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

        if (GUILayout.Button("配置 Character 模型（Generic）", GUILayout.Height(30)))
        {
            ConfigureCharacterModel();
        }

        if (GUILayout.Button("配置 Animation 模型", GUILayout.Height(30)))
        {
            ConfigureAnimationModel();
        }

        if (GUILayout.Button("全部执行", GUILayout.Height(40)))
        {
            ConfigureCharacterModel();
            ConfigureAnimationModel();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("完成", "猫模型配置完成！", "确定");
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "步骤：\n" +
            "1. 点击「全部执行」\n" +
            "2. 把 Character_output 拖到场景里\n" +
            "3. 给猫挂上 CatController 脚本\n" +
            "4. 动画 clip 在 Project 面板手动拖到 Animator Controller",
            MessageType.Info);
    }

    static void ConfigureCharacterModel()
    {
        string charPath = "Assets/Art/Cat/Meshy_AI_Low_Poly_Orange_Cat_quadruped_Character_output.fbx";
        var importer = AssetImporter.GetAtPath(charPath) as ModelImporter;
        if (importer == null)
        {
            Debug.LogError("找不到 Character 模型: " + charPath);
            return;
        }

        importer.animationType = ModelImporterAnimationType.Generic;
        importer.importAnimation = true;
        importer.importBlendShapes = true;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        importer.SaveAndReimport();

        Debug.Log("[CatModelConfig] Character 模型配置完成");
    }

    static void ConfigureAnimationModel()
    {
        string animPath = "Assets/Art/Cat/Meshy_AI_Low_Poly_Orange_Cat_quadruped_Animation_Walking_frame_rate_60.fbx";
        var importer = AssetImporter.GetAtPath(animPath) as ModelImporter;
        if (importer == null)
        {
            Debug.LogError("找不到 Animation 模型: " + animPath);
            return;
        }

        importer.animationType = ModelImporterAnimationType.Generic;
        importer.importAnimation = true;
        importer.materialImportMode = ModelImporterMaterialImportMode.None;
        importer.SaveAndReimport();

        Debug.Log("[CatModelConfig] Animation 模型配置完成");
    }
}
