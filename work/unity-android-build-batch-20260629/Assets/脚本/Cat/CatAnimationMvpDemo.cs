using System.Collections;
using UnityEngine;
using CatLife.Core;

namespace CatLife.Cat
{
    /// <summary>
    /// MVP 动画验收入口：按固定节奏驱动 CatLife 四个核心状态。
    /// </summary>
    public class CatAnimationMvpDemo : MonoBehaviour
    {
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool loop = true;
        [SerializeField] private float stateDuration = 3f;

        private readonly CatState[] demoSequence =
        {
            CatState.Normal,
            CatState.Transition,
            CatState.Focus,
            CatState.Reward
        };

        private Coroutine demoCoroutine;

        private void Awake()
        {
            if (stateMachine == null)
            {
                stateMachine = FindAnyObjectByType<StateMachine>();
            }
        }

        private void Start()
        {
            if (playOnStart)
            {
                PlayDemo();
            }
        }

        public void PlayDemo()
        {
            if (stateMachine == null)
            {
                Debug.LogWarning("[CatAnimationMvpDemo] Missing StateMachine; cannot play demo sequence.");
                return;
            }

            if (demoCoroutine != null)
            {
                StopCoroutine(demoCoroutine);
            }

            demoCoroutine = StartCoroutine(PlayDemoSequence());
        }

        private IEnumerator PlayDemoSequence()
        {
            do
            {
                for (int i = 0; i < demoSequence.Length; i++)
                {
                    stateMachine.SwitchState(demoSequence[i]);
                    yield return new WaitForSeconds(stateDuration);
                }
            }
            while (loop);

            demoCoroutine = null;
        }
    }
}
