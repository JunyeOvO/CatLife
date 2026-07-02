using UnityEngine;

namespace CatLife.CameraControls
{
    [DisallowMultipleComponent]
    public sealed class CatLifePlazaCameraRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 fixedPosition = new Vector3(0.1f, 2.88f, -0.58f);
        [SerializeField] private float yawDegrees = 180f;
        [SerializeField] private float basePitchDegrees = 8f;
        [SerializeField] private float pitchDegrees = 8f;
        [SerializeField] private float pitchOffsetDegrees;
        [SerializeField] private float maxPitchOffsetDegrees = 45f;
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
                pitchDegrees = ClampPitch(value);
                pitchOffsetDegrees = Mathf.Clamp(pitchDegrees - basePitchDegrees, -MaxPitchOffsetDegrees, MaxPitchOffsetDegrees);
                ApplyPose();
            }
        }

        public float BasePitchDegrees
        {
            get { return basePitchDegrees; }
            set
            {
                basePitchDegrees = ClampPitch(value);
                pitchOffsetDegrees = Mathf.Clamp(pitchDegrees - basePitchDegrees, -MaxPitchOffsetDegrees, MaxPitchOffsetDegrees);
                ApplyPose();
            }
        }

        public float MaxPitchOffsetDegrees
        {
            get { return Mathf.Max(0f, maxPitchOffsetDegrees); }
            set
            {
                maxPitchOffsetDegrees = Mathf.Max(0f, value);
                pitchOffsetDegrees = Mathf.Clamp(pitchOffsetDegrees, -MaxPitchOffsetDegrees, MaxPitchOffsetDegrees);
                pitchDegrees = ClampPitch(basePitchDegrees + pitchOffsetDegrees);
                ApplyPose();
            }
        }

        public float PitchOffsetNormalized
        {
            get
            {
                float maxOffset = MaxPitchOffsetDegrees;
                return maxOffset > 0f ? Mathf.Clamp(pitchOffsetDegrees / maxOffset, -1f, 1f) : 0f;
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
            SetYawRotationDirection(direction);
        }

        public void SetYawRotationDirection(float direction)
        {
            rotationDirection = Mathf.Approximately(direction, 0f) ? 0f : Mathf.Sign(direction);
        }

        public void SetPitchOffsetNormalized(float normalizedOffset)
        {
            pitchOffsetDegrees = Mathf.Clamp(normalizedOffset, -1f, 1f) * MaxPitchOffsetDegrees;
            pitchDegrees = ClampPitch(basePitchDegrees + pitchOffsetDegrees);
            ApplyPose();
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

        private static float ClampPitch(float value)
        {
            return Mathf.Clamp(value, -89f, 89f);
        }
    }
}
