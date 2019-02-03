using System.Collections.Immutable;

namespace AkkaBank.Web.Presentation.Models
{
    public class Steps
    {
        private static readonly Step[] _steps = {
            new Step("Home", "~/Pages/index.cshtml"),
            new Step("Actors", "~/Pages/actors.cshtml"),
            new Step("Messages", "~/Pages/messages.cshtml"),
            new Step("Example Actor", "~/Pages/exampleActor.cshtml"),
            new Step("BasicBank - Awfuly Simple", "~/Pages/ConsoleDirect.cshtml"),
            new Step("BasicBank - Atm V1", "~/Pages/AtmV1.cshtml"),
        };

        public static Step[] Get() => _steps;

        public static Step Get(int number) => number >= 0 && number <= _steps.Length - 1
            ? _steps[number]
            : null;
    }

    public class _LayoutModel : _BlankModel
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

        public _LayoutModel(Step[] steps, int stepNumber) : base(Models.Steps.Get(stepNumber).Title)
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