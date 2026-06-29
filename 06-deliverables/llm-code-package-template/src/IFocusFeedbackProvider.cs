using System.Threading;
using System.Threading.Tasks;

namespace CatLife.Llm
{
    public interface IFocusFeedbackProvider
    {
        Task<FocusFeedback> GenerateAsync(
            BehaviorFeatureSummary summary,
            CancellationToken cancellationToken);
    }
}
