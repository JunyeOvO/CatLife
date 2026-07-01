using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFadeTransition : MonoBehaviour
{
    [Header("淡入淡出")]
    [SerializeField] private Image fadeImage;       // 全屏黑色 Image
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("自动跳转")]
    [SerializeField] private float jumpDelay = 2f;   // 等待几秒后跳转
    [SerializeField] private string nextSceneName = "mainscene";

    private bool hasJumped = false;

    private void Start()
    {
        if (!hasJumped && !string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(AutoJump());
        }
    }

    private IEnumerator AutoJump()
    {
        // 等到延迟时间
        yield return new WaitForSeconds(jumpDelay);

        // 空保护：如果 fadeImage 没设置，直接跳转不过渡
        if (fadeImage == null)
        {
            Debug.LogWarning("[SceneFade] fadeImage 未设置，直接加载场景");
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        LoadSceneWithFade(nextSceneName);
        hasJumped = true;
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (fadeImage == null) yield break;

        // 淡出到黑屏
        float elapsed = 0;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;

        // 黑屏状态下加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 淡入回来
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
    }
}