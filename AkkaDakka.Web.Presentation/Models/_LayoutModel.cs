using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AkkaBank.Web.Presentation.Models
{
    public class Steps
    {
        private static readonly Step[] _steps = {
            new Step("Home", "~/Pages/index.cshtml"),
            new Step("Actors", "~/Pages/actors.cshtml")
        };

        public static Step[] Get() => _steps;

        public static Step Get(int number) => _steps[number];

    }
    public class _LayoutModel
    {
        public ImmutableArray<Step> Steps { get; }
        public int StepNumber { get; }

        public int? PreviousStep => StepNumber - 1 >= 0
            ? StepNumber - 1
            : (int?)null;
        public Step CurrentStep => Steps[StepNumber];
        public int? NextStep => StepNumber + 1 < Steps.Length
            ? StepNumber + 1
            : (int?)null;

        public _LayoutModel(Step[] steps, int stepNumber)
        {
            Steps = steps.ToImmutableArray();
            StepNumber = stepNumber;
        }
    }

    public class Step
    {
        public string Title { get; }

        public string View { get; }

        public Step(string title, string view)
        {
            Title = title;
            View = view;
        }
    }
}