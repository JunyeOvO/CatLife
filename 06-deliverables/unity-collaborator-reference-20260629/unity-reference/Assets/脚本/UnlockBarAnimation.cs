using UnityEngine;

public class UnlockBarAnimation : MonoBehaviour
{
    public float floatHeight = 15f;
    public float floatSpeed = 2f;

    private RectTransform rect;
    private Vector2 startPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        rect.anchoredPosition = startPos + new Vector2(0, yOffset);
    }
}
