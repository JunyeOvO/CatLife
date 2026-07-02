using CatLife.CameraControls;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CatLife.UI
{
    [DisallowMultipleComponent]
    public sealed class CatLifeCameraRotationSlider : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEndDragHandler
    {
        [SerializeField] private CatLifePlazaCameraRotator cameraRotator;
        [SerializeField] private RectTransform track;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float deadZonePixels = 8f;

        private Vector2 handleStartPosition;
        private bool hasHandleStartPosition;

        private void Awake()
        {
            if (track == null)
            {
                track = transform as RectTransform;
            }

            if (cameraRotator == null && Camera.main != null)
            {
                cameraRotator = Camera.main.GetComponent<CatLifePlazaCameraRotator>();
            }

            CacheHandleStartPosition();
            ResetHandle();
        }

        private void OnDisable()
        {
            StopRotation();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopRotation();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StopRotation();
        }

        private void UpdateDrag(PointerEventData eventData)
        {
            if (track == null)
            {
                return;
            }

            Vector2 localPoint;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(track, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                StopRotation();
                return;
            }

            float halfTrack = Mathf.Max(0f, track.rect.width * 0.5f);
            float halfHandle = handle != null ? handle.rect.width * 0.5f : 0f;
            float maxOffset = Mathf.Max(0f, halfTrack - halfHandle);
            float offset = Mathf.Clamp(localPoint.x, -maxOffset, maxOffset);

            if (handle != null)
            {
                Vector2 position = handleStartPosition;
                position.x += offset;
                handle.anchoredPosition = position;
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
                cameraRotator.SetRotationDirection(direction);
            }
        }

        private void StopRotation()
        {
            if (cameraRotator != null)
            {
                cameraRotator.StopRotation();
            }

            ResetHandle();
        }

        private void ResetHandle()
        {
            CacheHandleStartPosition();
            if (handle != null)
            {
                handle.anchoredPosition = handleStartPosition;
            }
        }

        private void CacheHandleStartPosition()
        {
            if (!hasHandleStartPosition && handle != null)
            {
                handleStartPosition = handle.anchoredPosition;
                hasHandleStartPosition = true;
            }
        }
    }
}
