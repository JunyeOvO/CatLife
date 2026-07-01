using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public sealed class CatLifeHomeFontBinder : MonoBehaviour
{
    [SerializeField]
    private string[] preferredFontNames =
    {
        "Microsoft YaHei UI",
        "Microsoft YaHei",
        "SimHei",
        "Arial"
    };

    private static Font cachedFont;

    private void OnEnable()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    public void Apply()
    {
        Font font = ResolveFont();
        if (font == null)
        {
            return;
        }

        Text[] labels = GetComponentsInChildren<Text>(true);
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].font = font;
            labels[i].SetAllDirty();
        }
    }

    private Font ResolveFont()
    {
        if (cachedFont != null)
        {
            return cachedFont;
        }

        cachedFont = Font.CreateDynamicFontFromOSFont(preferredFontNames, 64);
        if (cachedFont == null)
        {
            cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        return cachedFont;
    }
}
