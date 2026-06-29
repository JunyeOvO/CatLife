using System.Collections.Generic;

namespace CatLife.Llm
{
    public sealed class BehaviorFeatureSummary
    {
        public string SessionId { get; set; } = string.Empty;
        public int DurationSec { get; set; }
        public int FocusScoreAvg { get; set; }
        public int ArousalScoreAvg { get; set; }
        public int DistractionScoreAvg { get; set; }
        public int FocusBlocks { get; set; }
        public int LongestFocusSec { get; set; }
        public int InterruptCount { get; set; }
        public List<string> CatStateSequence { get; set; } = new List<string>();
        public string UserVisibleGoal { get; set; } = string.Empty;
        public string Locale { get; set; } = "zh-CN";
    }
}
