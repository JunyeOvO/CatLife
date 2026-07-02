using CatLife.CameraControls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace CatLife.UI
{
    [DisallowMultipleComponent]
    public sealed class CatLifeCameraRotationSlider : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEndDragHandler
    {
        [SerializeField] private CatLifePlazaCameraRotator cameraRotator;
        [FormerlySerializedAs("track")]
        [SerializeField] private RectTransform horizontalTrack;
        [FormerlySerializedAs("handle")]
        [SerializeField] private RectTransform horizontalHandle;
        [SerializeField] private RectTransform verticalTrack;
        [SerializeField] private RectTransform verticalHandle;
        [SerializeField] private GameObject controlsRoot;
        [SerializeField] private GameObject[] hideWhileHeld;
        [SerializeField] private float longPressSeconds = 0.22f;
        [SerializeField] private float deadZonePixels = 8f;

        private Vector2 horizontalHandleStartPosition;
        private Vector2 verticalHandleStartPosition;
        private bool hasHorizontalHandleStartPosition;
        private bool hasVerticalHandleStartPosition;
        private bool pointerHeld;
        private bool controlsVisible;
        private float pointerDownTime;
        private float pressVerticalLocalY;
        private float verticalStartNormalized;
        private Camera pressEventCamera;
        private Vector2 lastPointerPosition;

        private void Awake()
        {
            if (horizontalTrack == null)
            {
                horizontalTrack = transform as RectTransform;
            }

            if (cameraRotator == null && Camera.main != null)
            {
                cameraRotator = Camera.main.GetComponent<CatLifePlazaCameraRotator>();
            }

            CacheHandleStartPosition();
            ResetHorizontalHandle();
            SyncVerticalHandleFromCamera();
            SetHeldVisuals(false);
        }

        private void Update()
        {
            if (!pointerHeld || controlsVisible)
            {
                return;
            }

            if (Time.unscaledTime - pointerDownTime >= longPressSeconds)
            {
                BeginHeldControls();
            }
        }

        private void OnDisable()
        {
            EndHeldControls();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerHeld = true;
            pointerDownTime = Time.unscaledTime;
            pressEventCamera = eventData.pressEventCamera;
            lastPointerPosition = eventData.position;
            StopRotation();
            SetHeldVisuals(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!pointerHeld)
            {
                return;
            }

            pressEventCamera = eventData.pressEventCamera;
            lastPointerPosition = eventData.position;

            if (!controlsVisible && Time.unscaledTime - pointerDownTime >= longPressSeconds)
            {
                BeginHeldControls();
            }

            if (controlsVisible)
            {
                UpdateDrag(eventData.position, eventData.pressEventCamera);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EndHeldControls();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            EndHeldControls();
        }

        private void BeginHeldControls()
        {
            controlsVisible = true;
            CacheVerticalPress();
            ResetHorizontalHandle();
            SyncVerticalHandleFromCamera();
            SetHeldVisuals(true);
            UpdateDrag(lastPointerPosition, pressEventCamera);
        }

        private void EndHeldControls()
        {
            pointerHeld = false;
            controlsVisible = false;
            StopRotation();
            ResetHorizontalHandle();
            SetHeldVisuals(false);
        }

        private void UpdateDrag(Vector2 screenPosition, Camera eventCamera)
        {
            UpdateHorizontalDrag(screenPosition, eventCamera);
            UpdateVerticalDrag(screenPosition, eventCamera);
        }

        private void UpdateHorizontalDrag(Vector2 screenPosition, Camera eventCamera)
        {
            if (horizontalTrack == null)
            {
                return;
            }

            Vector2 localPoint;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(horizontalTrack, screenPosition, eventCamera, out localPoint))
            {
                StopRotation();
                return;
            }

            float halfTrack = Mathf.Max(0f, horizontalTrack.rect.width * 0.5f);
            float halfHandle = horizontalHandle != null ? horizontalHandle.rect.width * 0.5f : 0f;
            float maxOffset = Mathf.Max(0f, halfTrack - halfHandle);
            float offset = Mathf.Clamp(localPoint.x, -maxOffset, maxOffset);

            if (horizontalHandle != null)
            {
                Vector2 position = horizontalHandleStartPosition;
                position.x += offset;
                horizontalHandle.anchoredPosition = position;
            }

            float direction = 0f;
            if (offset < -deadZonePixels)
            {
                direction = -1f;
            }
            else if (offset > deadZonePixels)
            {
                direction = 1f;
            }

            if (cameraRotator != null)
            {
                cameraRotator.SetYawRotationDirection(direction);
            }
        }

        private void UpdateVerticalDrag(Vector2 screenPosition, Camera eventCamera)
        {
            if (verticalTrack == null || cameraRotator == null)
            {
                return;
            }

            Vector2 localPoint;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(verticalTrack, screenPosition, eventCamera, out localPoint))
            {
                return;
            }

            float halfTrack = Mathf.Max(0f, verticalTrack.rect.height * 0.5f);
            float halfHandle = verticalHandle != null ? verticalHandle.rect.height * 0.5f : 0f;
            float maxOffset = Mathf.Max(0f, halfTrack - halfHandle);
            if (maxOffset <= 0f)
            {
                return;
            }

            float delta = localPoint.y - pressVerticalLocalY;
            float normalized = verticalStartNormalized;
            if (Mathf.Abs(delta) > deadZonePixels)
            {
                normalized = Mathf.Clamp(verticalStartNormalized - delta / maxOffset, -1f, 1f);
            }

            cameraRotator.SetPitchOffsetNormalized(normalized);
            SetVerticalHandle(normalized);
        }

        private void CacheVerticalPress()
        {
            verticalStartNormalized = cameraRotator != null ? cameraRotator.PitchOffsetNormalized : 0f;
            pressVerticalLocalY = 0f;

            if (verticalTrack == null)
            {
                return;
            }

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(verticalTrack, lastPointerPosition, pressEventCamera, out localPoint))
            {
                pressVerticalLocalY = localPoint.y;
            }
        }

        private void StopRotation()
        {
            if (cameraRotator != null)
            {
                cameraRotator.StopRotation();
            }

            ResetHorizontalHandle();
        }

        private void ResetHorizontalHandle()
        {
            CacheHandleStartPosition();
            if (horizontalHandle != null)
            {
                horizontalHandle.anchoredPosition = horizontalHandleStartPosition;
            }
        }

        private void SyncVerticalHandleFromCamera()
        {
            float normalized = cameraRotator != null ? cameraRotator.PitchOffsetNormalized : 0f;
            SetVerticalHandle(normalized);
        }

        private void SetVerticalHandle(float normalized)
        {
            CacheHandleStartPosition();
            if (verticalTrack == null || verticalHandle == null)
            {
                return;
            }

            float halfTrack = Mathf.Max(0f, verticalTrack.rect.height * 0.5f);
            float halfHandle = verticalHandle.rect.height * 0.5f;
            float maxOffset = Mathf.Max(0f, halfTrack - halfHandle);
            Vector2 position = verticalHandleStartPosition;
            position.y -= Mathf.Clamp(normalized, -1f, 1f) * maxOffset;
            verticalHandle.anchoredPosition = position;
        }

        private void SetHeldVisuals(bool visible)
        {
            if (controlsRoot != null)
            {
                controlsRoot.SetActive(visible);
            }
            else
            {
                SetActive(horizontalTrack, visible);
                SetActive(verticalTrack, visible);
            }

            if (hideWhileHeld == null)
            {
                return;
            }

            for (int i = 0; i < hideWhileHeld.Length; i++)
            {
                if (hideWhileHeld[i] != null)
                {
                    hideWhileHeld[i].SetActive(!visible);
                }
            }
        }

        private static void SetActive(RectTransform rectTransform, bool active)
        {
            if (rectTransform != null)
            {
                rectTransform.gameObject.SetActive(active);
            }
        }

        private void CacheHandleStartPosition()
        {
            if (!hasHorizontalHandleStartPosition && horizontalHandle != null)
            {
                horizontalHandleStartPosition = horizontalHandle.anchoredPosition;
                hasHorizontalHandleStartPosition = true;
            }

            if (!hasVerticalHandleStartPosition && verticalHandle != null)
            {
                verticalHandleStartPosition = verticalHandle.anchoredPosition;
                hasVerticalHandleStartPosition = true;
            }
        }
    }
}
