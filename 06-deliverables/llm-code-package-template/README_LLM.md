# CatLife Large Model Code Package Template

This package is a reviewable template for the competition code bundle. It demonstrates where the large-model API call belongs, how privacy filtering works, and how CatLife degrades to local templates when no network or key is available.

Current status: template only. It is not yet wired into the final Unity/Android project.

## Files

| File | Purpose |
|---|---|
| `src/BehaviorFeatureSummary.cs` | Aggregated, non-sensitive session feature DTO |
| `src/FocusFeedback.cs` | Feedback output DTO |
| `src/IFocusFeedbackProvider.cs` | Provider interface |
| `src/PrivacyGateway.cs` | Allow-list based validation before model calls |
| `src/LocalTemplateFallback.cs` | Offline/no-key fallback text generation |
| `src/LLMExplainClient.cs` | API-call boundary with timeout and fallback |
| `prompts/prompt_focus_explain.md` | Prompt template |
| `samples/sample_feature_summary.json` | Safe sample input |
| `samples/sample_llm_response.json` | Safe sample output |

## Privacy Boundary

Allowed input:

- session duration
- aggregate focus/arousal/distraction scores
- focus block count
- interruption count
- state sequence
- optional user-visible goal text

Forbidden input:

- raw typed content
- raw tap coordinates
- screenshots or OCR
- cross-app behavior
- precise location
- identifiers such as phone number, student ID, email, token, cookie

## API Key Handling

Do not put keys in this package. The integration layer should read a key from runtime configuration or environment variables outside version control.

If no key is configured, use `LocalTemplateFallback`.

## Review Demo

Expected demo flow:

```text
sample_feature_summary.json -> PrivacyGateway -> LLMExplainClient
                                    | no key / timeout / invalid input
                                    v
                              LocalTemplateFallback
```

The model output should be short, non-judgmental, and aligned with CatLife's companion design.
