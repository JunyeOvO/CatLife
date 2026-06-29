using UnityEngine;
using CatLife.Core;

namespace CatLife.Cat
{
    /// <summary>
    /// 猫咪行为控制
    /// 负责播放动画、响应状态变化、执行随机小动作
    /// </summary>
    public class CatController : MonoBehaviour
    {
        [Header("猫咪模型")]
        [SerializeField] private GameObject catModel;
        [SerializeField] private Animator catAnimator;

        [Header("动画配置")]
        [SerializeField] private string idleAnimName = "CL_CAT_IdleBreath_v06_headsync_loop_108f";
        [SerializeField] private string walkAnimName = "CL_CAT_CuriousSniff_v02_loop_112f";
        [SerializeField] private string restAnimName = "CL_CAT_IdleBreath_v06_headsync_loop_108f";
        [SerializeField] private string celebrateAnimName = "CL_CAT_TailWagHappy_v01_loop_96f";
        [SerializeField] private string approachAnimName = "CL_CAT_CuriousSniff_v02_loop_112f";
        [SerializeField] private float crossFadeDuration = 0.15f;
        [SerializeField] private int animationLayer = 0;

        [Header("随机动作")]
        [SerializeField] private bool enableRandomActions = true;
        [SerializeField] private float randomActionIntervalMin = 3f;
        [SerializeField] private float randomActionIntervalMax = 8f;

        private StateMachine stateMachine;
        private bool isActing = false;

        // 随机动作参数
        private float nextActionTime = 0f;
        private string[] randomActionStateNames =
        {
            "CL_CAT_AlertLook_v01_loop_120f",
            "CL_CAT_PawWave_v01_loop_96f",
            "CL_CAT_HeadTiltListen_v01_loop_96f",
            "CL_CAT_LookBack_v02_loop_112f",
            "CL_CAT_EarTwitchAlert_v02_loop_120f"
        };

        private void Awake()
        {
            // 查找场景中的 StateMachine
            stateMachine = FindAnyObjectByType<CatLife.Core.StateMachine>();

            if (catAnimator == null)
            {
                catAnimator = GetComponentInChildren<Animator>();
            }
        }

        private void OnEnable()
        {
            if (stateMachine != null)
            {
                stateMachine.OnEnterState += HandleStateEnter;
                stateMachine.OnExitState += HandleStateExit;
            }
        }

        private void OnDisable()
        {
            if (stateMachine != null)
            {
                stateMachine.OnEnterState -= HandleStateEnter;
                stateMachine.OnExitState -= HandleStateExit;
            }
        }

        private void Start()
        {
            // 初始化随机动作计时
            ResetNextActionTime();

            if (stateMachine == null || stateMachine.IsState(CatState.Normal))
            {
                PlayIdle();
            }
        }

        private void Update()
        {
            // 随机小动作（仅在 Normal 状态）
            if (enableRandomActions && stateMachine != null && stateMachine.IsState(CatState.Normal))
            {
                TryRandomAction();
            }
        }

        #region 状态响应

        private void HandleStateEnter(CatState state)
        {
            Debug.Log($"[CatController] 进入状态: {state}");
            switch (state)
            {
                case CatState.Normal:
                    PlayIdle();
                    break;
                case CatState.Transition:
                    PlayApproach();
                    break;
                case CatState.Focus:
                    PlayRest();
                    break;
                case CatState.Reward:
                    PlayCelebrate();
                    break;
            }
        }

        private void HandleStateExit(CatState state)
        {
            // 保留状态退出扩展点，后续可用于过渡音效或表情复位。
        }

        #endregion

        #region 动画播放

        /// <summary>
        /// 播放待机动画（循环）
        /// </summary>
        public void PlayIdle()
        {
            PlayAnimation(idleAnimName);
        }

        /// <summary>
        /// 播放走近动画（从 Normal 到 Focus 的过渡）
        /// </summary>
        public void PlayApproach()
        {
            PlayAnimation(approachAnimName);
        }

        /// <summary>
        /// 播放休息动画（专注状态，循环）
        /// </summary>
        public void PlayRest()
        {
            PlayAnimation(restAnimName);
        }

        /// <summary>
        /// 播放庆祝动画（一次性）
        /// </summary>
        public void PlayCelebrate()
        {
            PlayAnimation(celebrateAnimName);
        }

        /// <summary>
        /// 播放走路动画
        /// </summary>
        public void PlayWalk()
        {
            PlayAnimation(walkAnimName);
        }

        /// <summary>
        /// 停止所有动画
        /// </summary>
        public void StopAnimation()
        {
            if (catAnimator != null)
            {
                catAnimator.speed = 0f;
            }
        }

        /// <summary>
        /// 播放指定动画
        /// </summary>
        /// <param name="animName">Animator Controller 中的状态名称</param>
        private void PlayAnimation(string animName)
        {
            if (catAnimator == null || string.IsNullOrEmpty(animName))
            {
                return;
            }

            int stateHash = Animator.StringToHash(animName);
            if (!catAnimator.HasState(animationLayer, stateHash))
            {
                Debug.LogWarning($"[CatController] Animator 缺少状态: {animName}");
                return;
            }

            catAnimator.speed = 1f;
            catAnimator.CrossFadeInFixedTime(stateHash, crossFadeDuration, animationLayer, 0f);
        }

        #endregion

        #region 随机动作

        /// <summary>
        /// 尝试执行随机动作
        /// </summary>
        private void TryRandomAction()
        {
            if (Time.time >= nextActionTime && !isActing)
            {
                StartCoroutine(RandomActionCoroutine());
            }
        }

        private System.Collections.IEnumerator RandomActionCoroutine()
        {
            isActing = true;

            // 随机选择动作
            int actionIndex = Random.Range(0, randomActionStateNames.Length);
            string actionStateName = randomActionStateNames[actionIndex];

            Debug.Log($"[CatController] 执行随机动作: {actionStateName}");

            // 播放随机动作
            PlayAnimation(actionStateName);

            // 等待一小段时间
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            // 恢复到待机
            PlayIdle();
            ResetNextActionTime();
            isActing = false;
        }

        private void ResetNextActionTime()
        {
            nextActionTime = Time.time + Random.Range(randomActionIntervalMin, randomActionIntervalMax);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取猫咪模型 Transform（供其他脚本使用）
        /// </summary>
        public Transform GetCatTransform()
        {
            return catModel != null ? catModel.transform : transform;
        }

        /// <summary>
        /// 设置猫咪位置
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            if (catModel != null)
                catModel.transform.position = position;
            else
                transform.position = position;
        }

        /// <summary>
        /// 看向指定方向
        /// </summary>
        public void LookAt(Vector3 direction)
        {
            if (catModel != null)
            {
                catModel.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        #endregion
    }
}
