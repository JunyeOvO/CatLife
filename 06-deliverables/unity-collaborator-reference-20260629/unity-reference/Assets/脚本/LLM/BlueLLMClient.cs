using UnityEngine;
using System;
using System.Text;
using CatLife.Core;

namespace CatLife.LLM
{
    /// <summary>
    /// BlueLM 端侧模型 Android 实现
    /// 通过 Unity AndroidJavaClass 调用 LlmManager SDK
    ///
    /// 模型路径: /sdcard/1225/
    /// 纯文本模式 Prompt 模板: [|Human|]:{输入}\n[|AI|]:
    /// 多模态模式 Prompt 模板: [|Human|]:<im_start><image><im_end>{输入}\n[|AI|]:
    /// </summary>
    public class BlueLLMClient : ILLMClient
    {
        public bool IsInitialized => _isInitialized;
        private bool _isInitialized = false;

        private AndroidJavaObject _llmManager;
        private bool _isMultimodal = false;
        private string _modelPath = "/sdcard/1225/";

        // 配置参数
        private const int N_PREDICT = 512;
        private const int N_CTX = 2048;
        private const int N_THREADS = 4;
        private const int NPU_POWER = 100;
        private const float TEMPERATURE = 0.95f;
        private const float TOP_P = 0.8f;
        private const int TOP_K = 50;

        private StringBuilder _responseBuffer = new StringBuilder();

        // AndroidJavaClass 缓存
        private static readonly string LLM_MANAGER_CLASS = "com.vivo.llm.LlmManager";
        private static readonly string LLM_CONFIG_CLASS = "com.vivo.llm.LlmConfig";

