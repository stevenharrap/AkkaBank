using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AkkaBank.Web.Presentation.Models
{
    public class Steps
    {
        public static Step[] Get() => new Step[] {
            new Step("Home", "index")
        };

    }
    public class _LayoutModel
    {
        public ImmutableArray<Step> Steps { get; }
        public int StepNumber { get; }

        public Step PreviousStep => StepNumber - 1 >= 0
            ? Steps[StepNumber - 1]
            : null;
        public Step CurrentStep => Steps[StepNumber];
        public Step NextStep => StepNumber + 1 < Steps.Length - 1
            ? Steps[StepNumber + 1]
            : null;

        public _LayoutModel(Step[] steps, int stepNumber)
        {
            Steps = steps.ToImmutableArray();
            StepNumber = stepNumber;
        }
    }

    public class Step
    {
        public string Title { get; }

        public string Url { get; }

        public Step(string title, string url)
        {
            Title = title;
            Url = url;
        }
    }
}