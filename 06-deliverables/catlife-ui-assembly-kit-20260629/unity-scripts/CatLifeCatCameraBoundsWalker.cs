using UnityEngine;

namespace CatLife.UI
{
    public sealed class CatLifeCatCameraBoundsWalker : MonoBehaviour
    {
        [SerializeField] private Transform catRoot;
        [SerializeField] private Animator animator;
        [SerializeField] private Vector2 localXRange = new Vector2(-1.6f, 1.6f);
        [SerializeField] private Vector2 localZRange = new Vector2(-0.8f, 2.4f);
        [SerializeField] private float walkSpeed = 0.8f;
        [SerializeField] private float retargetSeconds = 4f;

        private Vector3 targetLocal;
        private float nextRetargetAt;

        private void Awake()
        {
            if (catRoot == null)
            {
                catRoot = transform;
            }

            PickTarget();
        }

        private void Update()
        {
            if (catRoot == null) return;

            if (Time.time >= nextRetargetAt)
            {
                PickTarget();
            }

            Vector3 current = catRoot.localPosition;
            Vector3 next = Vector3.MoveTowards(current, targetLocal, walkSpeed * Time.deltaTime);
            catRoot.localPosition = next;

            Vector3 delta = targetLocal - current;
            bool walking = delta.sqrMagnitude > 0.01f;
            if (walking)
            {
                Quaternion look = Quaternion.LookRotation(new Vector3(delta.x, 0f, delta.z), Vector3.up);
                catRoot.localRotation = Quaternion.Slerp(catRoot.localRotation, look, Time.deltaTime * 5f);
            }

            if (animator != null)
            {
                animator.SetBool("IsWalking", walking);
            }
        }

        public void SetBehaviorTarget(string behavior)
        {
            if (animator == null) return;

            animator.ResetTrigger("CuriousSniff");
            animator.ResetTrigger("HeadTiltListen");
            animator.ResetTrigger("TailWagHappy");

            switch (behavior)
            {
                case "transition":
                    animator.SetTrigger("CuriousSniff");
                    break;
                case "focus":
                    animator.SetTrigger("HeadTiltListen");
                    break;
                case "reward":
                    animator.SetTrigger("TailWagHappy");
                    break;
            }
        }

        private void PickTarget()
        {
            targetLocal = new Vector3(
                Random.Range(localXRange.x, localXRange.y),
                catRoot != null ? catRoot.localPosition.y : 0f,
                Random.Range(localZRange.x, localZRange.y));
            nextRetargetAt = Time.time + retargetSeconds;
        }
    }
}