        public void Initialize(Action<int> onComplete)
        {
#if !UNITY_ANDROID
            Debug.LogWarning("[BlueLLMClient] 仅支持 Android 平台");
            onComplete?.Invoke(-1);
            return;
#endif

            try
            {
                using (AndroidJavaClass llmManagerClass = new AndroidJavaClass(LLM_MANAGER_CLASS))
                {
                    // 创建 Java LlmManager 实例
                    _llmManager = llmManagerClass.CallStatic<AndroidJavaObject>("getInstance");

                    // 构建 LlmConfig
                    using (AndroidJavaObject config = new AndroidJavaObject(LLM_CONFIG_CLASS))
                    {
                        config.Set<string>("modelPath", _modelPath);
                        config.Set<bool>("multimodal", _isMultimodal);
                        config.Set<int>("nPredict", N_PREDICT);
                        config.Set<int>("nCtx", N_CTX);
                        config.Set<int>("nThreads", N_THREADS);
                        config.Set<int>("npuPower", NPU_POWER);
                        config.Set<float>("temperature", TEMPERATURE);
                        config.Set<float>("topP", TOP_P);
                        config.Set<int>("topK", TOP_K);

                        // 在子线程初始化
                        new System.Threading.Thread(() =>
                        {
                            int ret = _llmManager.Call<int>("init", config);
                            _isInitialized = (ret == 0);

                            UnityMainThreadDispatcher.Enqueue(() =>
                            {
                                if (_isInitialized)
                                {
                                    Debug.Log("[BlueLLMClient] 初始化成功");
                                }
                                else
                                {
                                    Debug.LogError($"[BlueLLMClient] 初始化失败，错误码: {ret}");
                                }
                                onComplete?.Invoke(ret);
                            });
                        }).Start();
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[BlueLLMClient] 初始化异常: {e.Message}");
                onComplete?.Invoke(-999);
            }
        }

        public void Generate(string prompt, LLMCallback callback)
        {
            if (!_isInitialized || _llmManager == null)
            {
                callback?.OnError?.Invoke(-1, "LLM 未初始化");
                return;
            }

            _responseBuffer.Clear();

            try
            {
                // 构建带模板的 prompt
                string fullPrompt = BuildPrompt(prompt);

                // 创建 TokenCallback（匿名内部类通过 UnityPlayer 传递）
                _llmManager.Call("generate", fullPrompt, new TokenCallbackProxy(callback, _responseBuffer));
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[BlueLLMClient] Generate 异常: {e.Message}");
                callback?.OnError?.Invoke(-999, e.Message);
            }
        }

        public void Interrupt()
        {
            if (_llmManager != null)
            {
                try
                {
                    _llmManager.Call("interrupt");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[BlueLLMClient] Interrupt 异常: {e.Message}");
                }
            }
        }

        public void Release()
        {
            if (_llmManager != null)
            {
                try
                {
                    _llmManager.Call("release");
                    _llmManager.Dispose();
                    _llmManager = null;
                }
                catch { }
            }
            _isInitialized = false;
        }

        public void GenerateWithRetry(string prompt, LLMCallback callback, int maxRetries = 2)
        {
            int attempts = 0;
            System.Action retryAction = null;

            retryAction = () =>
            {
                attempts++;
                Generate(prompt, new LLMCallback
                {
                    OnToken = callback.OnToken,
                    OnComplete = callback.OnComplete,
                    OnError = (code, msg) =>
                    {
                        if (attempts < maxRetries)
                        {
                            Debug.LogWarning($"[BlueLLMClient] 生成失败，尝试第 {attempts + 1} 次重试...");
                            ThreadingHelper.Delay(1f, retryAction);
                        }
                        else
                        {
                            callback.OnError?.Invoke(code, msg);
                        }
                    }
                });
            };

            retryAction();
        }

        /// <summary>
        /// 构建带模板的 prompt
        /// </summary>
        private string BuildPrompt(string userInput)
        {
            if (_isMultimodal)
            {
                return $"[|Human|]:<im_start><image><im_end>{userInput}\n[|AI|]:";
            }
            else
            {
                return $"[|Human|]:{userInput}\n[|AI|]:";
            }
        }

        /// <summary>
        /// 设置多模态模式
        /// </summary>
        public void SetMultimodal(bool enable)
        {
            _isMultimodal = enable;
        }

        /// <summary>
        /// 设置模型路径
        /// </summary>
        public void SetModelPath(string path)
        {
            _modelPath = path;
        }
    }

    /// <summary>
    /// Token 回调代理（AndroidJavaProxy 实现 Java TokenCallback 接口）
    /// </summary>
    public class TokenCallbackProxy : AndroidJavaProxy
    {
        private readonly LLMCallback _callback;
        private readonly StringBuilder _buffer;

        // Java 端回调接口完整类名
        private const string CALLBACK_INTERFACE = "com.vivo.llm.LlmManager$TokenCallback";

        public TokenCallbackProxy(LLMCallback callback, StringBuilder buffer)
            : base(CALLBACK_INTERFACE)
        {
            _callback = callback;
            _buffer = buffer;
        }

        // 这些方法名必须与 Java 接口方法名完全一致
        public void onToken(string token)
        {
            _buffer.Append(token);
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _callback.OnToken?.Invoke(token);
            });
        }

        public void onComplete()
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _callback.OnComplete?.Invoke();
            });
        }

        public void onError(int code, string message)
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _callback.OnError?.Invoke(code, message);
            });
        }
    }

    /// <summary>
    /// 主线程调度器（工具类）
    /// </summary>
    public static class UnityMainThread
    {
        private static readonly System.Collections.Generic.Queue<System.Action> _queue =
            new System.Collections.Generic.Queue<System.Action>();

        public static void Enqueue(System.Action action)
        {
            lock (_queue)
            {
                _queue.Enqueue(action);
            }
        }

        public static void Execute()
        {
            while (true)
            {
                System.Action action = null;
                lock (_queue)
                {
                    if (_queue.Count > 0)
                        action = _queue.Dequeue();
                    else
                        break;
                }
                action?.Invoke();
            }
        }
    }

    /// <summary>
    /// 线程帮助工具
    /// </summary>
    public static class ThreadingHelper
    {
        public static void StartThread(System.Action action)
        {
            new System.Threading.Thread(() => action()).Start();
        }

        public static void Delay(float seconds, System.Action callback)
        {
            StartThread(() =>
            {
                System.Threading.Thread.Sleep((int)(seconds * 1000));
                UnityMainThreadDispatcher.Enqueue(callback);
            });
        }
    }
}
