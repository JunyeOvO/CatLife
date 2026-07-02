using UnityEngine;

namespace CatLife.Cat
{
    [DisallowMultipleComponent]
    public sealed class CatTownWalker : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string isWalkingParameter = "IsWalking";
        [SerializeField] private Vector2 xRange = new Vector2(-6.5f, 6.5f);
        [SerializeField] private Vector2 zRange = new Vector2(-12.5f, -4.0f);
        [SerializeField] private float groundY = 0.03f;
        [SerializeField] private float walkSpeed = 1.15f;
        [SerializeField] private float turnSpeed = 5.5f;
        [SerializeField] private Vector2 waitSecondsRange = new Vector2(1.0f, 2.8f);
        [SerializeField] private float targetTolerance = 0.08f;
        [SerializeField] private bool startWalkingOnEnable = true;

        private Vector3 targetPosition;
        private float waitUntil;
        private bool isWalking;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(true);
            }
        }

        private void OnEnable()
        {
            SnapToGround();

            if (startWalkingOnEnable)
            {
                PickTarget();
                SetWalking(true);
            }
            else
            {
                waitUntil = Time.time + Random.Range(waitSecondsRange.x, waitSecondsRange.y);
                SetWalking(false);
            }
        }

        private void Update()
        {
            if (!isWalking)
            {
                if (Time.time >= waitUntil)
                {
                    PickTarget();
                    SetWalking(true);
                }

                return;
            }

            Vector3 current = transform.position;
            Vector3 toTarget = targetPosition - current;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude <= targetTolerance * targetTolerance)
            {
                waitUntil = Time.time + Random.Range(waitSecondsRange.x, waitSecondsRange.y);
                SetWalking(false);
                return;
            }

            Vector3 direction = toTarget.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

            Vector3 next = Vector3.MoveTowards(current, targetPosition, walkSpeed * Time.deltaTime);
            next.y = groundY;
            transform.position = next;
        }

        public void Recenter(Vector3 center, Vector2 size)
        {
            xRange = new Vector2(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
            zRange = new Vector2(center.z - size.y * 0.5f, center.z + size.y * 0.5f);
            SnapToGround();
            PickTarget();
        }

        private void PickTarget()
        {
            targetPosition = new Vector3(
                Random.Range(xRange.x, xRange.y),
                groundY,
                Random.Range(zRange.x, zRange.y));
        }

        private void SnapToGround()
        {
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(position.x, xRange.x, xRange.y);
            position.y = groundY;
            position.z = Mathf.Clamp(position.z, zRange.x, zRange.y);
            transform.position = position;
        }

        private void SetWalking(bool walking)
        {
            isWalking = walking;
            if (animator != null && !string.IsNullOrEmpty(isWalkingParameter))
            {
                animator.SetBool(isWalkingParameter, walking);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.62f, 0.1f, 0.75f);
            Vector3 center = new Vector3((xRange.x + xRange.y) * 0.5f, groundY, (zRange.x + zRange.y) * 0.5f);
            Vector3 size = new Vector3(Mathf.Abs(xRange.y - xRange.x), 0.05f, Mathf.Abs(zRange.y - zRange.x));
            Gizmos.DrawWireCube(center, size);
        }
    }
}
