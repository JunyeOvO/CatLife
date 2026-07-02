using UnityEngine;

namespace CatLife.CameraControls
{
    [DisallowMultipleComponent]
    public sealed class CatLifePlazaCameraRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 fixedPosition = new Vector3(0.1f, 2.88f, -0.58f);
        [SerializeField] private float yawDegrees = 180f;
        [SerializeField] private float pitchDegrees = 8f;
        [SerializeField] private float degreesPerSecond = 10f;
        [SerializeField] private bool useUnscaledTime = true;

        private float rotationDirection;

        public Vector3 FixedPosition
        {
            get { return fixedPosition; }
            set
            {
                fixedPosition = value;
                ApplyPose();
            }
        }

        public float YawDegrees
        {
            get { return yawDegrees; }
            set
            {
                yawDegrees = value;
                ApplyPose();
            }
        }

        public float PitchDegrees
        {
            get { return pitchDegrees; }
            set
            {
                pitchDegrees = value;
                ApplyPose();
            }
        }

        public float DegreesPerSecond
        {
            get { return degreesPerSecond; }
            set { degreesPerSecond = Mathf.Max(0f, value); }
        }

        private void Awake()
        {
            ApplyPose();
        }

        private void LateUpdate()
        {
            StepRotation(useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
            ApplyPose();
        }

        public void SetRotationDirection(float direction)
        {
            rotationDirection = Mathf.Approximately(direction, 0f) ? 0f : Mathf.Sign(direction);
        }

        public void StopRotation()
        {
            rotationDirection = 0f;
        }

        public void StepRotation(float deltaTime)
        {
            if (Mathf.Approximately(rotationDirection, 0f))
            {
                return;
            }

            yawDegrees = Mathf.Repeat(yawDegrees + Mathf.Sign(rotationDirection) * degreesPerSecond * Mathf.Max(0f, deltaTime), 360f);
        }

        public void ApplyPose()
        {
            transform.position = fixedPosition;
            transform.rotation = Quaternion.Euler(pitchDegrees, yawDegrees, 0f);
        }
    }
}
