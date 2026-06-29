using UnityEngine;
using System;

namespace CatLife.Core
{
    /// <summary>
    /// 猫咪状态枚举
    /// </summary>
    public enum CatState
    {
        Normal,     // 普通状态（自由活动）
        Transition, // 过渡状态（检测到分心，猫咪走近提醒）
        Focus,      // 专注状态（安静趴着）
        Reward      // 奖励状态（庆祝/撒花）
    }

    /// <summary>
    /// 状态机核心类
    /// 提供状态切换、状态查询、状态事件通知
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        [Header("初始状态")]
        [SerializeField] private CatState initialState = CatState.Normal;

        [Header("自动切换配置")]
        [SerializeField] private bool autoTransition = true;
        [SerializeField] private float transitionDelay = 2f;

        // 当前状态
        private CatState _currentState;
        public CatState currentState => _currentState;

        // 上一状态（用于过渡动画判断）
        public CatState previousState { get; private set; }

        // 状态事件
        public event Action<CatState> OnStateChanged;
        public event Action<CatState> OnEnterState;
        public event Action<CatState> OnExitState;

        // 静态事件（全局通知，方便其他脚本订阅）
        public static event Action<CatState> GlobalStateChanged;

        private void Awake()
        {
            _currentState = initialState;
            previousState = initialState;
        }

        private void Start()
        {
            OnEnterState?.Invoke(_currentState);
        }

        /// <summary>
        /// 切换到新状态
        /// </summary>
        public void SwitchState(CatState newState)
        {
            if (newState == _currentState) return;

            previousState = _currentState;
            CatState oldState = _currentState;
            _currentState = newState;

            Debug.Log($"[StateMachine] 状态切换: {oldState} → {newState}");

            // 触发事件
            OnExitState?.Invoke(oldState);
            OnStateChanged?.Invoke(newState);
            GlobalStateChanged?.Invoke(newState);
            OnEnterState?.Invoke(newState);
        }

        /// <summary>
        /// 延迟切换状态（用于过渡动画）
        /// </summary>
        public void SwitchStateDelayed(CatState newState, float delay)
        {
            StartCoroutine(DelayedSwitch(newState, delay));
        }

        private System.Collections.IEnumerator DelayedSwitch(CatState newState, float delay)
        {
            yield return new WaitForSeconds(delay);
            SwitchState(newState);
        }

        /// <summary>
        /// 检查是否处于某个状态
        /// </summary>
        public bool IsState(CatState state) => _currentState == state;

        /// <summary>
        /// 检查是否处于任意一个指定状态
        /// </summary>
        public bool IsAnyState(params CatState[] states)
        {
            foreach (var s in states)
                if (_currentState == s) return true;
            return false;
        }

        /// <summary>
        /// 是否可以进入专注状态（手动或自动触发）
        /// </summary>
        public bool CanEnterFocus()
        {
            return IsAnyState(CatState.Normal, CatState.Transition);
        }

        /// <summary>
        /// 从专注状态退出
        /// </summary>
        public void ExitFocus()
        {
            if (IsState(CatState.Focus))
            {
                SwitchState(CatState.Reward);
            }
        }
    }
}
