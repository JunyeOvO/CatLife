using UnityEngine;
using System.Text;

namespace CatLife.LLM
{
    /// <summary>
    /// Mock LLM 客户端（用于 Editor 开发调试）
    /// 模拟流式输出，不依赖真实 BlueLM SDK
    /// </summary>
    public class MockLLMClient : ILLMClient
    {
        public bool IsInitialized => _isInitialized;
        private bool _isInitialized = false;

        [SerializeField] private float initDelay = 1f;
        [SerializeField] private float tokenInterval = 0.05f;
        [SerializeField] private string[] mockResponses = new string[]
        {
            "专注",
            "分心，请猫咪走近提醒用户"
        };

        public void Initialize(System.Action<int> onComplete)
        {
            _isInitialized = false;
            Debug.Log("[MockLLMClient] 开始初始化（模拟 1 秒）...");

            // 模拟初始化延迟
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                System.Threading.Thread.Sleep((int)(initDelay * 1000));
                _isInitialized = true;
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Debug.Log("[MockLLMClient] 初始化成功（Mock）");
                    onComplete?.Invoke(0);
                });
            });
        }

        public void Generate(string prompt, LLMCallback callback)
        {
            if (!_isInitialized)
            {
                callback?.OnError?.Invoke(-1, "Mock LLM 未初始化");
                return;
            }

            Debug.Log($"[MockLLMClient] 收到请求: {prompt}");

            // 判断是哪种请求
            string response = mockResponses[0]; // 默认专注
            if (prompt.Contains("分心") || prompt.Contains("不专注"))
            {
                response = mockResponses[1];
            }

            // 模拟流式输出
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in response)
                {
                    sb.Append(c);
                    string token = c.ToString();

                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        callback.OnToken?.Invoke(token);
                    });

                    System.Threading.Thread.Sleep((int)(tokenInterval * 1000));
                }

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    callback.OnComplete?.Invoke();
                });
            });
        }

        public void Interrupt()
        {
            Debug.Log("[MockLLMClient] 中断（Mock 无实际效果）");
        }

        public void Release()
        {
            _isInitialized = false;
            Debug.Log("[MockLLMClient] 释放（Mock）");
        }

        public void GenerateWithRetry(string prompt, LLMCallback callback, int maxRetries = 2)
        {
            Generate(prompt, callback);
        }
    }
}
