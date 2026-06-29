using UnityEngine;
using System;
using System.Collections.Generic;

namespace CatLife.LLM
{
    /// <summary>
    /// Unity 主线程调度器
    /// 将子线程的任务调度到主线程执行
    /// 必须挂载在一个 GameObject 上才能工作
    /// </summary>
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        public static UnityMainThreadDispatcher Instance { get; private set; }

        private readonly Queue<Action> _queue = new Queue<Action>();
        private readonly object _lock = new object();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateInstance()
        {
            if (Instance != null) return;

            GameObject go = new GameObject("[UnityMainThreadDispatcher]");
            Instance = go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideInHierarchy;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            lock (_lock)
            {
                while (_queue.Count > 0)
                {
                    var action = _queue.Dequeue();
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        /// <summary>
        /// 将 Action 排队到主线程执行
        /// </summary>
        public static void Enqueue(Action action)
        {
            if (action == null) return;

            if (Instance == null)
            {
                Debug.LogWarning("[UnityMainThreadDispatcher] Instance 未创建，action 被忽略");
                return;
            }

            lock (Instance._lock)
            {
                Instance._queue.Enqueue(action);
            }
        }
    }
}
