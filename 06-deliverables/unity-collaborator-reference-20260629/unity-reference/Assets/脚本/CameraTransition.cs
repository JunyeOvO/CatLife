using UnityEngine;
using System.Collections;
using System;  // ⚠️ 这个很重要！用于 Action 事件

public class CameraExactTransition : MonoBehaviour
{
    // ✅ 事件声明
    public static event Action OnCameraTransitionComplete;

    [Header("起始位置/旋转")]
    [SerializeField] private Vector3 startPosition = new Vector3(614f, 681f, -303f);
    [SerializeField] private Vector3 startRotation = new Vector3(50.889f, -149.442f, -5.715f);

    [Header("目标位置/旋转")]
    [SerializeField] private Vector3 endPosition = new Vector3(596f, 15f, -513f);
    [SerializeField] private Vector3 endRotation = new Vector3(3.551f, -136.607f, -1.169f);

    [Header("动画设置")]
    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private float delayBeforeStart = 0f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("触发方式")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private KeyCode triggerKey = KeyCode.Space;

    private Camera mainCamera;
    private bool isTransitioning = false;

    void Start()
    {
        mainCamera = Camera.main;

        mainCamera.transform.position = startPosition;
        mainCamera.transform.eulerAngles = startRotation;

        if (autoStart)
        {
            Invoke(nameof(StartTransition), delayBeforeStart);
        }
    }

    void Update()
    {
        if (!autoStart && Input.GetKeyDown(triggerKey) && !isTransitioning)
        {
            StartTransition();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToStart();
        }
    }

    public void StartTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(SmoothTransition());
        }
    }

    IEnumerator SmoothTransition()
    {
        isTransitioning = true;

        Vector3 startPos = mainCamera.transform.position;
        Vector3 startRot = mainCamera.transform.eulerAngles;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            float curveT = moveCurve.Evaluate(t);

            mainCamera.transform.position = Vector3.Lerp(startPos, endPosition, curveT);
            mainCamera.transform.eulerAngles = new Vector3(
                Mathf.LerpAngle(startRot.x, endRotation.x, curveT),
                Mathf.LerpAngle(startRot.y, endRotation.y, curveT),
                Mathf.LerpAngle(startRot.z, endRotation.z, curveT)
            );

            yield return null;
        }

        mainCamera.transform.position = endPosition;
        mainCamera.transform.eulerAngles = endRotation;

        isTransitioning = false;

        // ✅ 触发事件
        if (OnCameraTransitionComplete != null)
        {
            OnCameraTransitionComplete();
        }

        Debug.Log("相机过渡完成！");
    }

    public void ResetToStart()
    {
        StopAllCoroutines();
        mainCamera.transform.position = startPosition;
        mainCamera.transform.eulerAngles = startRotation;
        isTransitioning = false;
    }
}
