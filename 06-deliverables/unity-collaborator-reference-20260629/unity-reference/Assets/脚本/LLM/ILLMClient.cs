using System;
using System.Collections.Generic;

namespace CatLife.LLM
{
    /// <summary>
    /// LLM 客户端接口
    /// 抽象出统一的对话调用方式，方便切换实现（如 BlueLM 本地 / 云端 / Mock）
    /// </summary>
    public interface ILLMClient
    {
        /// <summary>
        /// 是否已初始化
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// 初始化客户端
        /// </summary>
        /// <param name="onComplete">成功返回 0，失败返回错误码</param>
        void Initialize(Action<int> onComplete);

        /// <summary>
        /// 发送对话请求（流式）
        /// </summary>
        /// <param name="prompt">Prompt（需包含模板格式）</param>
        /// <param name="callback">Token 回调</param>
        void Generate(string prompt, LLMCallback callback);

        /// <summary>
        /// 中断当前推理
        /// </summary>
        void Interrupt();

        /// <summary>
        /// 释放资源
        /// </summary>
        void Release();

        /// <summary>
        /// 发送对话请求（带重试）
        /// </summary>
        void GenerateWithRetry(string prompt, LLMCallback callback, int maxRetries = 2);
    }

    /// <summary>
    /// LLM 回调接口
    /// </summary>
    public class LLMCallback
    {
        public Action<string> OnToken;
        public Action OnComplete;
        public Action<int, string> OnError;

        public static LLMCallback Create(Action<string> onToken = null,
                                          Action onComplete = null,
                                          Action<int, string> onError = null)
        {
            return new LLMCallback
            {
                OnToken = onToken,
                OnComplete = onComplete,
                OnError = onError
            };
        }
    }

    /// <summary>
    /// 专注分析结果
    /// </summary>
    [System.Serializable]
    public class FocusAnalysisResult
    {
        public bool isFocused;       // 是否专注
        public float score;          // 专注分数 0-1
        public string suggestion;    // 建议文本
        public string rawResponse;   // 原始 LLM 输出（用于调试）

        public static FocusAnalysisResult Default()
        {
            return new FocusAnalysisResult
            {
                isFocused = false,
                score = 0.5f,
                suggestion = "",
                rawResponse = ""
            };
        }
    }

    /// <summary>
    /// LLM 响应解析结果
    /// </summary>
    [System.Serializable]
    public class LLMResponse
    {
        public bool success;
        public string content;
        public int errorCode;
        public string errorMessage;

        public static LLMResponse Success(string content) =>
            new LLMResponse { success = true, content = content };

        public static LLMResponse Error(int code, string message) =>
            new LLMResponse { success = false, errorCode = code, errorMessage = message };
    }
}
