using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CatLife.Llm
{
    public sealed class PrivacyGateway
    {
        private static readonly Regex SensitivePattern = new Regex(
            @"(sk-|api[_-]?key|token|password|手机号|学号|身份证|邮箱|@)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool CanSend(BehaviorFeatureSummary summary, out string reason)
        {
            if (summary == null)
            {
                reason = "summary_missing";
                return false;
            }

            if (summary.DurationSec < 0 || summary.DurationSec > 24 * 60 * 60)
            {
                reason = "duration_out_of_range";
                return false;
            }

            if (!IsScore(summary.FocusScoreAvg) ||
                !IsScore(summary.ArousalScoreAvg) ||
                !IsScore(summary.DistractionScoreAvg))
            {
                reason = "score_out_of_range";
                return false;
            }

            if (summary.CatStateSequence == null || summary.CatStateSequence.Count == 0)
            {
                reason = "state_sequence_missing";
                return false;
            }

            if (summary.CatStateSequence.Any(s => s == null || s.Length > 32))
            {
                reason = "state_sequence_invalid";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(summary.UserVisibleGoal) &&
                SensitivePattern.IsMatch(summary.UserVisibleGoal))
            {
                reason = "user_goal_contains_sensitive_text";
                return false;
            }

            reason = "ok";
            return true;
        }

        private static bool IsScore(int score)
        {
            return score >= 0 && score <= 100;
        }
    }
}
