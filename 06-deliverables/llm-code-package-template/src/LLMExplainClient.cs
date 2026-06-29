using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatLife.Llm
{
    public sealed class LLMExplainClient : IFocusFeedbackProvider
    {
        private readonly HttpClient httpClient;
        private readonly PrivacyGateway privacyGateway;
        private readonly IFocusFeedbackProvider fallback;
        private readonly string endpoint;
        private readonly Func<string> apiKeyProvider;

        public LLMExplainClient(
            HttpClient httpClient,
            PrivacyGateway privacyGateway,
            IFocusFeedbackProvider fallback,
            string endpoint,
            Func<string> apiKeyProvider)
        {
            this.httpClient = httpClient;
            this.privacyGateway = privacyGateway;
            this.fallback = fallback;
            this.endpoint = endpoint;
            this.apiKeyProvider = apiKeyProvider;
        }

        public async Task<FocusFeedback> GenerateAsync(
            BehaviorFeatureSummary summary,
            CancellationToken cancellationToken)
        {
            if (!privacyGateway.CanSend(summary, out _))
            {
                return await fallback.GenerateAsync(summary, cancellationToken);
            }

            string apiKey = apiKeyProvider?.Invoke() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return await fallback.GenerateAsync(summary, cancellationToken);
            }

            using (var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500)))
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token))
            {
                try
                {
                    string payload = BuildPayload(summary);
                    using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                    {
                        request.Headers.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                        using (HttpResponseMessage response = await httpClient.SendAsync(request, linked.Token))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                return await fallback.GenerateAsync(summary, cancellationToken);
                            }

                            string body = await response.Content.ReadAsStringAsync();
                            string text = ExtractText(body);
                            if (string.IsNullOrWhiteSpace(text))
                            {
                                return await fallback.GenerateAsync(summary, cancellationToken);
                            }

                            return FocusFeedback.Llm(TrimFeedback(text));
                        }
                    }
                }
                catch
                {
                    return await fallback.GenerateAsync(summary, cancellationToken);
                }
            }
        }

        private static string BuildPayload(BehaviorFeatureSummary summary)
        {
            string states = string.Join(",", summary.CatStateSequence ?? new System.Collections.Generic.List<string>());
            string goal = Escape(summary.UserVisibleGoal ?? "未填写");

            return "{"
                + "\"model\":\"demo-model\","
                + "\"messages\":["
                + "{\"role\":\"system\",\"content\":\"你是 CatLife 的陪伴式专注反馈助手。\"},"
                + "{\"role\":\"user\",\"content\":\"请根据去标识化摘要生成60字以内反馈。"
                + "duration=" + summary.DurationSec
                + ",focus=" + summary.FocusScoreAvg
                + ",arousal=" + summary.ArousalScoreAvg
                + ",distraction=" + summary.DistractionScoreAvg
                + ",interrupts=" + summary.InterruptCount
                + ",states=" + Escape(states)
                + ",goal=" + goal
                + "\"}"
                + "]"
                + "}";
        }

        private static string ExtractText(string responseBody)
        {
            // Competition template: replace with provider-specific JSON parsing in final integration.
            return responseBody;
        }

        private static string TrimFeedback(string text)
        {
            text = text.Trim();
            return text.Length <= 60 ? text : text.Substring(0, 60);
        }

        private static string Escape(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
