namespace CatLife.Llm
{
    public sealed class FocusFeedback
    {
        public string Text { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public bool IsDegraded { get; set; }

        public static FocusFeedback Local(string text)
        {
            return new FocusFeedback
            {
                Text = text,
                Source = "local_template",
                IsDegraded = true
            };
        }

        public static FocusFeedback Llm(string text)
        {
            return new FocusFeedback
            {
                Text = text,
                Source = "llm",
                IsDegraded = false
            };
        }
    }
}
