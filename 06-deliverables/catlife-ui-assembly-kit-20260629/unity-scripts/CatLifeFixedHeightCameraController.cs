using UnityEngine;

namespace CatLife.UI
{
    public sealed class CatLifeFixedHeightCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraRig;
        [SerializeField] private Transform cameraTransform;

        [Header("Horizontal Rotate")]
        [SerializeField] private float rotateSpeedDegrees = 55f;

        [Header("Forward Back Move")]
        [SerializeField] private float moveSpeed = 1.8f;
        [SerializeField] private float minDistanceFromCenter = 2.2f;
        [SerializeField] private float maxDistanceFromCenter = 8.0f;

        private float fixedHeight;
        private float rotateInput;
        private float moveInput;

        private void Awake()
        {
            if (cameraRig == null)
            {
                cameraRig = transform;
            }

            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }

            fixedHeight = cameraRig.position.y;
        }

        private void Update()
        {
            if (cameraRig == null) return;

            if (Mathf.Abs(rotateInput) > 0.01f)
            {
                cameraRig.Rotate(Vector3.up, rotateInput * rotateSpeedDegrees * Time.deltaTime, Space.World);
            }

            if (Mathf.Abs(moveInput) > 0.01f)
            {
                Vector3 forward = cameraRig.forward;
                forward.y = 0f;
                forward.Normalize();

                Vector3 next = cameraRig.position + forward * (moveInput * moveSpeed * Time.deltaTime);
                next.y = fixedHeight;

                float distance = new Vector2(next.x, next.z).magnitude;
                if (distance >= minDistanceFromCenter && distance <= maxDistanceFromCenter)
                {
                    cameraRig.position = next;
                }
            }

            Vector3 locked = cameraRig.position;
            locked.y = fixedHeight;
            cameraRig.position = locked;
        }

        public void HoldRotateLeft() => rotateInput = -1f;
        public void HoldRotateRight() => rotateInput = 1f;
        public void StopRotate() => rotateInput = 0f;
        public void HoldMoveForward() => moveInput = 1f;
        public void HoldMoveBack() => moveInput = -1f;
        public void StopMove() => moveInput = 0f;
    }
}
