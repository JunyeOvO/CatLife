namespace CatLife.Llm
{
    public sealed class LocalTemplateFallback : IFocusFeedbackProvider
    {
        public System.Threading.Tasks.Task<FocusFeedback> GenerateAsync(
            BehaviorFeatureSummary summary,
            System.Threading.CancellationToken cancellationToken)
        {
            string text;

            if (summary == null)
            {
                text = "猫咪会先安静陪你一会儿，下一轮从一个更小的目标开始。";
            }
            else if (summary.InterruptCount <= 1 && summary.FocusScoreAvg >= 70)
            {
                text = "这一轮很稳，猫咪已经安静陪你完成了一段专注。";
            }
            else if (summary.InterruptCount >= 4)
            {
                text = "刚才有几次短暂停顿，下一轮可以把目标切小一点，猫咪继续陪你回来。";
            }
            else
            {
                text = "你已经把注意力慢慢收回来了，下一轮保持这个节奏就好。";
            }

            return System.Threading.Tasks.Task.FromResult(FocusFeedback.Local(text));
        }
    }
}
